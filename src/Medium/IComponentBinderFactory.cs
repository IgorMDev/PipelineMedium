namespace Medium;

/// <summary>
/// Represents a factory for creating component binders that process a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IComponentBinderFactory<TRequest>
{
    /// <summary>
    /// Creates a new instance of a component binder.
    /// </summary>
    /// <returns>A new instance of a component binder.</returns>
    IComponentBinder<TRequest> Create();
}

/// <summary>
/// Represents a factory for creating component binders that process a request and return a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IComponentBinderFactory<TRequest, TResult>
{
    /// <summary>
    /// Creates a new instance of a component binder.
    /// </summary>
    /// <returns>A new instance of a component binder.</returns>
    IComponentBinder<TRequest, TResult> Create();
}