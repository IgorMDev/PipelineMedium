namespace Medium;

/// <summary>
/// Represents a descriptor for a middleware.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class MiddlewareDescriptor<TRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class.
    /// </summary>
    internal MiddlewareDescriptor() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with a middleware type.
    /// </summary>
    /// <param name="middlewareType">The type of the middleware.</param>
    public MiddlewareDescriptor(Type middlewareType)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareType);
#else
        if (middlewareType is null) {
            throw new ArgumentNullException(nameof(middlewareType));
        }
#endif        
        MiddlewareType = middlewareType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with an asynchronous middleware action.
    /// </summary>
    /// <param name="middlewareAction">The asynchronous middleware action.</param>
    public MiddlewareDescriptor(AsyncMiddlewareSPFunc<TRequest> middlewareAction)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareAction);
#else
        if (middlewareAction is null) {
            throw new ArgumentNullException(nameof(middlewareAction));
        }
#endif   
        AsyncMiddlewareAction = middlewareAction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    public MiddlewareDescriptor(AsyncMiddlewareFunc<TRequest> middleware)
        : this((_, next) => middleware(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with an asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public MiddlewareDescriptor(Func<TRequest, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        : this((_, next) => (request, ct) => middleware(request, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with an asynchronous middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public MiddlewareDescriptor(Func<IServiceProvider, TRequest, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        : this((sp, next) => (request, ct) => middleware(sp, request, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with a middleware action.
    /// </summary>
    /// <param name="middlewareAction">The middleware action.</param>
    public MiddlewareDescriptor(MiddlewareSPFunc<TRequest> middlewareAction)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareAction);
#else
        if (middlewareAction is null) {
            throw new ArgumentNullException(nameof(middlewareAction));
        }
#endif  
        MiddlewareAction = middlewareAction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with a middleware function.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    public MiddlewareDescriptor(MiddlewareFunc<TRequest> middleware)
        : this((_, next) => middleware(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with a middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public MiddlewareDescriptor(Action<TRequest, NextMiddlewareDelegate> middleware)
        : this((_, next) => request => middleware(request, next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest}"/> class with a middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public MiddlewareDescriptor(Action<IServiceProvider, TRequest, NextMiddlewareDelegate> middleware)
        : this((sp, next) => request => middleware(sp, request, next))
    { }

    /// <summary>
    /// Gets or sets the condition to evaluate for the middleware.
    /// </summary>
    public Predicate<TRequest>? Condition { get; set; }

    /// <summary>
    /// Gets the type of the middleware.
    /// </summary>
    public Type? MiddlewareType { get; }

    /// <summary>
    /// Gets the asynchronous middleware action.
    /// </summary>
    public AsyncMiddlewareSPFunc<TRequest>? AsyncMiddlewareAction { get; }

    /// <summary>
    /// Gets the middleware action.
    /// </summary>
    public MiddlewareSPFunc<TRequest>? MiddlewareAction { get; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => MiddlewareType is not null || AsyncMiddlewareAction is not null || MiddlewareAction is not null;
}

/// <summary>
/// Represents a descriptor for a termination middleware.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class TerminationMiddlewareDescriptor<TRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminationMiddlewareDescriptor{TRequest}"/> class.
    /// </summary>
    internal TerminationMiddlewareDescriptor()
    {
        Action = _ => { };
        AsyncAction = (_, _) => Task.CompletedTask;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminationMiddlewareDescriptor{TRequest}"/> class with an asynchronous middleware action.
    /// </summary>
    /// <param name="middlewareAction">The asynchronous middleware action.</param>
    public TerminationMiddlewareDescriptor(AsyncMiddlewareDelegate<TRequest> middlewareAction)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareAction);
#else
        if (middlewareAction is null) {
            throw new ArgumentNullException(nameof(middlewareAction));
        }
#endif  
        AsyncAction = middlewareAction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminationMiddlewareDescriptor{TRequest}"/> class with a middleware action.
    /// </summary>
    /// <param name="middlewareAction">The middleware action.</param>
    public TerminationMiddlewareDescriptor(MiddlewareDelegate<TRequest> middlewareAction)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareAction);
#else
        if (middlewareAction is null) {
            throw new ArgumentNullException(nameof(middlewareAction));
        }
#endif  
        Action = middlewareAction;
    }

    /// <summary>
    /// Gets or sets the asynchronous middleware action.
    /// </summary>
    public AsyncMiddlewareDelegate<TRequest>? AsyncAction { get; set; }

    /// <summary>
    /// Gets or sets the middleware action.
    /// </summary>
    public MiddlewareDelegate<TRequest>? Action { get; set; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => AsyncAction is not null || Action is not null;
}

/// <summary>
/// Represents a descriptor for a middleware with a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class MiddlewareDescriptor<TRequest, TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class.
    /// </summary>
    internal MiddlewareDescriptor() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with a middleware type.
    /// </summary>
    /// <param name="middlewareType">The type of the middleware.</param>
    public MiddlewareDescriptor(Type middlewareType)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareType);
#else
        if (middlewareType is null) {
            throw new ArgumentNullException(nameof(middlewareType));
        }
#endif   
        MiddlewareType = middlewareType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The asynchronous middleware function.</param>
    public MiddlewareDescriptor(AsyncMiddlewareSPFunc<TRequest, TResult> middlewareFunc)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareFunc);
#else
        if (middlewareFunc is null) {
            throw new ArgumentNullException(nameof(middlewareFunc));
        }
#endif  
        AsyncMiddlewareFunc = middlewareFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The asynchronous middleware function.</param>
    public MiddlewareDescriptor(AsyncMiddlewareFunc<TRequest, TResult> middlewareFunc)
        : this((_, next) => middlewareFunc(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with an asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public MiddlewareDescriptor(Func<TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        : this((_, next) => (request, ct) => middleware(request, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with an asynchronous middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public MiddlewareDescriptor(Func<IServiceProvider, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        : this((sp, next) => (request, ct) => middleware(sp, request, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with a middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The middleware function.</param>
    public MiddlewareDescriptor(MiddlewareSPFunc<TRequest, TResult> middlewareFunc)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareFunc);
#else
        if (middlewareFunc is null) {
            throw new ArgumentNullException(nameof(middlewareFunc));
        }
#endif  
        MiddlewareFunc = middlewareFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with a middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The middleware function.</param>
    public MiddlewareDescriptor(MiddlewareFunc<TRequest, TResult> middlewareFunc)
        : this((_, next) => middlewareFunc(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with a middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public MiddlewareDescriptor(Func<TRequest, NextMiddlewareDelegate<TResult>, TResult> middleware)
        : this((_, next) => request => middleware(request, next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareDescriptor{TRequest, TResult}"/> class with a middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public MiddlewareDescriptor(Func<IServiceProvider, TRequest, NextMiddlewareDelegate<TResult>, TResult> middleware)
        : this((sp, next) => request => middleware(sp, request, next))
    { }

    /// <summary>
    /// Gets or sets the condition to evaluate for the middleware.
    /// </summary>
    public Predicate<TRequest>? Condition { get; set; }

    /// <summary>
    /// Gets the type of the middleware.
    /// </summary>
    public Type? MiddlewareType { get; }

    /// <summary>
    /// Gets the asynchronous middleware function.
    /// </summary>
    public AsyncMiddlewareSPFunc<TRequest, TResult>? AsyncMiddlewareFunc { get; }

    /// <summary>
    /// Gets the middleware function.
    /// </summary>
    public MiddlewareSPFunc<TRequest, TResult>? MiddlewareFunc { get; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => MiddlewareType is not null || AsyncMiddlewareFunc is not null || MiddlewareFunc is not null;
}

/// <summary>
/// Represents a descriptor for a termination middleware with a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class TerminationMiddlewareDescriptor<TRequest, TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminationMiddlewareDescriptor{TRequest, TResult}"/> class.
    /// </summary>
    internal TerminationMiddlewareDescriptor()
    {
        Func = _ => default!;
        AsyncFunc = (_, _) => Task.FromResult<TResult>(default!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminationMiddlewareDescriptor{TRequest, TResult}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The asynchronous middleware function.</param>
    public TerminationMiddlewareDescriptor(AsyncMiddlewareDelegate<TRequest, TResult> middlewareFunc)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareFunc);
#else
        if (middlewareFunc is null) {
            throw new ArgumentNullException(nameof(middlewareFunc));
        }
#endif  
        AsyncFunc = middlewareFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminationMiddlewareDescriptor{TRequest, TResult}"/> class with a middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The middleware function.</param>
    public TerminationMiddlewareDescriptor(MiddlewareDelegate<TRequest, TResult> middlewareFunc)
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(middlewareFunc);
#else
        if (middlewareFunc is null) {
            throw new ArgumentNullException(nameof(middlewareFunc));
        }
#endif  
        Func = middlewareFunc;
    }

    /// <summary>
    /// Gets or sets the asynchronous middleware function.
    /// </summary>
    public AsyncMiddlewareDelegate<TRequest, TResult>? AsyncFunc { get; set; }

    /// <summary>
    /// Gets or sets the middleware function.
    /// </summary>
    public MiddlewareDelegate<TRequest, TResult>? Func { get; set; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => AsyncFunc is not null || Func is not null;
}