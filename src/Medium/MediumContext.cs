namespace Medium;

public class MediumContext<TPayload>(IServiceProvider serviceProvider, TPayload payload)
{
    internal IServiceProvider ServiceProvider { get; } = serviceProvider;
    internal CancellationToken CancellationToken { get; set; }

    public TPayload Payload { get; } = payload;
}

public class MediumContext<TPayload, TResult>(IServiceProvider serviceProvider, TPayload payload) 
    : MediumContext<TPayload>(serviceProvider, payload)
{
}