namespace Medium;

public interface IComponentBinder<TPayload>
{
    ContextualAsyncMiddlewareDelegate<TPayload> GetAsyncMiddlewareDelegate();
    ContextualMiddlewareDelegate<TPayload> GetMiddlewareDelegate();

    IComponentBinder<TPayload> Init(TerminateComponentDescriptor<TPayload> descriptor);

    IComponentBinder<TPayload> BindComponents(IReadOnlyCollection<ComponentDescriptor<TPayload>> descriptors)
    {
        foreach(var descriptor in descriptors)
            BindToComponent(descriptor);

        return this;
    }

    IComponentBinder<TPayload> BindToComponent(ComponentDescriptor<TPayload> descriptor);
}

public interface IComponentBinder<TPayload, TResult>
{
    ContextualAsyncMiddlewareDelegate<TPayload, TResult> GetAsyncMiddlewareDelegate();
    ContextualMiddlewareDelegate<TPayload, TResult> GetMiddlewareDelegate();

    IComponentBinder<TPayload, TResult> Init(TerminateComponentDescriptor<TPayload, TResult> descriptor);

    IComponentBinder<TPayload, TResult> BindComponents(IReadOnlyCollection<ComponentDescriptor<TPayload, TResult>> descriptors)
    {
        foreach(var descriptor in descriptors)
            BindToComponent(descriptor);

        return this;
    }

    IComponentBinder<TPayload, TResult> BindToComponent(ComponentDescriptor<TPayload, TResult> descriptor);
}