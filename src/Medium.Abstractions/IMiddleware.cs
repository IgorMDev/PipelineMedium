namespace Medium;

/// <summary>
/// Represents an asynchronous middleware component that processes a payload and invokes the next middleware in the pipeline.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public interface IAsyncMiddleware<TPayload>
{
    /// <summary>
    /// Processes the payload asynchronously and invokes the next middleware in the pipeline.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(TPayload payload, NextAsyncMiddlewareDelegate next);
}

/// <summary>
/// Represents a middleware component that processes a payload and invokes the next middleware in the pipeline.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public interface IMiddleware<TPayload>
{
    /// <summary>
    /// Processes the payload and invokes the next middleware in the pipeline.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    void Invoke(TPayload payload, NextMiddlewareDelegate next);
}

/// <summary>
/// Represents an asynchronous middleware component that processes a payload and invokes the next middleware in the pipeline, returning a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IAsyncMiddleware<TPayload, TResult>
{
    /// <summary>
    /// Processes the payload asynchronously and invokes the next middleware in the pipeline, returning a result.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> InvokeAsync(TPayload payload, NextAsyncMiddlewareDelegate<TResult> next);
}

/// <summary>
/// Represents a middleware component that processes a payload and invokes the next middleware in the pipeline, returning a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMiddleware<TPayload, TResult>
{
    /// <summary>
    /// Processes the payload and invokes the next middleware in the pipeline, returning a result.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    /// <param name="next">The delegate representing the next middleware in the pipeline.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Invoke(TPayload payload, NextMiddlewareDelegate<TResult> next);
}