using Medium.Resources;

using Microsoft.Extensions.DependencyInjection;

namespace Medium;

/// <summary>
/// Binds components to middleware delegates for processing payloads.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public class ComponentBinder<TPayload> : IComponentBinder<TPayload>
{
    private ContextualAsyncMiddlewareDelegate<TPayload>? AsyncMiddlewareDelegate;
    private ContextualMiddlewareDelegate<TPayload>? MiddlewareDelegate;

    /// <summary>
    /// Gets the asynchronous middleware delegate.
    /// </summary>
    /// <returns>The asynchronous middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualAsyncMiddlewareDelegate<TPayload> GetAsyncMiddlewareDelegate()
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

    /// <summary>
    /// Gets the middleware delegate.
    /// </summary>
    /// <returns>The middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualMiddlewareDelegate<TPayload> GetMiddlewareDelegate()
    {
        if(MiddlewareDelegate is not null)
            return MiddlewareDelegate;
        if(AsyncMiddlewareDelegate is not null)
            return context => AsyncMiddlewareDelegate(context).GetAwaiter().GetResult();

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <summary>
    /// Initializes the binder with a terminate component descriptor.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload}"/> instance.</returns>
    public IComponentBinder<TPayload> Init(TerminateComponentDescriptor<TPayload> descriptor)
    {
        if(descriptor.AsyncAction != null) {
            AsyncMiddlewareDelegate = context => descriptor.AsyncAction(context.Payload);
            if(descriptor.Action is null)
                MiddlewareDelegate = context => descriptor.AsyncAction(context.Payload).GetAwaiter().GetResult();
        }

        if(descriptor.Action != null) {
            MiddlewareDelegate = context => descriptor.Action(context.Payload);
            if(descriptor.AsyncAction is null)
                AsyncMiddlewareDelegate = context => {
                    descriptor.Action(context.Payload);
                    return Task.CompletedTask;
                };
        }

        return this;
    }

    /// <summary>
    /// Binds a single component to the binder using the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload}"/> instance.</returns>
    public IComponentBinder<TPayload> BindToComponent(ComponentDescriptor<TPayload> descriptor)
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
                if(descriptor.Condition(context.Payload))
                    return asyncMiddlewareDelegate(context);
                if(nextAsyncDelegate is not null)
                    return nextAsyncDelegate(context);

                throw new InvalidOperationException(Errors.NextMiddlewareNotDefined);
            };

            MiddlewareDelegate = middlewareDelegate is null ? null : context => {
                if(descriptor.Condition(context.Payload)) {
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
    private ContextualAsyncMiddlewareDelegate<TPayload>? BindToAsyncDelegate(ComponentDescriptor<TPayload> descriptor)
    {
        AsyncMiddlewareSPFunc<TPayload>? asyncMiddlewareAction = default;
        if(descriptor.MiddlewareType is not null && typeof(IAsyncMiddleware<TPayload>).IsAssignableFrom(descriptor.MiddlewareType)) {
            asyncMiddlewareAction = (sp, next) => (p, ct) => {
                var middleware = (IAsyncMiddleware<TPayload>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                return middleware.InvokeAsync(p, next, ct);
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
    private ContextualMiddlewareDelegate<TPayload>? BindToDelegate(ComponentDescriptor<TPayload> descriptor)
    {
        MiddlewareSPFunc<TPayload>? middlewareAction = default;
        if(descriptor.MiddlewareType is not null && typeof(IMiddleware<TPayload>).IsAssignableFrom(descriptor.MiddlewareType)) {
            middlewareAction = (sp, next) => p => {
                var middleware = (IMiddleware<TPayload>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                middleware.Invoke(p, next);
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
    private static ContextualAsyncMiddlewareDelegate<TPayload> BindAsync(AsyncMiddlewareSPFunc<TPayload> middleware, ContextualAsyncMiddlewareDelegate<TPayload> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Payload, context.CancellationToken);
    }

    /// <summary>
    /// Binds an asynchronous middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private static ContextualAsyncMiddlewareDelegate<TPayload> BindAsync(AsyncMiddlewareSPFunc<TPayload> middleware, ContextualMiddlewareDelegate<TPayload> next)
    {
        return context => middleware(context.ServiceProvider, () => {
            next(context);
            return Task.CompletedTask;
        })(context.Payload, context.CancellationToken);
    }

    /// <summary>
    /// Binds a middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TPayload> Bind(MiddlewareSPFunc<TPayload> middleware, ContextualMiddlewareDelegate<TPayload> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Payload);
    }

    /// <summary>
    /// Binds a middleware function to the next asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next asynchronous middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TPayload> Bind(MiddlewareSPFunc<TPayload> middleware, ContextualAsyncMiddlewareDelegate<TPayload> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context).GetAwaiter().GetResult())(context.Payload);
    }
    #endregion
}

/// <summary>
/// Binds components to middleware delegates for processing payloads and returning results.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class ComponentBinder<TPayload, TResult> : IComponentBinder<TPayload, TResult>
{
    protected ContextualAsyncMiddlewareDelegate<TPayload, TResult>? AsyncMiddlewareDelegate;
    protected ContextualMiddlewareDelegate<TPayload, TResult>? MiddlewareDelegate;

    /// <summary>
    /// Gets the asynchronous middleware delegate.
    /// </summary>
    /// <returns>The asynchronous middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualAsyncMiddlewareDelegate<TPayload, TResult> GetAsyncMiddlewareDelegate()
    {
        if(AsyncMiddlewareDelegate is not null)
            return AsyncMiddlewareDelegate;
        if(MiddlewareDelegate is not null)
            return context => Task.FromResult(MiddlewareDelegate(context));

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <summary>
    /// Gets the middleware delegate.
    /// </summary>
    /// <returns>The middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualMiddlewareDelegate<TPayload, TResult> GetMiddlewareDelegate()
    {
        if(MiddlewareDelegate is not null)
            return MiddlewareDelegate;
        if(AsyncMiddlewareDelegate is not null)
            return context => AsyncMiddlewareDelegate(context).GetAwaiter().GetResult();

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <summary>
    /// Initializes the binder with a terminate component descriptor.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload, TResult}"/> instance.</returns>
    public IComponentBinder<TPayload, TResult> Init(TerminateComponentDescriptor<TPayload, TResult> descriptor)
    {
        if(descriptor.AsyncFunc is not null) {
            AsyncMiddlewareDelegate = context => descriptor.AsyncFunc(context.Payload);
            if(descriptor.Func is null)
                MiddlewareDelegate = context => descriptor.AsyncFunc(context.Payload).GetAwaiter().GetResult();
        }

        if(descriptor.Func is not null) {
            MiddlewareDelegate = context => descriptor.Func(context.Payload);
            if(descriptor.AsyncFunc is null)
                AsyncMiddlewareDelegate = context => {
                    return Task.FromResult(descriptor.Func(context.Payload));
                };
        }

        return this;
    }

    /// <summary>
    /// Binds a single component to the binder using the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload, TResult}"/> instance.</returns>
    public IComponentBinder<TPayload, TResult> BindToComponent(ComponentDescriptor<TPayload, TResult> descriptor)
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
                if(descriptor.Condition(context.Payload))
                    return asyncMiddlewareDelegate(context);
                if(nextAsyncDelegate is not null)
                    return nextAsyncDelegate(context);

                throw new InvalidOperationException(Errors.NextMiddlewareNotDefined);
            };

            MiddlewareDelegate = middlewareDelegate is null ? null : context => {
                if(descriptor.Condition(context.Payload))
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
    private ContextualAsyncMiddlewareDelegate<TPayload, TResult>? BindToAsyncDelegate(ComponentDescriptor<TPayload, TResult> descriptor)
    {
        AsyncMiddlewareSPFunc<TPayload, TResult>? asyncMiddlewareFunc = default;
        if(descriptor.MiddlewareType is not null && typeof(IAsyncMiddleware<TPayload, TResult>).IsAssignableFrom(descriptor.MiddlewareType)) {
            asyncMiddlewareFunc = (sp, next) => (p, ct) => {
                var middleware = (IAsyncMiddleware<TPayload, TResult>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                return middleware.InvokeAsync(p, next, ct);
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
    private ContextualMiddlewareDelegate<TPayload, TResult>? BindToDelegate(ComponentDescriptor<TPayload, TResult> descriptor)
    {
        MiddlewareSPFunc<TPayload, TResult>? middlewareFunc = default;
        if(descriptor.MiddlewareType is not null && typeof(IMiddleware<TPayload, TResult>).IsAssignableFrom(descriptor.MiddlewareType)) {
            middlewareFunc = (sp, next) => p => {
                var middleware = (IMiddleware<TPayload, TResult>)ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.MiddlewareType);
                return middleware.Invoke(p, next);
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
    private static ContextualAsyncMiddlewareDelegate<TPayload, TResult> BindAsync(AsyncMiddlewareSPFunc<TPayload, TResult> middleware, ContextualAsyncMiddlewareDelegate<TPayload, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Payload, context.CancellationToken);
    }

    /// <summary>
    /// Binds an asynchronous middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound asynchronous middleware delegate.</returns>
    private static ContextualAsyncMiddlewareDelegate<TPayload, TResult> BindAsync(AsyncMiddlewareSPFunc<TPayload, TResult> middleware, ContextualMiddlewareDelegate<TPayload, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => Task.FromResult(next(context)))(context.Payload, context.CancellationToken);
    }

    /// <summary>
    /// Binds a middleware function to the next middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TPayload, TResult> Bind(MiddlewareSPFunc<TPayload, TResult> middleware, ContextualMiddlewareDelegate<TPayload, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context))(context.Payload);
    }

    /// <summary>
    /// Binds a middleware function to the next asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <param name="next">The next asynchronous middleware delegate.</param>
    /// <returns>The bound middleware delegate.</returns>
    private static ContextualMiddlewareDelegate<TPayload, TResult> Bind(MiddlewareSPFunc<TPayload, TResult> middleware, ContextualAsyncMiddlewareDelegate<TPayload, TResult> next)
    {
        return context => middleware(context.ServiceProvider, () => next(context).GetAwaiter().GetResult())(context.Payload);
    }
    #endregion
}