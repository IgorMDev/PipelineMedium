namespace Medium;

public class MediumOptions<TPayload>
{
    public List<ComponentDescriptor<TPayload>> Components { get; set; } = [];
    public TerminateComponentDescriptor<TPayload> TerminateComponent { get; set; } = new();
}

public class MediumOptions<TPayload, TResult>
{
    public List<ComponentDescriptor<TPayload, TResult>> Components { get; set; } = [];
    public TerminateComponentDescriptor<TPayload, TResult> TerminateComponent { get; set; } = new();
}