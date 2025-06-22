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
public class Medium<TRequest> : IMedium<TRequest>
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, ContextualAsyncMiddlewareDelegate<TRequest>> _asyncMiddlewareDelegates = [];
    private readonly Dictionary<string, ContextualMiddlewareDelegate<TRequest>> _middlewareDelegates = [];

    private readonly Func<string, MediumOptions<TRequest>> _optionsFactoryFunc;

    public Medium(IServiceProvider serviceProvider, IOptionsMonitor<MediumOptions<TRequest>> optionsMonitor)
    {
        _serviceProvider = serviceProvider;
        _optionsFactoryFunc = name => optionsMonitor.Get(name);
    }

    internal Medium(IServiceProvider serviceProvider, MediumOptions<TRequest> options)
    {
        _serviceProvider = serviceProvider;
        _optionsFactoryFunc = _ => options;
    }

    protected ContextualAsyncMiddlewareDelegate<TRequest> GetAsyncMiddlewareDelegate(in string name)
    {
        if(_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            return middlewareDelegate;

        var options = _optionsFactoryFunc(name);
        var pipeline = new MiddlewarePipeline<TRequest>(options.TerminationMiddleware)
            .AddMiddlewares(options.Middlewares);

        return pipeline.ToAsyncMiddlewareDelegate();
    }

    protected ContextualMiddlewareDelegate<TRequest> GetMiddlewareDelegate(in string name)
    {
        var options = _optionsFactoryFunc(name);
        var pipeline = new MiddlewarePipeline<TRequest>(options.TerminationMiddleware)
            .AddMiddlewares(options.Middlewares);

        return pipeline.ToMiddlewareDelegate();
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(string name, TRequest request, CancellationToken cancellationToken = default)
    {
        var middlewareDelegate = GetAsyncMiddlewareDelegate(name);

        using var serviceScope = _serviceProvider.CreateScope();
        MediumContext<TRequest> context = new(serviceScope.ServiceProvider, request)
        {
            CancellationToken = cancellationToken
        };
        await middlewareDelegate(context);
    }

    /// <inheritdoc />
    public Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default) => ExecuteAsync(Medium.DefaultName, request, cancellationToken);

    /// <inheritdoc />
    public void Execute(string name, TRequest request)
    {
        var middlewareDelegate = GetMiddlewareDelegate(name);

        using var serviceScope = _serviceProvider.CreateScope();
        MediumContext<TRequest> context = new(serviceScope.ServiceProvider, request);
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
public class Medium<TRequest, TResult> : IMedium<TRequest, TResult>
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, ContextualAsyncMiddlewareDelegate<TRequest, TResult>> _asyncMiddlewareDelegates = [];
    private readonly Dictionary<string, ContextualMiddlewareDelegate<TRequest, TResult>> _middlewareDelegates = [];

    private readonly Func<string, MediumOptions<TRequest, TResult>> _optionsFactoryFunc;

    public Medium(IServiceProvider serviceProvider, IOptionsMonitor<MediumOptions<TRequest, TResult>> optionsMonitor)
    {
        _serviceProvider = serviceProvider;
        _optionsFactoryFunc = name => optionsMonitor.Get(name);
    }

    internal Medium(IServiceProvider serviceProvider, MediumOptions<TRequest, TResult> options)
    {
        _serviceProvider = serviceProvider;
        _optionsFactoryFunc = _ => options;
    }

    protected ContextualAsyncMiddlewareDelegate<TRequest, TResult> GetAsyncMiddlewareDelegate(in string name)
    {
        if (_asyncMiddlewareDelegates.TryGetValue(name, out var middlewareDelegate))
            return middlewareDelegate;

        var options = _optionsFactoryFunc(name);
        var pileline = new MiddlewarePipeline<TRequest, TResult>(options.TerminationMiddleware)
            .AddMiddlewares(options.Middlewares);

        return pileline.ToAsyncMiddlewareDelegate();
    }

    protected ContextualMiddlewareDelegate<TRequest, TResult> GetMiddlewareDelegate(in string name)
    {
        var options = _optionsFactoryFunc(name);
        var pileline = new MiddlewarePipeline<TRequest, TResult>(options.TerminationMiddleware)
            .AddMiddlewares(options.Middlewares);

        return pileline.ToMiddlewareDelegate();
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteAsync(string name, TRequest request, CancellationToken cancellationToken = default)
    {
        var middlewareDelegate = GetAsyncMiddlewareDelegate(name);

        using var serviceScope = _serviceProvider.CreateScope();
        MediumContext<TRequest, TResult> context = new(serviceScope.ServiceProvider, request)
        {
            CancellationToken = cancellationToken
        };
        return await middlewareDelegate(context);
    }

    /// <inheritdoc />
    public Task<TResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default) => ExecuteAsync(Medium.DefaultName, request, cancellationToken);

    /// <inheritdoc />
    public TResult Execute(string name, TRequest request)
    {
        var middlewareDelegate = GetMiddlewareDelegate(name);

        using var serviceScope = _serviceProvider.CreateScope();
        MediumContext<TRequest, TResult> context = new(serviceScope.ServiceProvider, request);

        return middlewareDelegate(context);
    }

    /// <inheritdoc />
    public TResult Execute(TRequest request) => Execute(Medium.DefaultName, request);
}
