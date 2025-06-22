namespace Medium;

/// <summary>
/// Represents the options for configuring a Medium with a specific request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class MediumOptions<TRequest>
{
    /// <summary>
    /// Gets or sets the list of middleware descriptors for the Medium.
    /// </summary>
    public List<MiddlewareDescriptor<TRequest>> Middlewares { get; set; } = [];

    /// <summary>
    /// Gets or sets the termination middleware descriptor for the Medium.
    /// </summary>
    public TerminationMiddlewareDescriptor<TRequest> TerminationMiddleware { get; set; } = new();
}

/// <summary>
/// Represents the options for configuring a Medium with a specific request and result type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class MediumOptions<TRequest, TResult>
{
    /// <summary>
    /// Gets or sets the list of middleware descriptors for the Medium.
    /// </summary>
    public List<MiddlewareDescriptor<TRequest, TResult>> Middlewares { get; set; } = [];

    /// <summary>
    /// Gets or sets the termination middleware descriptor for the Medium.
    /// </summary>
    public TerminationMiddlewareDescriptor<TRequest, TResult> TerminationMiddleware { get; set; } = new();
}