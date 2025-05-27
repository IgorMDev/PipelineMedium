namespace Medium;

/// <summary>
/// Represents an asynchronous middleware component that processes a request and invokes the next middleware in the pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IAsyncMiddleware<TRequest>
{
    /// <summary>
    /// Processes the request asynchronously and invokes the next middleware in the pipeline.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(TRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a middleware component that processes a request and invokes the next middleware in the pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IMiddleware<TRequest>
{
    /// <summary>
    /// Processes the request and invokes the next middleware in the pipeline.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    void Invoke(TRequest request, NextMiddlewareDelegate next);
}

/// <summary>
/// Represents an asynchronous middleware component that processes a request and invokes the next middleware in the pipeline, returning a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IAsyncMiddleware<TRequest, TResult>
{
    /// <summary>
    /// Processes the request asynchronously and invokes the next middleware in the pipeline, returning a result.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> InvokeAsync(TRequest request, NextAsyncMiddlewareDelegate<TResult> next, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a middleware component that processes a request and invokes the next middleware in the pipeline, returning a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMiddleware<TRequest, TResult>
{
    /// <summary>
    /// Processes the request and invokes the next middleware in the pipeline, returning a result.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Invoke(TRequest request, NextMiddlewareDelegate<TResult> next);
}