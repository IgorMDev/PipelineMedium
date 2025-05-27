namespace Medium;

/// <summary>
/// Represents a medium for executing operations with a provided request and/or result types.
/// </summary>
public interface IMedium
{
    /// <summary>
    /// Executes an asynchronous operation with a specified name and request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync<TRequest>(string name, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    void Execute<TRequest>(string name, TRequest request);

    /// <summary>
    /// Executes an operation with a request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="request">The request to be processed.</param>
    void Execute<TRequest>(TRequest request);

    /// <summary>
    /// Executes an asynchronous operation with a specified name and request, and returns a result.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync<TRequest, TResult>(string name, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a request, and returns a result.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and request, and returns a result.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute<TRequest, TResult>(string name, TRequest request);

    /// <summary>
    /// Executes an operation with a request, and returns a result.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="request">The request to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute<TRequest, TResult>(TRequest request);
}

/// <summary>
/// Represents a medium for executing operations with a specific request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IMedium<TRequest>
{
    /// <summary>
    /// Executes an asynchronous operation with a specified name and request.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(string name, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a request.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and request.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    void Execute(string name, TRequest request);

    /// <summary>
    /// Executes an operation with a request.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    void Execute(TRequest request);
}

/// <summary>
/// Represents a medium for executing operations with a specific request type and returning a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMedium<TRequest, TResult>
{
    /// <summary>
    /// Executes an asynchronous operation with a specified name and request, and returns a result.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync(string name, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a request, and returns a result.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and request, and returns a result.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="request">The request to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute(string name, TRequest request);

    /// <summary>
    /// Executes an operation with a request, and returns a result.
    /// </summary>
    /// <param name="request">The request to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute(TRequest request);
}