using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Medium;

/// <summary>
/// Provides a builder for configuring Medium services with a specific payload type.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public class MediumBuilder<TPayload>
{
    /// <summary>
    /// Gets the service collection to which the Medium services are added.
    /// </summary>
    protected readonly IServiceCollection Services;

    /// <summary>
    /// Gets the options for configuring the Medium.
    /// </summary>
    protected readonly MediumOptions<TPayload> Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediumBuilder{TPayload}"/> class.
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
    protected virtual MediumOptions<TPayload> InitOptions() => new();

    /// <summary>
    /// Configures the options for the Medium.
    /// </summary>
    /// <param name="name">The name of the Medium.</param>
    /// <param name="options">The options to configure.</param>
    protected virtual void ConfigureOptions(string name, MediumOptions<TPayload> options)
    {
        Services.AddSingleton<IConfigureOptions<MediumOptions<TPayload>>>(new MediumConfigureOptions<TPayload>(name, options));
        Services.AddSingleton<IValidateOptions<MediumOptions<TPayload>>>(new MediumValidateOptions<TPayload>(name));
    }

    /// <summary>
    /// Sets the default terminate component descriptor for the Medium.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    internal MediumBuilder<TPayload> SetDefault(TerminateComponentDescriptor<TPayload> descriptor)
    {
        Options.TerminateComponent = descriptor;
        return this;
    }

    /// <summary>
    /// Adds a component descriptor to the Medium.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    internal MediumBuilder<TPayload> Use(ComponentDescriptor<TPayload> descriptor)
    {
        Options.Components.Insert(0, descriptor);
        return this;
    }

    /// <summary>
    /// Sets the default asynchronous middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> SetDefault(AsyncMiddlewareDelegate<TPayload> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Sets the default middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> SetDefault(MiddlewareDelegate<TPayload> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Type middlewareType)
        => Use(new ComponentDescriptor<TPayload>(middlewareType));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Type middlewareType)
        => Use(new ComponentDescriptor<TPayload>(middlewareType) { Condition = condition });

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TMiddleware>() => Use(typeof(TMiddleware));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TMiddleware>(Predicate<TPayload> condition) => Use(condition, typeof(TMiddleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(AsyncMiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, AsyncMiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Func<TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Func<TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep>(Func<TPayload, TDep, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep>(Predicate<TPayload> condition, Func<TPayload, TDep, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition });

    /// <summary>
    /// Adds a middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(MiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Adds a middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, MiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Action<TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Action<TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Action<IServiceProvider, TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Action<IServiceProvider, TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep>(Action<TPayload, TDep, NextMiddlewareDelegate> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep>(Predicate<TPayload> condition, Action<TPayload, TDep, NextMiddlewareDelegate> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Action<TPayload, TDep1, TDep2, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Predicate<TPayload> condition, Action<TPayload, TDep1, TDep2, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition }
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Action<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next))
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload}"/> instance.</returns>
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Action<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition }
        );
}

/// <summary>
/// Provides a builder for configuring Medium services with a specific payload and result type.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class MediumBuilder<TPayload, TResult>
{
    /// <summary>
    /// Gets the service collection to which the Medium services are added.
    /// </summary>
    protected readonly IServiceCollection Services;

    /// <summary>
    /// Gets the options for configuring the Medium.
    /// </summary>
    protected readonly MediumOptions<TPayload, TResult> Options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediumBuilder{TPayload, TResult}"/> class.
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
    protected virtual MediumOptions<TPayload, TResult> InitOptions() => new();

    /// <summary>
    /// Configures the options for the Medium.
    /// </summary>
    /// <param name="name">The name of the Medium.</param>
    /// <param name="options">The options to configure.</param>
    protected virtual void ConfigureOptions(string name, MediumOptions<TPayload, TResult> options)
    {
        Services.AddSingleton<IConfigureOptions<MediumOptions<TPayload, TResult>>>(new MediumConfigureOptions<TPayload, TResult>(name, options));
        Services.AddSingleton<IValidateOptions<MediumOptions<TPayload, TResult>>>(new MediumValidateOptions<TPayload, TResult>(name));
    }

    /// <summary>
    /// Sets the default terminate component descriptor for the Medium.
    /// </summary>
    /// <param name="descriptor">The terminate component descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    internal MediumBuilder<TPayload, TResult> SetDefault(TerminateComponentDescriptor<TPayload, TResult> descriptor)
    {
        Options.TerminateComponent = descriptor;
        return this;
    }

    /// <summary>
    /// Adds a component descriptor to the Medium.
    /// </summary>
    /// <param name="descriptor">The component descriptor.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    internal MediumBuilder<TPayload, TResult> Use(ComponentDescriptor<TPayload, TResult> descriptor)
    {
        Options.Components.Insert(0, descriptor);
        return this;
    }

    /// <summary>
    /// Sets the default asynchronous middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> SetDefault(AsyncMiddlewareDelegate<TPayload, TResult> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Sets the default middleware for the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> SetDefault(MiddlewareDelegate<TPayload, TResult> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Type middlewareType)
        => Use(new ComponentDescriptor<TPayload, TResult>(middlewareType));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middlewareType">The middleware type.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Type middlewareType)
        => Use(new ComponentDescriptor<TPayload, TResult>(middlewareType) { Condition = condition });

    /// <summary>
    /// Adds a middleware type to the Medium.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TMiddleware>() => Use(typeof(TMiddleware));

    /// <summary>
    /// Adds a middleware type to the Medium with a condition.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of the middleware.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TMiddleware>(Predicate<TPayload> condition) => Use(condition, typeof(TMiddleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(AsyncMiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, AsyncMiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Func<TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep>(Func<TPayload, TDep, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep>(Predicate<TPayload> condition, Func<TPayload, TDep, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition });

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)));

    /// <summary>
    /// Adds an asynchronous middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The asynchronous middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition });

    /// <summary>
    /// Adds a middleware function to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(MiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Adds a middleware function to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, MiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Func<TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a service provider.
    /// </summary>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Func<IServiceProvider, TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a service provider.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<IServiceProvider, TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep>(Func<TPayload, TDep, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)));

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses a dependency.
    /// </summary>
    /// <typeparam name="TDep">The type of the dependency.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep>(Predicate<TPayload> condition, Func<TPayload, TDep, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)) { Condition = condition }
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Func<TPayload, TDep1, TDep2, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next))
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses two dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition }
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Func<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next))
        );

    /// <summary>
    /// Adds a middleware delegate to the Medium with a condition that uses three dependencies.
    /// </summary>
    /// <typeparam name="TDep1">The type of the first dependency.</typeparam>
    /// <typeparam name="TDep2">The type of the second dependency.</typeparam>
    /// <typeparam name="TDep3">The type of the third dependency.</typeparam>
    /// <param name="middleware">The middleware delegate.</param>
    /// <returns>The current <see cref="MediumBuilder{TPayload, TResult}"/> instance.</returns>
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition }
        );
}