namespace Medium;

/// <summary>
/// Represents a binder for components that process a payload.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public interface IComponentBinder<TPayload>
{
    /// <summary>
    /// Gets the asynchronous middleware delegate for the component.
    /// </summary>
    /// <returns>The asynchronous middleware delegate.</returns>
    ContextualAsyncMiddlewareDelegate<TPayload> GetAsyncMiddlewareDelegate();

    /// <summary>
    /// Gets the middleware delegate for the component.
    /// </summary>
    /// <returns>The middleware delegate.</returns>
    ContextualMiddlewareDelegate<TPayload> GetMiddlewareDelegate();

    /// <summary>
    /// Initializes the binder with a terminate component descriptor.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload}"/> instance.</returns>
    IComponentBinder<TPayload> Init(TerminateComponentDescriptor<TPayload> descriptor);

#if NET5_0_OR_GREATER
    /// <summary>
    /// Binds the components to the binder using the specified descriptors.
    /// </summary>
    /// <param name="descriptors">The component descriptors.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload}"/> instance.</returns>
    IComponentBinder<TPayload> BindComponents(IReadOnlyCollection<ComponentDescriptor<TPayload>> descriptors)
    {
        foreach(var descriptor in descriptors)
            BindToComponent(descriptor);

        return this;
    }
#else
    /// <summary>
    /// Binds the components to the binder using the specified descriptors.
    /// </summary>
    /// <param name="descriptors">The component descriptors.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload}"/> instance.</returns>
    IComponentBinder<TPayload> BindComponents(IReadOnlyCollection<ComponentDescriptor<TPayload>> descriptors);
#endif

    /// <summary>
    /// Binds a single component to the binder using the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload}"/> instance.</returns>
    IComponentBinder<TPayload> BindToComponent(ComponentDescriptor<TPayload> descriptor);
}

/// <summary>
/// Represents a binder for components that process a payload and return a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IComponentBinder<TPayload, TResult>
{
    /// <summary>
    /// Gets the asynchronous middleware delegate for the component.
    /// </summary>
    /// <returns>The asynchronous middleware delegate.</returns>
    ContextualAsyncMiddlewareDelegate<TPayload, TResult> GetAsyncMiddlewareDelegate();

    /// <summary>
    /// Gets the middleware delegate for the component.
    /// </summary>
    /// <returns>The middleware delegate.</returns>
    ContextualMiddlewareDelegate<TPayload, TResult> GetMiddlewareDelegate();

    /// <summary>
    /// Initializes the binder with a terminate component descriptor.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload, TResult}"/> instance.</returns>
    IComponentBinder<TPayload, TResult> Init(TerminateComponentDescriptor<TPayload, TResult> descriptor);

#if NET5_0_OR_GREATER
    /// <summary>
    /// Binds the components to the binder using the specified descriptors.
    /// </summary>
    /// <param name="descriptors">The component descriptors.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload, TResult}"/> instance.</returns>
    IComponentBinder<TPayload, TResult> BindComponents(IReadOnlyCollection<ComponentDescriptor<TPayload, TResult>> descriptors)
    {
        foreach(var descriptor in descriptors)
            BindToComponent(descriptor);

        return this;
    }
#else
    /// <summary>
    /// Binds the components to the binder using the specified descriptors.
    /// </summary>
    /// <param name="descriptors">The component descriptors.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload, TResult}"/> instance.</returns>
    IComponentBinder<TPayload, TResult> BindComponents(IReadOnlyCollection<ComponentDescriptor<TPayload, TResult>> descriptors);
#endif

    /// <summary>
    /// Binds a single component to the binder using the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TPayload, TResult}"/> instance.</returns>
    IComponentBinder<TPayload, TResult> BindToComponent(ComponentDescriptor<TPayload, TResult> descriptor);
}