namespace Medium;

public interface IComponentBinderFactory<TPayload>
{
    IComponentBinder<TPayload> Create();
}

public interface IComponentBinderFactory<TPayload, TResult>
{
    IComponentBinder<TPayload, TResult> Create();
}