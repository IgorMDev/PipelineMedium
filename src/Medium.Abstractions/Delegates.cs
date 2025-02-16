namespace Medium;

/// <summary>
/// Represents the delegate for invoking the next middleware in the pipeline.
/// </summary>
public delegate void NextMiddlewareDelegate();
/// <summary>
/// Represents the delegate for invoking the next asynchronous middleware in the pipeline.
/// </summary>
public delegate Task NextAsyncMiddlewareDelegate();

/// <summary>
/// Represents the delegate for a middleware component that processes a payload.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <param name="payload">The payload to be processed.</param>
public delegate void MiddlewareDelegate<in TPayload>(TPayload payload);
/// <summary>
/// Represents the delegate for an asynchronous middleware component that processes a payload.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <param name="payload">The payload to be processed.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate Task AsyncMiddlewareDelegate<in TPayload>(TPayload payload, CancellationToken cancellationToken = default);

/// <summary>
/// Represents the delegate for a middleware function that returns a middleware delegate.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the payload.</returns>
public delegate MiddlewareDelegate<TPayload> MiddlewareFunc<TPayload>(NextMiddlewareDelegate next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that returns an asynchronous middleware delegate.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the payload.</returns>
public delegate AsyncMiddlewareDelegate<TPayload> AsyncMiddlewareFunc<TPayload>(NextAsyncMiddlewareDelegate next);

/// <summary>
/// Represents the delegate for a middleware function that uses a service provider and returns a middleware delegate.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the payload.</returns>
public delegate MiddlewareDelegate<TPayload> MiddlewareSPFunc<TPayload>(IServiceProvider serviceProvider, NextMiddlewareDelegate next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that uses a service provider and returns an asynchronous middleware delegate.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the payload.</returns>
public delegate AsyncMiddlewareDelegate<TPayload> AsyncMiddlewareSPFunc<TPayload>(IServiceProvider serviceProvider, NextAsyncMiddlewareDelegate next);


/// <summary>
/// Represents the delegate for invoking the next middleware in the pipeline and returning a result.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
public delegate TResult NextMiddlewareDelegate<TResult>();
/// <summary>
/// Represents the delegate for invoking the next asynchronous middleware in the pipeline and returning a result.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
public delegate Task<TResult> NextAsyncMiddlewareDelegate<TResult>();

/// <summary>
/// Represents the delegate for a middleware component that processes a payload and returns a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="payload">The payload to be processed.</param>
/// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
public delegate TResult MiddlewareDelegate<in TPayload, TResult>(TPayload payload);
/// <summary>
/// Represents the delegate for an asynchronous middleware component that processes a payload and returns a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="payload">The payload to be processed.</param>
/// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
public delegate Task<TResult> AsyncMiddlewareDelegate<in TPayload, TResult>(TPayload payload, CancellationToken cancellationToken = default);

/// <summary>
/// Represents the delegate for a middleware function that returns a middleware delegate with a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the payload and returns a result.</returns>
public delegate MiddlewareDelegate<TPayload, TResult> MiddlewareFunc<TPayload, TResult>(NextMiddlewareDelegate<TResult> next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that returns an asynchronous middleware delegate with a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the payload and returns a result.</returns>
public delegate AsyncMiddlewareDelegate<TPayload, TResult> AsyncMiddlewareFunc<TPayload, TResult>(NextAsyncMiddlewareDelegate<TResult> next);

/// <summary>
/// Represents the delegate for a middleware function that uses a service provider and returns a middleware delegate with a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the payload and returns a result.</returns>
public delegate MiddlewareDelegate<TPayload, TResult> MiddlewareSPFunc<TPayload, TResult>(IServiceProvider serviceProvider, NextMiddlewareDelegate<TResult> next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that uses a service provider and returns an asynchronous middleware delegate with a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the payload and returns a result.</returns>
public delegate AsyncMiddlewareDelegate<TPayload, TResult> AsyncMiddlewareSPFunc<TPayload, TResult>(IServiceProvider serviceProvider, NextAsyncMiddlewareDelegate<TResult> next);