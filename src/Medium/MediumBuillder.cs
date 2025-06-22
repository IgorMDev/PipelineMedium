using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Medium;

/// <summary>
/// Provides a builder for configuring Medium services with a specific request type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class MediumBuilder<TRequest>
{
    /// <summary>
    /// Gets the service collection to which the Medium services are added.
    /// </summary>
    protected readonly IServiceCollection Services;

    /// <summary>
    /// Gets the options for configuring the Medium.
    /// </summary>
    protected readonly MediumOptions<TRequest> Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediumBuilder{TRequest}"/> class.
    /// </summary>
    /// <param name="services">The service collection to which the Medium services are added.</param>
    public MediumBuilder(IServiceCollection services) : this(services, Medium.DefaultName) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediumBuilder{TRequest}"/> class.
    /// </summary>
    /// <param name="services">The service collection to which the Medium services are added.</param>
    /// <param name="name">The name of the Medium.</param>
    public MediumBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Options = InitOptions();
        ConfigureOptions(name, Options);
    }

    /// <summary>
    /// Initializes the options for configuring the Medium.
    /// </summary>
    /// <returns>The initialized options.</returns>
    protected virtual MediumOptions<TRequest> InitOptions() => new();

    /// <summary>
    /// Configures the options for the Medium.
    /// </summary>
    /// <param name="name">The name of the Medium.</param>
    /// <param name="options">The options to configure.</param>
    protected virtual void ConfigureOptions(string name, MediumOptions<TRequest> options)
    {
        Services.AddSingleton<IConfigureOptions<MediumOptions<TRequest>>>(new MediumConfigureOptions<TRequest>(name, options));
        Services.AddSingleton<IValidateOptions<MediumOptions<TRequest>>>(new MediumValidateOptions<TRequest>(name));
    }

    /// <summary>
    /// Sets the default termination middleware for the Medium.
    /// </summary>
    /// <param name="descriptor">The termination middleware descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    internal MediumBuilder<TRequest> UseTermination(TerminationMiddlewareDescriptor<TRequest> descriptor)
    {
        Options.TerminationMiddleware = descriptor;
        return this;
    }

    /// <summary>
    /// Adds a middleware descriptor to the Medium.
    /// </summary>
    /// <param name="descriptor">The middleware descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    internal MediumBuilder<TRequest> Use(MiddlewareDescriptor<TRequest> descriptor)
    {
        Options.Middlewares.Insert(0, descriptor);
        return this;
    }

    /// <summary>
    /// Creates a new instance of the Medium with the configured options.
    /// </summary>
    /// <returns>Medium instance</returns>
    public Medium<TRequest> Build(IServiceProvider serviceProvider)
    {
        return new Medium<TRequest>(serviceProvider, Options);
    }

    /// <summary>
    /// Sets termination asynchronous middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseTermination(AsyncMiddlewareDelegate<TRequest> middleware)
        => UseTermination(new TerminationMiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Sets termination middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseTermination(MiddlewareDelegate<TRequest> middleware)
        => UseTermination(new TerminationMiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use(Type middlewareType)
        => Use(new MiddlewareDescriptor<TRequest>(middlewareType));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen(Predicate<TRequest> condition, Type middlewareType)
        => Use(new MiddlewareDescriptor<TRequest>(middlewareType) { Condition = condition });

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use<TMiddleware>() => Use(typeof(TMiddleware));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen<TMiddleware>(Predicate<TRequest> condition) => UseWhen(condition, typeof(TMiddleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use(AsyncMiddlewareFunc<TRequest> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen(Predicate<TRequest> condition, AsyncMiddlewareFunc<TRequest> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use(Func<TRequest, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen(Predicate<TRequest> condition, Func<TRequest, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use(Func<IServiceProvider, TRequest, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen(Predicate<TRequest> condition, Func<IServiceProvider, TRequest, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use<TDep>(Func<TRequest, TDep, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => (request, ct) => middleware(request, sp.GetRequiredService<TDep>(), next, ct)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen<TDep>(Predicate<TRequest> condition, Func<TRequest, TDep, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => (request, ct) => middleware(request, sp.GetRequiredService<TDep>(), next, ct)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use<TDep1, TDep2>(Func<TRequest, TDep1, TDep2, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => (request, ct) => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next, ct)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen<TDep1, TDep2>(Predicate<TRequest> condition, Func<TRequest, TDep1, TDep2, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => (request, ct) => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next, ct)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use<TDep1, TDep2, TDep3>(Func<TRequest, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => (request, ct) => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next, ct)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen<TDep1, TDep2, TDep3>(Predicate<TRequest> condition, Func<TRequest, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate, CancellationToken, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => (request, ct) => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next, ct)) { Condition = condition });

    /// <summary>
    /// Adds a middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use(MiddlewareFunc<TRequest> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Adds a middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen(Predicate<TRequest> condition, MiddlewareFunc<TRequest> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use(Action<TRequest, NextMiddlewareDelegate> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen(Predicate<TRequest> condition, Action<TRequest, NextMiddlewareDelegate> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use(Action<IServiceProvider, TRequest, NextMiddlewareDelegate> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen(Predicate<TRequest> condition, Action<IServiceProvider, TRequest, NextMiddlewareDelegate> middleware)
        => Use(new MiddlewareDescriptor<TRequest>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use<TDep>(Action<TDep, TRequest, NextMiddlewareDelegate> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => request => middleware(sp.GetRequiredService<TDep>(), request, next)));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen<TDep>(Predicate<TRequest> condition, Action<TDep, TRequest, NextMiddlewareDelegate> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => request => middleware(sp.GetRequiredService<TDep>(), request, next)) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use<TDep1, TDep2>(Action<TDep1, TDep2, TRequest, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => request => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), request, next)));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen<TDep1, TDep2>(Predicate<TRequest> condition, Action<TDep1, TDep2, TRequest, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => request => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), request, next)) { Condition = condition }
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> Use<TDep1, TDep2, TDep3>(Action<TDep1, TDep2, TDep3, TRequest, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => request => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), request, next))
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest}"/> instance.</returns>
    public MediumBuilder<TRequest> UseWhen<TDep1, TDep2, TDep3>(Predicate<TRequest> condition, Action<TDep1, TDep2, TDep3, TRequest, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest>((sp, next) => request => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), request, next)) { Condition = condition }
        );
}

/// <summary>
/// Provides a builder for configuring Medium services with a specific request and result type.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class MediumBuilder<TRequest, TResult>
{
    /// <summary>
    /// Gets the service collection to which the Medium services are added.
    /// </summary>
    protected readonly IServiceCollection Services;

    /// <summary>
    /// Gets the options for configuring the Medium.
    /// </summary>
    protected readonly MediumOptions<TRequest, TResult> Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediumBuilder{TRequest, TResult}"/> class.
    /// </summary>
    /// <param name="services">The service collection to which the Medium services are added.</param>
    public MediumBuilder(IServiceCollection services) : this(services, Medium.DefaultName) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MediumBuilder{TRequest, TResult}"/> class.
    /// </summary>
    /// <param name="services">The service collection to which the Medium services are added.</param>
    /// <param name="name">The name of the Medium.</param>
    public MediumBuilder(IServiceCollection services, string name = Medium.DefaultName)
    {
        Services = services;
        Options = InitOptions();
        ConfigureOptions(name, Options);
    }

    /// <summary>
    /// Initializes the options for configuring the Medium.
    /// </summary>
    /// <returns>The initialized options.</returns>
    protected virtual MediumOptions<TRequest, TResult> InitOptions() => new();

    /// <summary>
    /// Configures the options for the Medium.
    /// </summary>
    /// <param name="name">The name of the Medium.</param>
    /// <param name="options">The options to configure.</param>
    protected virtual void ConfigureOptions(string name, MediumOptions<TRequest, TResult> options)
    {
        Services.AddSingleton<IConfigureOptions<MediumOptions<TRequest, TResult>>>(new MediumConfigureOptions<TRequest, TResult>(name, options));
        Services.AddSingleton<IValidateOptions<MediumOptions<TRequest, TResult>>>(new MediumValidateOptions<TRequest, TResult>(name));
    }

    /// <summary>
    /// Sets the default termination middleware descriptor for the Medium.
    /// </summary>
    /// <param name="descriptor">The termination middleware descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    internal MediumBuilder<TRequest, TResult> UseTermination(TerminationMiddlewareDescriptor<TRequest, TResult> descriptor)
    {
        Options.TerminationMiddleware = descriptor;
        return this;
    }

    /// <summary>
    /// Adds a component descriptor to the Medium.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    internal MediumBuilder<TRequest, TResult> Use(MiddlewareDescriptor<TRequest, TResult> descriptor)
    {
        Options.Middlewares.Insert(0, descriptor);
        return this;
    }

    /// <summary>
    /// Creates a new instance of the Medium with the configured options.
    /// </summary>
    /// <returns>Medium instance</returns>
    public Medium<TRequest, TResult> Build(IServiceProvider serviceProvider)
    {
        return new Medium<TRequest, TResult>(serviceProvider, Options);
    }

    /// <summary>
    /// Sets termination asynchronous middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseTermination(AsyncMiddlewareDelegate<TRequest, TResult> middleware)
        => UseTermination(new TerminationMiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Sets termination middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseTermination(MiddlewareDelegate<TRequest, TResult> middleware)
        => UseTermination(new TerminationMiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use(Type middlewareType)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middlewareType));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen(Predicate<TRequest> condition, Type middlewareType)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middlewareType) { Condition = condition });

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use<TMiddleware>() => Use(typeof(TMiddleware));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen<TMiddleware>(Predicate<TRequest> condition) => UseWhen(condition, typeof(TMiddleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use(AsyncMiddlewareFunc<TRequest, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen(Predicate<TRequest> condition, AsyncMiddlewareFunc<TRequest, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use(Func<TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen(Predicate<TRequest> condition, Func<TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use(Func<IServiceProvider, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen(Predicate<TRequest> condition, Func<IServiceProvider, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use<TDep>(Func<TDep, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => (request, ct) => middleware(sp.GetRequiredService<TDep>(), request, next, ct)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen<TDep>(Predicate<TRequest> condition, Func<TDep, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => (request, ct) => middleware(sp.GetRequiredService<TDep>(), request, next, ct)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use<TDep1, TDep2>(Func<TDep1, TDep2, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => (request, ct) => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), request, next, ct)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen<TDep1, TDep2>(Predicate<TRequest> condition, Func<TDep1, TDep2, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => (request, ct) => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), request, next, ct)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use<TDep1, TDep2, TDep3>(Func<TDep1, TDep2, TDep3, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => (request, ct) => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), request, next, ct)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen<TDep1, TDep2, TDep3>(Predicate<TRequest> condition, Func<TDep1, TDep2, TDep3, TRequest, NextAsyncMiddlewareDelegate<TResult>, CancellationToken, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => (request, ct) => middleware(sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), request, next, ct)) { Condition = condition });

    /// <summary>
    /// Adds a middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use(MiddlewareFunc<TRequest, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Adds a middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen(Predicate<TRequest> condition, MiddlewareFunc<TRequest, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use(Func<TRequest, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen(Predicate<TRequest> condition, Func<TRequest, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use(Func<IServiceProvider, TRequest, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen(Predicate<TRequest> condition, Func<IServiceProvider, TRequest, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new MiddlewareDescriptor<TRequest, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use<TDep>(Func<TRequest, TDep, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => request => middleware(request, sp.GetRequiredService<TDep>(), next)));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen<TDep>(Predicate<TRequest> condition, Func<TRequest, TDep, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => request => middleware(request, sp.GetRequiredService<TDep>(), next)) { Condition = condition }
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use<TDep1, TDep2>(Func<TRequest, TDep1, TDep2, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => request => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next))
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen<TDep1, TDep2>(Predicate<TRequest> condition, Func<TRequest, TDep1, TDep2, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => request => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition }
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> Use<TDep1, TDep2, TDep3>(Func<TRequest, TDep1, TDep2, TDep3, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => request => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next))
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TRequest, TResult}"/> instance.</returns>
    public MediumBuilder<TRequest, TResult> UseWhen<TDep1, TDep2, TDep3>(Predicate<TRequest> condition, Func<TRequest, TDep1, TDep2, TDep3, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new MiddlewareDescriptor<TRequest, TResult>((sp, next) => request => middleware(request, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition }
        );
}