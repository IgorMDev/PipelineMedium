namespace Medium;

/// <summary>
/// Represents a descriptor for a middleware component.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public class ComponentDescriptor<TPayload>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class.
    /// </summary>
    internal ComponentDescriptor() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with a middleware type.
    /// </summary>
    /// <param name="middlewareType">The type of the middleware.</param>
    public ComponentDescriptor(Type middlewareType)
    {
        ArgumentNullException.ThrowIfNull(middlewareType);
        MiddlewareType = middlewareType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with an asynchronous middleware action.
    /// </summary>
    /// <param name="middlewareAction">The asynchronous middleware action.</param>
    public ComponentDescriptor(AsyncMiddlewareSPFunc<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        AsyncMiddlewareAction = middlewareAction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    public ComponentDescriptor(AsyncMiddlewareFunc<TPayload> middleware)
        : this((_, next) => middleware(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with an asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public ComponentDescriptor(Func<TPayload, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        : this((_, next) => (payload, ct) => middleware(payload, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with an asynchronous middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public ComponentDescriptor(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        : this((sp, next) => (payload, ct) => middleware(sp, payload, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with a middleware action.
    /// </summary>
    /// <param name="middlewareAction">The middleware action.</param>
    public ComponentDescriptor(MiddlewareSPFunc<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        MiddlewareAction = middlewareAction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with a middleware function.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    public ComponentDescriptor(MiddlewareFunc<TPayload> middleware)
        : this((_, next) => middleware(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with a middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public ComponentDescriptor(Action<TPayload, NextMiddlewareDelegate> middleware)
        : this((_, next) => payload => middleware(payload, next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload}"/> class with a middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public ComponentDescriptor(Action<IServiceProvider, TPayload, NextMiddlewareDelegate> middleware)
        : this((sp, next) => payload => middleware(sp, payload, next))
    { }

    /// <summary>
    /// Gets or sets the condition to evaluate for the middleware.
    /// </summary>
    public Predicate<TPayload>? Condition { get; set; }

    /// <summary>
    /// Gets the type of the middleware.
    /// </summary>
    public Type? MiddlewareType { get; }

    /// <summary>
    /// Gets the asynchronous middleware action.
    /// </summary>
    public AsyncMiddlewareSPFunc<TPayload>? AsyncMiddlewareAction { get; }

    /// <summary>
    /// Gets the middleware action.
    /// </summary>
    public MiddlewareSPFunc<TPayload>? MiddlewareAction { get; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => MiddlewareType is not null || AsyncMiddlewareAction is not null || MiddlewareAction is not null;
}

/// <summary>
/// Represents a descriptor for a terminate component.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public class TerminateComponentDescriptor<TPayload>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminateComponentDescriptor{TPayload}"/> class.
    /// </summary>
    internal TerminateComponentDescriptor()
    {
        Action = _ => { };
        AsyncAction = (_, _) => Task.CompletedTask;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminateComponentDescriptor{TPayload}"/> class with an asynchronous middleware action.
    /// </summary>
    /// <param name="middlewareAction">The asynchronous middleware action.</param>
    public TerminateComponentDescriptor(AsyncMiddlewareDelegate<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        AsyncAction = middlewareAction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminateComponentDescriptor{TPayload}"/> class with a middleware action.
    /// </summary>
    /// <param name="middlewareAction">The middleware action.</param>
    public TerminateComponentDescriptor(MiddlewareDelegate<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        Action = middlewareAction;
    }

    /// <summary>
    /// Gets or sets the asynchronous middleware action.
    /// </summary>
    public AsyncMiddlewareDelegate<TPayload>? AsyncAction { get; set; }

    /// <summary>
    /// Gets or sets the middleware action.
    /// </summary>
    public MiddlewareDelegate<TPayload>? Action { get; set; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => AsyncAction is not null || Action is not null;
}

/// <summary>
/// Represents a descriptor for a middleware component with a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class ComponentDescriptor<TPayload, TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class.
    /// </summary>
    internal ComponentDescriptor() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with a middleware type.
    /// </summary>
    /// <param name="middlewareType">The type of the middleware.</param>
    public ComponentDescriptor(Type middlewareType)
    {
        ArgumentNullException.ThrowIfNull(middlewareType);
        MiddlewareType = middlewareType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The asynchronous middleware function.</param>
    public ComponentDescriptor(AsyncMiddlewareSPFunc<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        AsyncMiddlewareFunc = middlewareFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The asynchronous middleware function.</param>
    public ComponentDescriptor(AsyncMiddlewareFunc<TPayload, TResult> middlewareFunc)
        : this((_, next) => middlewareFunc(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with an asynchronous middleware delegate.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public ComponentDescriptor(Func<TPayload, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        : this((_, next) => (payload, ct) => middleware(payload, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with an asynchronous middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    public ComponentDescriptor(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        : this((sp, next) => (payload, ct) => middleware(sp, payload, next, ct))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with a middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The middleware function.</param>
    public ComponentDescriptor(MiddlewareSPFunc<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        MiddlewareFunc = middlewareFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with a middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The middleware function.</param>
    public ComponentDescriptor(MiddlewareFunc<TPayload, TResult> middlewareFunc)
        : this((_, next) => middlewareFunc(next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with a middleware delegate.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public ComponentDescriptor(Func<TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        : this((_, next) => payload => middleware(payload, next))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentDescriptor{TPayload, TResult}"/> class with a middleware delegate that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    public ComponentDescriptor(Func<IServiceProvider, TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        : this((sp, next) => payload => middleware(sp, payload, next))
    { }

    /// <summary>
    /// Gets or sets the condition to evaluate for the middleware.
    /// </summary>
    public Predicate<TPayload>? Condition { get; set; }

    /// <summary>
    /// Gets the type of the middleware.
    /// </summary>
    public Type? MiddlewareType { get; }

    /// <summary>
    /// Gets the asynchronous middleware function.
    /// </summary>
    public AsyncMiddlewareSPFunc<TPayload, TResult>? AsyncMiddlewareFunc { get; }

    /// <summary>
    /// Gets the middleware function.
    /// </summary>
    public MiddlewareSPFunc<TPayload, TResult>? MiddlewareFunc { get; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => MiddlewareType is not null || AsyncMiddlewareFunc is not null || MiddlewareFunc is not null;
}

/// <summary>
/// Represents a descriptor for a terminate component with a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class TerminateComponentDescriptor<TPayload, TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TerminateComponentDescriptor{TPayload, TResult}"/> class.
    /// </summary>
    internal TerminateComponentDescriptor()
    {
        Func = _ => default!;
        AsyncFunc = (_, _) => Task.FromResult<TResult>(default!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminateComponentDescriptor{TPayload, TResult}"/> class with an asynchronous middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The asynchronous middleware function.</param>
    public TerminateComponentDescriptor(AsyncMiddlewareDelegate<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        AsyncFunc = middlewareFunc;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminateComponentDescriptor{TPayload, TResult}"/> class with a middleware function.
    /// </summary>
    /// <param name="middlewareFunc">The middleware function.</param>
    public TerminateComponentDescriptor(MiddlewareDelegate<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        Func = middlewareFunc;
    }

    /// <summary>
    /// Gets or sets the asynchronous middleware function.
    /// </summary>
    public AsyncMiddlewareDelegate<TPayload, TResult>? AsyncFunc { get; set; }

    /// <summary>
    /// Gets or sets the middleware function.
    /// </summary>
    public MiddlewareDelegate<TPayload, TResult>? Func { get; set; }

    /// <summary>
    /// Gets a value indicating whether the descriptor is valid.
    /// </summary>
    public bool IsValid => AsyncFunc is not null || Func is not null;
}