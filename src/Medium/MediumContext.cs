namespace Medium;

public class MediumContext<TRequest>(IServiceProvider serviceProvider, TRequest request)
{
    internal IServiceProvider ServiceProvider { get; } = serviceProvider;
    internal CancellationToken CancellationToken { get; set; }

    public TRequest Request { get; } = request;
}

public class MediumContext<TRequest, TResult>(IServiceProvider serviceProvider, TRequest request) 
    : MediumContext<TRequest>(serviceProvider, request)
{
}