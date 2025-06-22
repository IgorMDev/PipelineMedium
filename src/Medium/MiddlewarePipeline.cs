using Medium.Resources;

using Microsoft.Extensions.DependencyInjection;

namespace Medium;

/// <summary>
/// Represents a pipeline for executing middleware components for a specific request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class MiddlewarePipeline<TRequest>
{
    private ContextualAsyncMiddlewareDelegate<TRequest>? AsyncMiddlewareDelegate;
    private ContextualMiddlewareDelegate<TRequest>? MiddlewareDelegate;

    public MiddlewarePipeline(TerminationMiddlewareDescriptor<TRequest> descriptor)
    {
        Init(descriptor);
    }

    protected void Init(TerminationMiddlewareDescriptor<TRequest> descriptor)
    {
        if (descriptor.AsyncAction != null) {
            AsyncMiddlewareDelegate = context => descriptor.AsyncAction(context.Request);
            if (descriptor.Action is null)
                MiddlewareDelegate = context => descriptor.AsyncAction(context.Request).GetAwaiter().GetResult();
        }

        if (descriptor.Action != null) {
            MiddlewareDelegate = context => descriptor.Action(context.Request);
            if (descriptor.AsyncAction is null)
                AsyncMiddlewareDelegate = context => {
                    descriptor.Action(context.Request);
                    return Task.CompletedTask;
                };
        }
    }

    /// <summary>
    /// Gets asynchronous middleware delegate for the pipeline to invoke.
    /// </summary>
    /// <returns>Asynchronous middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualAsyncMiddlewareDelegate<TRequest> ToAsyncMiddlewareDelegate()
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
    /// Gets middleware delegate for the pipeline to invoke.
    /// </summary>
    /// <returns>Middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualMiddlewareDelegate<TRequest> ToMiddlewareDelegate()
    {
        if(MiddlewareDelegate is not null)
            return MiddlewareDelegate;
        if(AsyncMiddlewareDelegate is not null)
            return context => AsyncMiddlewareDelegate(context).GetAwaiter().GetResult();

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <summary>
    /// Adds middlewares to the pipeline.
    /// </summary>
    /// <param name="descriptors">A collection of middleware descriptors.</param>
    /// <returns>The current <see cref="MiddlewarePipeline{TRequest}"/> instance.</returns>
    public MiddlewarePipeline<TRequest> AddMiddlewares(IReadOnlyCollection<MiddlewareDescriptor<TRequest>> descriptors)
    {
        foreach (var descriptor in descriptors)
            AddMiddleware(descriptor);

        return this;
    }

    /// <summary>
    /// Adds a middleware to the pipeline.
    /// </summary>
    /// <param name="descriptor">Middleware descriptor.</param>
    /// <returns>The current <see cref="MiddlewarePipeline{TRequest}"/> instance.</returns>
    public MiddlewarePipeline<TRequest> AddMiddleware(MiddlewareDescriptor<TRequest> descriptor)
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
    private ContextualAsyncMiddlewareDelegate<TRequest>? BindToAsyncDelegate(MiddlewareDescriptor<TRequest> descriptor)
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
    private ContextualMiddlewareDelegate<TRequest>? BindToDelegate(MiddlewareDescriptor<TRequest> descriptor)
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
public class MiddlewarePipeline<TRequest, TResult>
{
    protected ContextualAsyncMiddlewareDelegate<TRequest, TResult>? AsyncMiddlewareDelegate;
    protected ContextualMiddlewareDelegate<TRequest, TResult>? MiddlewareDelegate;

    public MiddlewarePipeline(TerminationMiddlewareDescriptor<TRequest, TResult> descriptor)
    {
        Init(descriptor);
    }

    protected MiddlewarePipeline<TRequest, TResult> Init(TerminationMiddlewareDescriptor<TRequest, TResult> descriptor)
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

    /// <summary>
    /// Gets asynchronous middleware delegate for the pipeline to invoke.
    /// </summary>
    /// <returns>Asynchronous middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualAsyncMiddlewareDelegate<TRequest, TResult> ToAsyncMiddlewareDelegate()
    {
        if(AsyncMiddlewareDelegate is not null)
            return AsyncMiddlewareDelegate;
        if(MiddlewareDelegate is not null)
            return context => Task.FromResult(MiddlewareDelegate(context));

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <summary>
    /// Gets middleware delegate for the pipeline to invoke.
    /// </summary>
    /// <returns>Middleware delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no middleware is defined.</exception>
    public ContextualMiddlewareDelegate<TRequest, TResult> ToMiddlewareDelegate()
    {
        if(MiddlewareDelegate is not null)
            return MiddlewareDelegate;
        if(AsyncMiddlewareDelegate is not null)
            return context => AsyncMiddlewareDelegate(context).GetAwaiter().GetResult();

        throw new InvalidOperationException(Errors.MiddlewareNotDefined);
    }

    /// <summary>
    /// Adds middlewares to the pipeline.
    /// </summary>
    /// <param name="descriptors">A collection of middleware descriptors.</param>
    /// <returns>The current <see cref="MiddlewarePipeline{TRequest, TResult}"/> instance.</returns>
    public MiddlewarePipeline<TRequest, TResult> AddMiddlewares(IReadOnlyCollection<MiddlewareDescriptor<TRequest, TResult>> descriptors)
    {
        foreach (var descriptor in descriptors)
            AddMiddleware(descriptor);

        return this;
    }

    /// <summary>
    /// Adds a middleware to the pipeline.
    /// </summary>
    /// <param name="descriptor">Middleware descriptor.</param>
    /// <returns>The current <see cref="MiddlewarePipeline{TRequest, TResult}"/> instance.</returns>
    public MiddlewarePipeline<TRequest, TResult> AddMiddleware(MiddlewareDescriptor<TRequest, TResult> descriptor)
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
    private ContextualAsyncMiddlewareDelegate<TRequest, TResult>? BindToAsyncDelegate(MiddlewareDescriptor<TRequest, TResult> descriptor)
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
    private ContextualMiddlewareDelegate<TRequest, TResult>? BindToDelegate(MiddlewareDescriptor<TRequest, TResult> descriptor)
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