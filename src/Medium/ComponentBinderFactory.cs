namespace Medium;

/// <summary>
/// Represents a factory for creating component binders that process a payload.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public class ComponentBinderFactory<TPayload> : IComponentBinderFactory<TPayload>
{
    /// <summary>
    /// Creates a new instance of a component binder.
    /// </summary>
    /// <returns>A new instance of a component binder.</returns>
    public virtual IComponentBinder<TPayload> Create() => new ComponentBinder<TPayload>();
}

/// <summary>
/// Represents a factory for creating component binders that process a payload and return a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class ComponentBinderFactory<TPayload, TResult> : IComponentBinderFactory<TPayload, TResult>
{
    /// <summary>
    /// Creates a new instance of a component binder.
    /// </summary>
    /// <returns>A new instance of a component binder.</returns>
    public virtual IComponentBinder<TPayload, TResult> Create() => new ComponentBinder<TPayload, TResult>();
}