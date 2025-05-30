﻿namespace Medium;

/// <summary>
/// Represents the options for configuring a Medium with a specific request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class MediumOptions<TRequest>
{
    /// <summary>
    /// Gets or sets the list of component descriptors for the Medium.
    /// </summary>
    public List<ComponentDescriptor<TRequest>> Components { get; set; } = [];

    /// <summary>
    /// Gets or sets the terminate component descriptor for the Medium.
    /// </summary>
    public TerminateComponentDescriptor<TRequest> TerminateComponent { get; set; } = new();
}

/// <summary>
/// Represents the options for configuring a Medium with a specific request and result type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class MediumOptions<TRequest, TResult>
{
    /// <summary>
    /// Gets or sets the list of component descriptors for the Medium.
    /// </summary>
    public List<ComponentDescriptor<TRequest, TResult>> Components { get; set; } = [];

    /// <summary>
    /// Gets or sets the terminate component descriptor for the Medium.
    /// </summary>
    public TerminateComponentDescriptor<TRequest, TResult> TerminateComponent { get; set; } = new();
}