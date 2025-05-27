using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Medium;

/// <summary>
/// Represents a medium for executing operations with a provided request and/or result types.
/// </summary>
public class Medium(IServiceProvider serviceProvider) : IMedium
{
    internal const string DefaultName = "Default";

    /// <inheritdoc />
    public Task ExecuteAsync<TRequest>(string name, TRequest request, CancellationToken cancellationToken = default) => serviceProvider.GetRequiredService<IMedium<TRequest>>().ExecuteAsync(name, request, cancellationToken);

    /// <inheritdoc />
    public Task ExecuteAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default) => serviceProvider.GetRequiredService<IMedium<TRequest>>().ExecuteAsync(request, cancellationToken);

    /// <inheritdoc />
    public void Execute<TRequest>(string name, TRequest request) => serviceProvider.GetRequiredService<IMedium<TRequest>>().Execute(name, request);

    /// <inheritdoc />
    public void Execute<TRequest>(TRequest request) => serviceProvider.GetRequiredService<IMedium<TRequest>>().Execute(request);

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync<TRequest, TResult>(string name, TRequest request, CancellationToken cancellationToken = default) => serviceProvider.GetRequiredService<IMedium<TRequest, TResult>>().ExecuteAsync(name, request, cancellationToken);

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default) => serviceProvider.GetRequiredService<IMedium<TRequest, TResult>>().ExecuteAsync(request, cancellationToken);

    /// <inheritdoc />
    public TResult Execute<TRequest, TResult>(string name, TRequest request) => serviceProvider.GetRequiredService<IMedium<TRequest, TResult>>().Execute(name, request);

    /// <inheritdoc />
    public TResult Execute<TRequest, TResult>(TRequest request) => serviceProvider.GetRequiredService<IMedium<TRequest, TResult>>().Execute(request);
}

/// <summary>
/// Represents a medium for executing operations with a specific request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class Medium<TRequest>(
    IServiceProvider serviceProvider,
    IOptionsMonitor<MediumOptions<TRequest>> optionsMonitor,
    IComponentBinderFactory<TRequest> componentBinderFactory
) : IMedium<TRequest>
{
    private readonly Dictionary<string, ContextualAsyncMiddlewareDelegate<TRequest>> _asyncMiddlewareDelegates = [];
    private readonly Dictionary<string, ContextualMiddlewareDelegate<TRequest>> _middlewareDelegates = [];

    private IComponentBinder<TRequest> GetBinder(string name)
    {
        var options = optionsMonitor.Get(name);
        var binder = componentBinderFactory.Create()
            .Init(options.TerminateComponent)
            .BindComponents(options.Components);

        return binder;
    }

    /// <inheritdoc />
    public Task ExecuteAsync(string name, TRequest request, CancellationToken cancellationToken = default)
    {
        if(!_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _asyncMiddlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetAsyncMiddlewareDelegate();

        MediumContext<TRequest> context = new(serviceProvider, request)
        {
            CancellationToken = cancellationToken
        };
        return middlewareDelegate(context);
    }

    /// <inheritdoc />
    public Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default) => ExecuteAsync(Medium.DefaultName, request, cancellationToken);

    /// <inheritdoc />
    public void Execute(string name, TRequest request)
    {
        if(!_middlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _middlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetMiddlewareDelegate();

        MediumContext<TRequest> context = new(serviceProvider, request);
        middlewareDelegate(context);
    }

    /// <inheritdoc />
    public void Execute(TRequest request) => Execute(Medium.DefaultName, request);
}

/// <summary>
/// Represents a medium for executing operations with a specific request type and returning a result.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class Medium<TRequest, TResult>(
    IServiceProvider serviceProvider,
    IOptionsMonitor<MediumOptions<TRequest, TResult>> optionsMonitor,
    IComponentBinderFactory<TRequest, TResult> componentBinderFactory
) : IMedium<TRequest, TResult>
{
    private readonly Dictionary<string, ContextualAsyncMiddlewareDelegate<TRequest, TResult>> _asyncMiddlewareDelegates = [];
    private readonly Dictionary<string, ContextualMiddlewareDelegate<TRequest, TResult>> _middlewareDelegates = [];

    private IComponentBinder<TRequest, TResult> GetBinder(string name)
    {
        var options = optionsMonitor.Get(name);
        var binder = componentBinderFactory.Create()
            .Init(options.TerminateComponent)
            .BindComponents(options.Components);

        return binder;
    }

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync(string name, TRequest request, CancellationToken cancellationToken = default)
    {
        if(!_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _asyncMiddlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetAsyncMiddlewareDelegate();

        MediumContext<TRequest, TResult> context = new(serviceProvider, request)
        {
            CancellationToken = cancellationToken
        };
        return middlewareDelegate(context);
    }

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default) => ExecuteAsync(Medium.DefaultName, request, cancellationToken);

    /// <inheritdoc />
    public TResult Execute(string name, TRequest request)
    {
        if(!_middlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            _middlewareDelegates[name] = middlewareDelegate = GetBinder(name).GetMiddlewareDelegate();

        MediumContext<TRequest, TResult> context = new(serviceProvider, request);
        return middlewareDelegate(context);
    }

    /// <inheritdoc />
    public TResult Execute(TRequest request) => Execute(Medium.DefaultName, request);
}
