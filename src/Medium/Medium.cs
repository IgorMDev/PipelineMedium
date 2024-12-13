using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Medium;

public class Medium(IServiceProvider serviceProvider) : IMedium
{
    internal const string DefaultName = "Default";

    public Task ExecuteAsync<TPayload>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().ExecuteAsync(name, payload);
    public Task ExecuteAsync<TPayload>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().ExecuteAsync(payload);

    public void Execute<TPayload>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().Execute(name, payload);
    public void Execute<TPayload>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().Execute(payload);

    public Task<TResult> ExecuteAsync<TPayload, TResult>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().ExecuteAsync(name, payload);
    public Task<TResult> ExecuteAsync<TPayload, TResult>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().ExecuteAsync(payload);

    public TResult Execute<TPayload, TResult>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().Execute(name, payload);
    public TResult Execute<TPayload, TResult>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().Execute(payload);
}

public class Medium<TPayload>(
    IServiceProvider serviceProvider,
    IOptionsMonitor<MediumOptions<TPayload>> optionsMonitor,
    IComponentBinderFactory<TPayload> componentBinderFactory
) : IMedium<TPayload>
{
    private readonly Dictionary<string, ContextualAsyncMiddlewareDelegate<TPayload>> _asyncMiddlewareDelegates = [];
    private readonly Dictionary<string, ContextualMiddlewareDelegate<TPayload>> _middlewareDelegates = [];

    private IComponentBinder<TPayload> GetBinder(string name)
    {
        var options = optionsMonitor.Get(name);
        var binder = componentBinderFactory.Create()
            .Init(options.TerminateComponent)
            .BindComponents(options.Components);

        return binder;
    }

    public Task ExecuteAsync(string name, TPayload payload)
    {
        if(!_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _asyncMiddlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetAsyncMiddlewareDelegate();

        MediumContext<TPayload> context = new(serviceProvider, payload);
        return middlewareDelegate(context);
    }
    public Task ExecuteAsync(TPayload payload) => ExecuteAsync(Medium.DefaultName, payload);

    public void Execute(string name, TPayload payload)
    {
        if(!_middlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _middlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetMiddlewareDelegate();

        MediumContext<TPayload> context = new(serviceProvider, payload);
        middlewareDelegate(context);
    }
    public void Execute(TPayload payload) => Execute(Medium.DefaultName, payload);
}

public class Medium<TPayload, TResult>(
    IServiceProvider serviceProvider,
    IOptionsMonitor<MediumOptions<TPayload, TResult>> optionsMonitor,
    IComponentBinderFactory<TPayload, TResult> componentBinderFactory
) : IMedium<TPayload, TResult>
{
    private readonly Dictionary<string, ContextualAsyncMiddlewareDelegate<TPayload, TResult>> _asyncMiddlewareDelegates = [];
    private readonly Dictionary<string, ContextualMiddlewareDelegate<TPayload, TResult>> _middlewareDelegates = [];

    private IComponentBinder<TPayload, TResult> GetBinder(string name)
    {
        var options = optionsMonitor.Get(name);
        var binder = componentBinderFactory.Create()
            .Init(options.TerminateComponent)
            .BindComponents(options.Components);

        return binder;
    }

    public Task<TResult> ExecuteAsync(string name, TPayload payload)
    {
        if(!_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _asyncMiddlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetAsyncMiddlewareDelegate();

        MediumContext<TPayload, TResult> context = new(serviceProvider, payload);
        return middlewareDelegate(context);
    }
    public Task<TResult> ExecuteAsync(TPayload payload) => ExecuteAsync(Medium.DefaultName, payload);

    public TResult Execute(string name, TPayload payload)
    {
        if(!_middlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _middlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetMiddlewareDelegate();

        MediumContext<TPayload, TResult> context = new(serviceProvider, payload);
        return middlewareDelegate(context);
    }
    public TResult Execute(TPayload payload) => Execute(Medium.DefaultName, payload);
}
