namespace Medium;

/// <summary>
/// Represents the options for configuring a Medium with a specific payload type.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public class MediumOptions<TPayload>
{
    /// <summary>
    /// Gets or sets the list of component descriptors for the Medium.
    /// </summary>
    public List<ComponentDescriptor<TPayload>> Components { get; set; } = [];

    /// <summary>
    /// Gets or sets the terminate component descriptor for the Medium.
    /// </summary>
    public TerminateComponentDescriptor<TPayload> TerminateComponent { get; set; } = new();
}

/// <summary>
/// Represents the options for configuring a Medium with a specific payload and result type.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class MediumOptions<TPayload, TResult>
{
    /// <summary>
    /// Gets or sets the list of component descriptors for the Medium.
    /// </summary>
    public List<ComponentDescriptor<TPayload, TResult>> Components { get; set; } = [];

    /// <summary>
    /// Gets or sets the terminate component descriptor for the Medium.
    /// </summary>
    public TerminateComponentDescriptor<TPayload, TResult> TerminateComponent { get; set; } = new();
}