using Medium.Resources;

using Microsoft.Extensions.DependencyInjection;

namespace Medium;

/// <summary>
/// Binds components to middleware delegates for processing payloads.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class ComponentBinder<TRequest> : IComponentBinder<TRequest>
{
    private ContextualAsyncMiddlewareDelegate<TRequest>? AsyncMiddlewareDelegate;
    private ContextualMiddlewareDelegate<TRequest>? MiddlewareDelegate;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualAsyncMiddlewareDelegate<TRequest> GetAsyncMiddlewareDelegate()
    {
        if(AsyncMiddlewareDelegate is not null)
            return AsyncMiddlewareDelegate;
        if(MiddlewareDelegate is not null)
            return context => {
                MiddlewareDelegate(context);
                return Task.CompletedTask;
            };

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualMiddlewareDelegate<TRequest> GetMiddlewareDelegate()
    {
        if(MiddlewareDelegate is not null)
            return MiddlewareDelegate;
        if(AsyncMiddlewareDelegate is not null)
            return context => AsyncMiddlewareDelegate(context).GetAwaiter().GetResult();

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }
    
    /// <inheritdoc/>
    public IComponentBinder<TRequest> Init(TerminateComponentDescriptor<TRequest> descriptor)
    {
        if(descriptor.AsyncAction != null) {
            AsyncMiddlewareDelegate = context => descriptor.AsyncAction(context.Request);
            if(descriptor.Action is null)
                MiddlewareDelegate = context => descriptor.AsyncAction(context.Request).GetAwaiter().GetResult();
        }

        if(descriptor.Action != null) {
            MiddlewareDelegate = context => descriptor.Action(context.Request);
            if(descriptor.AsyncAction is null)
                AsyncMiddlewareDelegate = context => {
                    descriptor.Action(context.Request);
                    return Task.CompletedTask;
                };
        }

        return this;
    }

#if NETSTANDARD2_0
    /// <inheritdoc/>
    public IComponentBinder<TRequest> BindComponents(IReadOnlyCollection<ComponentDescriptor<TRequest>> descriptors)
    {
        foreach (var descriptor in descriptors)
            BindToComponent(descriptor);

        return this;
    }
#endif

    /// <inheritdoc/>
    public IComponentBinder<TRequest> BindToComponent(ComponentDescriptor<TRequest> descriptor)
    {
        var asyncMiddlewareDelegate = BindToAsyncDelegate(descriptor);
        var middlewareDelegate = BindToDelegate(descriptor);

        if(descriptor.Condition is not null) {
            var nextAsyncDelegate = AsyncMiddlewareDelegate;
            var nextDelegate = MiddlewareDelegate;
            if(nextAsyncDelegate is null && nextDelegate is not null) {
                nextAsyncDelegate = context => {
                    nextDelegate(context);
                    return Task.CompletedTask;
                };
            }

            if(nextDelegate is null && nextAsyncDelegate is not null) {
                nextDelegate = context => nextAsyncDelegate(context).GetAwaiter().GetResult();
            }

            AsyncMiddlewareDelegate = asyncMiddlewareDelegate is null ? null : context => {
                if(descriptor.Condition(context.Request))
                    return asyncMiddlewareDelegate(context);
                if(nextAsyncDelegate is not null)
                    return nextAsyncDelegate(context);

                throw new InvalidOperationException(Errors.NextMiddlewareNotDefined);
            };

            MiddlewareDelegate = middlewareDelegate is null ? null : context => {
                if(descriptor.Condition(context.Request)) {
                    middlewareDelegate(context);
                    return;
                }
                if(nextDelegate is not null) {
                    nextDelegate(context);
                    return;
                }

                throw new InvalidOperationException(Errors.NextMiddlewareNotDefined);
            };
        }
        else {
            AsyncMiddlewareDelegate = asyncMiddlewareDelegate;
            MiddlewareDelegate = middlewareDelegate;
        }

        return this;
    }

    /// <summary>
    /// Binds the asynchronous middleware delegate to the specified component descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private ContextualAsyncMiddlewareDelegate<TRequest>? BindToAsyncDelegate(ComponentDescriptor<TRequest> descriptor)
    {
        AsyncMiddlewareSPFunc<TRequest>? asyncMiddlewareAction = default;
        if(descriptor.MiddlewareType is not null && typeof(IAsyncMiddleware<TRequest>).IsAssignableFrom(descriptor.MiddlewareType)) {
            asyncMiddlewareAction = (sp, next) => (r, ct) => {
                var middleware = (IAsyncMiddleware<TRequest>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                return middleware.InvokeAsync(r, next, ct);
            };
        }
        else {
            asyncMiddlewareAction = descriptor.AsyncMiddlewareAction;
        }
        if(asyncMiddlewareAction is not null) {
            if(AsyncMiddlewareDelegate is not null)
                return BindAsync(asyncMiddlewareAction, AsyncMiddlewareDelegate);
            else if(MiddlewareDelegate is not null)
                return BindAsync(asyncMiddlewareAction, MiddlewareDelegate);
        }
        return null;
    }

    /// <summary>
    /// Binds the middleware delegate to the specified component descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The bound middleware delegate.</returns>
    private ContextualMiddlewareDelegate<TRequest>? BindToDelegate(ComponentDescriptor<TRequest> descriptor)
    {
        MiddlewareSPFunc<TRequest>? middlewareAction = default;
        if(descriptor.MiddlewareType is not null && typeof(IMiddleware<TRequest>).IsAssignableFrom(descriptor.MiddlewareType)) {
            middlewareAction = (sp, next) => r => {
                var middleware = (IMiddleware<TRequest>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                middleware.Invoke(r, next);
            };
        }
        else {
            middlewareAction = descriptor.MiddlewareAction;
        }
        if(middlewareAction is not null) {
            if(MiddlewareDelegate is not null)
                return Bind(middlewareAction, MiddlewareDelegate);
            else if(AsyncMiddlewareDelegate is not null)
                return Bind(middlewareAction, AsyncMiddlewareDelegate);
        }
        return null;
    }

    #region Binders
    /// <summary>
    /// Binds an asynchronous middleware function to the next asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <param name="next">The next asynchronous middleware delegate.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private static ContextualAsyncMiddlewareDelegate<TRequest> BindAsync(AsyncMiddlewareSPFunc<TRequest> middleware, ContextualAsyncMiddlewareDelegate<TRequest> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Request, context.CancellationToken);
    }

    /// <summary>
    /// Binds an asynchronous middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private static ContextualAsyncMiddlewareDelegate<TRequest> BindAsync(AsyncMiddlewareSPFunc<TRequest> middleware, ContextualMiddlewareDelegate<TRequest> next)
    {
        return context => middleware(context.ServiceProvider, () => {
            next(context);
            return Task.CompletedTask;
        })(context.Request, context.CancellationToken);
    }

    /// <summary>
    /// Binds a middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TRequest> Bind(MiddlewareSPFunc<TRequest> middleware, ContextualMiddlewareDelegate<TRequest> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Request);
    }

    /// <summary>
    /// Binds a middleware function to the next asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next asynchronous middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TRequest> Bind(MiddlewareSPFunc<TRequest> middleware, ContextualAsyncMiddlewareDelegate<TRequest> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context).GetAwaiter().GetResult())(context.Request);
    }
    #endregion
}

/// <summary>
/// Binds components to middleware delegates for processing payloads and returning results.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class ComponentBinder<TRequest, TResult> : IComponentBinder<TRequest, TResult>
{
    protected ContextualAsyncMiddlewareDelegate<TRequest, TResult>? AsyncMiddlewareDelegate;
    protected ContextualMiddlewareDelegate<TRequest, TResult>? MiddlewareDelegate;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualAsyncMiddlewareDelegate<TRequest, TResult> GetAsyncMiddlewareDelegate()
    {
        if(AsyncMiddlewareDelegate is not null)
            return AsyncMiddlewareDelegate;
        if(MiddlewareDelegate is not null)
            return context => Task.FromResult(MiddlewareDelegate(context));

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualMiddlewareDelegate<TRequest, TResult> GetMiddlewareDelegate()
    {
        if(MiddlewareDelegate is not null)
            return MiddlewareDelegate;
        if(AsyncMiddlewareDelegate is not null)
            return context => AsyncMiddlewareDelegate(context).GetAwaiter().GetResult();

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <inheritdoc/>
    public IComponentBinder<TRequest, TResult> Init(TerminateComponentDescriptor<TRequest, TResult> descriptor)
    {
        if(descriptor.AsyncFunc is not null) {
            AsyncMiddlewareDelegate = context => descriptor.AsyncFunc(context.Request);
            if(descriptor.Func is null)
                MiddlewareDelegate = context => descriptor.AsyncFunc(context.Request).GetAwaiter().GetResult();
        }

        if(descriptor.Func is not null) {
            MiddlewareDelegate = context => descriptor.Func(context.Request);
            if(descriptor.AsyncFunc is null)
                AsyncMiddlewareDelegate = context => {
                    return Task.FromResult(descriptor.Func(context.Request));
                };
        }

        return this;
    }

#if NETSTANDARD2_0
    /// <inheritdoc/>
    public IComponentBinder<TRequest, TResult> BindComponents(IReadOnlyCollection<ComponentDescriptor<TRequest, TResult>> descriptors)
    {
        foreach (var descriptor in descriptors)
            BindToComponent(descriptor);

        return this;
    }
#endif

    /// <inheritdoc/>
    public IComponentBinder<TRequest, TResult> BindToComponent(ComponentDescriptor<TRequest, TResult> descriptor)
    {
        var asyncMiddlewareDelegate = BindToAsyncDelegate(descriptor);
        var middlewareDelegate = BindToDelegate(descriptor);

        if(descriptor.Condition is not null) {
            var nextAsyncDelegate = AsyncMiddlewareDelegate;
            var nextDelegate = MiddlewareDelegate;
            if(nextAsyncDelegate is null && nextDelegate is not null) {
                nextAsyncDelegate = context => {
                    nextDelegate(context);
                    return Task.FromResult(nextDelegate(context));
                };
            }

            if(nextDelegate is null && nextAsyncDelegate is not null) {
                nextDelegate = context => nextAsyncDelegate(context).GetAwaiter().GetResult();
            }

            AsyncMiddlewareDelegate = asyncMiddlewareDelegate is null ? null : context => {
                if(descriptor.Condition(context.Request))
                    return asyncMiddlewareDelegate(context);
                if(nextAsyncDelegate is not null)
                    return nextAsyncDelegate(context);

                throw new InvalidOperationException(Errors.NextMiddlewareNotDefined);
            };

            MiddlewareDelegate = middlewareDelegate is null ? null : context => {
                if(descriptor.Condition(context.Request))
                    return middlewareDelegate(context);
                if(nextDelegate is not null)
                    return nextDelegate(context);

                throw new InvalidOperationException(Errors.NextMiddlewareNotDefined);
            };
        }
        else {
            AsyncMiddlewareDelegate = asyncMiddlewareDelegate;
            MiddlewareDelegate = middlewareDelegate;
        }

        return this;
    }

    /// <summary>
    /// Binds the asynchronous middleware delegate to the specified component descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private ContextualAsyncMiddlewareDelegate<TRequest, TResult>? BindToAsyncDelegate(ComponentDescriptor<TRequest, TResult> descriptor)
    {
        AsyncMiddlewareSPFunc<TRequest, TResult>? asyncMiddlewareFunc = default;
        if(descriptor.MiddlewareType is not null && typeof(IAsyncMiddleware<TRequest, TResult>).IsAssignableFrom(descriptor.MiddlewareType)) {
            asyncMiddlewareFunc = (sp, next) => (r, ct) => {
                var middleware = (IAsyncMiddleware<TRequest, TResult>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                return middleware.InvokeAsync(r, next, ct);
            };
        }
        else {
            asyncMiddlewareFunc = descriptor.AsyncMiddlewareFunc;
        }
        if(asyncMiddlewareFunc is not null) {
            if(AsyncMiddlewareDelegate is not null)
                return BindAsync(asyncMiddlewareFunc, AsyncMiddlewareDelegate);
            else if(MiddlewareDelegate is not null)
                return BindAsync(asyncMiddlewareFunc, MiddlewareDelegate);
        }
        return null;
    }

    /// <summary>
    /// Binds the middleware delegate to the specified component descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The bound middleware delegate.</returns>
    private ContextualMiddlewareDelegate<TRequest, TResult>? BindToDelegate(ComponentDescriptor<TRequest, TResult> descriptor)
    {
        MiddlewareSPFunc<TRequest, TResult>? middlewareFunc = default;
        if(descriptor.MiddlewareType is not null && typeof(IMiddleware<TRequest, TResult>).IsAssignableFrom(descriptor.MiddlewareType)) {
            middlewareFunc = (sp, next) => r => {
                var middleware = (IMiddleware<TRequest, TResult>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                return middleware.Invoke(r, next);
            };
        }
        else {
            middlewareFunc = descriptor.MiddlewareFunc;
        }
        if(middlewareFunc is not null) {
            if(MiddlewareDelegate is not null)
                return Bind(middlewareFunc, MiddlewareDelegate);
            else if(AsyncMiddlewareDelegate is not null)
                return Bind(middlewareFunc, AsyncMiddlewareDelegate);
        }
        return null;
    }

    #region Binders
    /// <summary>
    /// Binds an asynchronous middleware function to the next asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <param name="next">The next asynchronous middleware delegate.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private static ContextualAsyncMiddlewareDelegate<TRequest, TResult> BindAsync(AsyncMiddlewareSPFunc<TRequest, TResult> middleware, ContextualAsyncMiddlewareDelegate<TRequest, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Request, context.CancellationToken);
    }

    /// <summary>
    /// Binds an asynchronous middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private static ContextualAsyncMiddlewareDelegate<TRequest, TResult> BindAsync(AsyncMiddlewareSPFunc<TRequest, TResult> middleware, ContextualMiddlewareDelegate<TRequest, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => Task.FromResult(next(context)))(context.Request, context.CancellationToken);
    }

    /// <summary>
    /// Binds a middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TRequest, TResult> Bind(MiddlewareSPFunc<TRequest, TResult> middleware, ContextualMiddlewareDelegate<TRequest, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Request);
    }

    /// <summary>
    /// Binds a middleware function to the next asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next asynchronous middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TRequest, TResult> Bind(MiddlewareSPFunc<TRequest, TResult> middleware, ContextualAsyncMiddlewareDelegate<TRequest, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context).GetAwaiter().GetResult())(context.Request);
    }
    #endregion
}