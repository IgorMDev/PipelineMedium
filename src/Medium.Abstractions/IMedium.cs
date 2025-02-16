namespace Medium;

/// <summary>
/// Represents a medium for executing operations with a provided payload and/or result types.
/// </summary>
public interface IMedium
{
    /// <summary>
    /// Executes an asynchronous operation with a specified name and payload.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync<TPayload>(string name, TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a payload.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync<TPayload>(TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and payload.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    void Execute<TPayload>(string name, TPayload payload);

    /// <summary>
    /// Executes an operation with a payload.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="payload">The payload to be processed.</param>
    void Execute<TPayload>(TPayload payload);

    /// <summary>
    /// Executes an asynchronous operation with a specified name and payload, and returns a result.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync<TPayload, TResult>(string name, TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a payload, and returns a result.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync<TPayload, TResult>(TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and payload, and returns a result.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute<TPayload, TResult>(string name, TPayload payload);

    /// <summary>
    /// Executes an operation with a payload, and returns a result.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute<TPayload, TResult>(TPayload payload);
}

/// <summary>
/// Represents a medium for executing operations with a specific payload type.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public interface IMedium<TPayload>
{
    /// <summary>
    /// Executes an asynchronous operation with a specified name and payload.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(string name, TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a payload.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and payload.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    void Execute(string name, TPayload payload);

    /// <summary>
    /// Executes an operation with a payload.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    void Execute(TPayload payload);
}

/// <summary>
/// Represents a medium for executing operations with a specific payload type and returning a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IMedium<TPayload, TResult>
{
    /// <summary>
    /// Executes an asynchronous operation with a specified name and payload, and returns a result.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync(string name, TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation with a payload, and returns a result.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of type <typeparamref name="TResult"/>.</returns>
    Task<TResult> ExecuteAsync(TPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation with a specified name and payload, and returns a result.
    /// </summary>
    /// <param name="name">The name of the operation.</param>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute(string name, TPayload payload);

    /// <summary>
    /// Executes an operation with a payload, and returns a result.
    /// </summary>
    /// <param name="payload">The payload to be processed.</param>
    /// <returns>The result of the operation of type <typeparamref name="TResult"/>.</returns>
    TResult Execute(TPayload payload);
}