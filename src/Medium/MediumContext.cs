namespace Medium;

public class MediumContext<TPayload>(IServiceProvider serviceProvider, TPayload payload)
{
    internal IServiceProvider ServiceProvider { get; init; } = serviceProvider;
    public TPayload Payload { get; set; } = payload;
}

public class MediumContext<TPayload, TResult>(IServiceProvider serviceProvider, TPayload payload) 
    : MediumContext<TPayload>(serviceProvider, payload)
{
}