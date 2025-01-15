namespace Medium;

/// <summary>
/// Represents a factory for creating component binders that process a payload.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public interface IComponentBinderFactory<TPayload>
{
    /// <summary>
    /// Creates a new instance of a component binder.
    /// </summary>
    /// <returns>A new instance of a component binder.</returns>
    IComponentBinder<TPayload> Create();
}

/// <summary>
/// Represents a factory for creating component binders that process a payload and return a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IComponentBinderFactory<TPayload, TResult>
{
    /// <summary>
    /// Creates a new instance of a component binder.
    /// </summary>
    /// <returns>A new instance of a component binder.</returns>
    IComponentBinder<TPayload, TResult> Create();
}