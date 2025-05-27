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
/// Represents the delegate for a middleware component that processes a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <param name="request">The request to be processed.</param>
public delegate void MiddlewareDelegate<in TRequest>(TRequest request);
/// <summary>
/// Represents the delegate for an asynchronous middleware component that processes a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <param name="request">The request to be processed.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public delegate Task AsyncMiddlewareDelegate<in TRequest>(TRequest request, CancellationToken cancellationToken = default);

/// <summary>
/// Represents the delegate for a middleware function that returns a middleware delegate.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the request.</returns>
public delegate MiddlewareDelegate<TRequest> MiddlewareFunc<TRequest>(NextMiddlewareDelegate next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that returns an asynchronous middleware delegate.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the request.</returns>
public delegate AsyncMiddlewareDelegate<TRequest> AsyncMiddlewareFunc<TRequest>(NextAsyncMiddlewareDelegate next);

/// <summary>
/// Represents the delegate for a middleware function that uses a service provider and returns a middleware delegate.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the request.</returns>
public delegate MiddlewareDelegate<TRequest> MiddlewareSPFunc<TRequest>(IServiceProvider serviceProvider, NextMiddlewareDelegate next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that uses a service provider and returns an asynchronous middleware delegate.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the request.</returns>
public delegate AsyncMiddlewareDelegate<TRequest> AsyncMiddlewareSPFunc<TRequest>(IServiceProvider serviceProvider, NextAsyncMiddlewareDelegate next);


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
/// Represents the delegate for a middleware component that processes a request and returns a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="request">The request to be processed.</param>
/// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
public delegate TResult MiddlewareDelegate<in TRequest, TResult>(TRequest request);
/// <summary>
/// Represents the delegate for an asynchronous middleware component that processes a request and returns a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="request">The request to be processed.</param>
/// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
public delegate Task<TResult> AsyncMiddlewareDelegate<in TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default);

/// <summary>
/// Represents the delegate for a middleware function that returns a middleware delegate with a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the request and returns a result.</returns>
public delegate MiddlewareDelegate<TRequest, TResult> MiddlewareFunc<TRequest, TResult>(NextMiddlewareDelegate<TResult> next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that returns an asynchronous middleware delegate with a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the request and returns a result.</returns>
public delegate AsyncMiddlewareDelegate<TRequest, TResult> AsyncMiddlewareFunc<TRequest, TResult>(NextAsyncMiddlewareDelegate<TResult> next);

/// <summary>
/// Represents the delegate for a middleware function that uses a service provider and returns a middleware delegate with a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next middleware in the pipeline.</param>
/// <returns>A middleware delegate that processes the request and returns a result.</returns>
public delegate MiddlewareDelegate<TRequest, TResult> MiddlewareSPFunc<TRequest, TResult>(IServiceProvider serviceProvider, NextMiddlewareDelegate<TResult> next);
/// <summary>
/// Represents the delegate for an asynchronous middleware function that uses a service provider and returns an asynchronous middleware delegate with a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="next">The delegate representing the next asynchronous middleware in the pipeline.</param>
/// <returns>An asynchronous middleware delegate that processes the request and returns a result.</returns>
public delegate AsyncMiddlewareDelegate<TRequest, TResult> AsyncMiddlewareSPFunc<TRequest, TResult>(IServiceProvider serviceProvider, NextAsyncMiddlewareDelegate<TResult> next);