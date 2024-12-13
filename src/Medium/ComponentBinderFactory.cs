namespace Medium;

public class ComponentBinderFactory<TPayload> : IComponentBinderFactory<TPayload>
{
    public virtual IComponentBinder<TPayload> Create() => new ComponentBinder<TPayload>();
}

public class ComponentBinderFactory<TPayload, TResult> : IComponentBinderFactory<TPayload, TResult>
{
    public virtual IComponentBinder<TPayload, TResult> Create() => new ComponentBinder<TPayload, TResult>();
}