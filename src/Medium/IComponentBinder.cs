namespace Medium;

/// <summary>
/// Represents a binder for components that process a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public interface IComponentBinder<TRequest>
{
    /// <summary>
    /// Gets the asynchronous middleware delegate for the component.
    /// </summary>
    /// <returns>The asynchronous middleware delegate.</returns>
    ContextualAsyncMiddlewareDelegate<TRequest> GetAsyncMiddlewareDelegate();

    /// <summary>
    /// Gets the middleware delegate for the component.
    /// </summary>
    /// <returns>The middleware delegate.</returns>
    ContextualMiddlewareDelegate<TRequest> GetMiddlewareDelegate();

    /// <summary>
    /// Initializes the binder with a terminate component descriptor.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TRequest}"/> instance.</returns>
    IComponentBinder<TRequest> Init(TerminateComponentDescriptor<TRequest> descriptor);

#if NET5_0_OR_GREATER
    /// <summary>
    /// Binds the components to the binder using the specified descriptors.
    /// </summary>
    /// <param name="descriptors">The component descriptors.</param>
    /// <returns>The current <see cref="IComponentBinder{TRequest}"/> instance.</returns>
    IComponentBinder<TRequest> BindComponents(IReadOnlyCollection<ComponentDescriptor<TRequest>> descriptors)
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
    /// <returns>The current <see cref="IComponentBinder{TRequest}"/> instance.</returns>
    IComponentBinder<TRequest> BindComponents(IReadOnlyCollection<ComponentDescriptor<TRequest>> descriptors);
#endif

    /// <summary>
    /// Binds a single component to the binder using the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TRequest}"/> instance.</returns>
    IComponentBinder<TRequest> BindToComponent(ComponentDescriptor<TRequest> descriptor);
}

/// <summary>
/// Represents a binder for components that process a request and return a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IComponentBinder<TRequest, TResult>
{
    /// <summary>
    /// Gets the asynchronous middleware delegate for the component.
    /// </summary>
    /// <returns>The asynchronous middleware delegate.</returns>
    ContextualAsyncMiddlewareDelegate<TRequest, TResult> GetAsyncMiddlewareDelegate();

    /// <summary>
    /// Gets the middleware delegate for the component.
    /// </summary>
    /// <returns>The middleware delegate.</returns>
    ContextualMiddlewareDelegate<TRequest, TResult> GetMiddlewareDelegate();

    /// <summary>
    /// Initializes the binder with a terminate component descriptor.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TRequest, TResult}"/> instance.</returns>
    IComponentBinder<TRequest, TResult> Init(TerminateComponentDescriptor<TRequest, TResult> descriptor);

#if NET5_0_OR_GREATER
    /// <summary>
    /// Binds the components to the binder using the specified descriptors.
    /// </summary>
    /// <param name="descriptors">The component descriptors.</param>
    /// <returns>The current <see cref="IComponentBinder{TRequest, TResult}"/> instance.</returns>
    IComponentBinder<TRequest, TResult> BindComponents(IReadOnlyCollection<ComponentDescriptor<TRequest, TResult>> descriptors)
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
    /// <returns>The current <see cref="IComponentBinder{TRequest, TResult}"/> instance.</returns>
    IComponentBinder<TRequest, TResult> BindComponents(IReadOnlyCollection<ComponentDescriptor<TRequest, TResult>> descriptors);
#endif

    /// <summary>
    /// Binds a single component to the binder using the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="IComponentBinder{TRequest, TResult}"/> instance.</returns>
    IComponentBinder<TRequest, TResult> BindToComponent(ComponentDescriptor<TRequest, TResult> descriptor);
}