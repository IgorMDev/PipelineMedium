using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Medium;

/// <summary>
/// Represents a medium for executing operations with a provided payload and/or result types.
/// </summary>
public class Medium(IServiceProvider serviceProvider) : IMedium
{
    internal const string DefaultName = "Default";

    /// <inheritdoc />
    public Task ExecuteAsync<TPayload>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().ExecuteAsync(name, payload);

    /// <inheritdoc />
    public Task ExecuteAsync<TPayload>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().ExecuteAsync(payload);

    /// <inheritdoc />
    public void Execute<TPayload>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().Execute(name, payload);

    /// <inheritdoc />
    public void Execute<TPayload>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload>>().Execute(payload);

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync<TPayload, TResult>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().ExecuteAsync(name, payload);

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync<TPayload, TResult>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().ExecuteAsync(payload);

    /// <inheritdoc />
    public TResult Execute<TPayload, TResult>(string name, TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().Execute(name, payload);

    /// <inheritdoc />
    public TResult Execute<TPayload, TResult>(TPayload payload) => serviceProvider.GetRequiredService<IMedium<TPayload, TResult>>().Execute(payload);
}

/// <summary>
/// Represents a medium for executing operations with a specific payload type.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
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

    /// <inheritdoc />
    public Task ExecuteAsync(string name, TPayload payload)
    {
        if(!_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _asyncMiddlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetAsyncMiddlewareDelegate();

        MediumContext<TPayload> context = new(serviceProvider, payload);
        return middlewareDelegate(context);
    }

    /// <inheritdoc />
    public Task ExecuteAsync(TPayload payload) => ExecuteAsync(Medium.DefaultName, payload);

    /// <inheritdoc />
    public void Execute(string name, TPayload payload)
    {
        if(!_middlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _middlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetMiddlewareDelegate();

        MediumContext<TPayload> context = new(serviceProvider, payload);
        middlewareDelegate(context);
    }

    /// <inheritdoc />
    public void Execute(TPayload payload) => Execute(Medium.DefaultName, payload);
}

/// <summary>
/// Represents a medium for executing operations with a specific payload type and returning a result.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
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

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync(string name, TPayload payload)
    {
        if(!_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _asyncMiddlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetAsyncMiddlewareDelegate();

        MediumContext<TPayload, TResult> context = new(serviceProvider, payload);
        return middlewareDelegate(context);
    }

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync(TPayload payload) => ExecuteAsync(Medium.DefaultName, payload);

    /// <inheritdoc />
    public TResult Execute(string name, TPayload payload)
    {
        if(!_middlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _middlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetMiddlewareDelegate();

        MediumContext<TPayload, TResult> context = new(serviceProvider, payload);
        return middlewareDelegate(context);
    }

    /// <inheritdoc />
    public TResult Execute(TPayload payload) => Execute(Medium.DefaultName, payload);
}
