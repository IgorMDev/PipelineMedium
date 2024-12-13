using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Medium;

public class MediumBuilder<TPayload>
{
    protected readonly IServiceCollection Services;
    protected readonly MediumOptions<TPayload> Options;

    public MediumBuilder(IServiceCollection services, string name = Medium.DefaultName)
    {
        Services = services;
        Options = InitOptions();
        ConfigureOptions(name, Options);
    }

    protected virtual MediumOptions<TPayload> InitOptions() => new();

    protected virtual void ConfigureOptions(string name, MediumOptions<TPayload> options)
    {
        Services.AddSingleton<IConfigureOptions<MediumOptions<TPayload>>>(new MediumConfigureOptions<TPayload>(name, options));
        Services.AddSingleton<IValidateOptions<MediumOptions<TPayload>>>(new MediumValidateOptions<TPayload>(name));
    }

    internal MediumBuilder<TPayload> SetDefault(TerminateComponentDescriptor<TPayload> descriptor)
    {
        Options.TerminateComponent = descriptor;
        return this;
    }
    internal MediumBuilder<TPayload> Use(ComponentDescriptor<TPayload> descriptor)
    {
        Options.Components.Insert(0, descriptor);
        return this;
    }

    public MediumBuilder<TPayload> SetDefault(AsyncMiddlewareDelegate<TPayload> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload>(middleware));
    public MediumBuilder<TPayload> SetDefault(MiddlewareDelegate<TPayload> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload>(middleware));

    public MediumBuilder<TPayload> Use(Type middlewareType)
        => Use(new ComponentDescriptor<TPayload>(middlewareType));
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Type middlewareType)
        => Use(new ComponentDescriptor<TPayload>(middlewareType) { Condition = condition });
    public MediumBuilder<TPayload> Use<TMiddleware>() => Use(typeof(TMiddleware));
    public MediumBuilder<TPayload> Use<TMiddleware>(Predicate<TPayload> condition) => Use(condition, typeof(TMiddleware));

    public MediumBuilder<TPayload> Use(AsyncMiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, AsyncMiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });
    public MediumBuilder<TPayload> Use(Func<TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Func<TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });
    public MediumBuilder<TPayload> Use(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });
    public MediumBuilder<TPayload> Use<TDep>(Func<TPayload, TDep, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep>(Predicate<TPayload> condition, Func<TPayload, TDep, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition }
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate, Task> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition }
        );

    public MediumBuilder<TPayload> Use(MiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, MiddlewareFunc<TPayload> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });
    public MediumBuilder<TPayload> Use(Action<TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Action<TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });
    public MediumBuilder<TPayload> Use(Action<IServiceProvider, TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware));
    public MediumBuilder<TPayload> Use(Predicate<TPayload> condition, Action<IServiceProvider, TPayload, NextMiddlewareDelegate> middleware)
        => Use(new ComponentDescriptor<TPayload>(middleware) { Condition = condition });
    public MediumBuilder<TPayload> Use<TDep>(Action<TPayload, TDep, NextMiddlewareDelegate> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep>(Predicate<TPayload> condition, Action<TPayload, TDep, NextMiddlewareDelegate> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Action<TPayload, TDep1, TDep2, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2>(Predicate<TPayload> condition, Action<TPayload, TDep1, TDep2, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition }
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Action<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next))
        );
    public MediumBuilder<TPayload> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Action<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition }
        );
}

public class MediumBuilder<TPayload, TResult>
{
    protected readonly IServiceCollection Services;
    protected readonly MediumOptions<TPayload, TResult> Options;

    public MediumBuilder(IServiceCollection services, string name = Medium.DefaultName)
    {
        Services = services;
        Options = InitOptions();
        ConfigureOptions(name, Options);
    }

    protected virtual MediumOptions<TPayload, TResult> InitOptions() => new();

    protected virtual void ConfigureOptions(string name, MediumOptions<TPayload, TResult> options)
    {
        Services.AddSingleton<IConfigureOptions<MediumOptions<TPayload, TResult>>>(new MediumConfigureOptions<TPayload, TResult>(name, options));
        Services.AddSingleton<IValidateOptions<MediumOptions<TPayload, TResult>>>(new MediumValidateOptions<TPayload, TResult>(name));
    }

    internal MediumBuilder<TPayload, TResult> SetDefault(TerminateComponentDescriptor<TPayload, TResult> descriptor)
    {
        Options.TerminateComponent = descriptor;
        return this;
    }
    internal MediumBuilder<TPayload, TResult> Use(ComponentDescriptor<TPayload, TResult> descriptor)
    {
        Options.Components.Insert(0, descriptor);
        return this;
    }

    public MediumBuilder<TPayload, TResult> SetDefault(AsyncMiddlewareDelegate<TPayload, TResult> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload, TResult>(middleware));
    public MediumBuilder<TPayload, TResult> SetDefault(MiddlewareDelegate<TPayload, TResult> middleware)
        => SetDefault(new TerminateComponentDescriptor<TPayload, TResult>(middleware));

    public MediumBuilder<TPayload, TResult> Use(Type middlewareType)
        => Use(new ComponentDescriptor<TPayload, TResult>(middlewareType));
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Type middlewareType)
        => Use(new ComponentDescriptor<TPayload, TResult>(middlewareType) { Condition = condition });
    public MediumBuilder<TPayload, TResult> Use<TMiddleware>() => Use(typeof(TMiddleware));
    public MediumBuilder<TPayload, TResult> Use<TMiddleware>(Predicate<TPayload> condition) => Use(condition, typeof(TMiddleware));

    public MediumBuilder<TPayload, TResult> Use(AsyncMiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, AsyncMiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });
    public MediumBuilder<TPayload, TResult> Use(Func<TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });
    public MediumBuilder<TPayload, TResult> Use(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });
    public MediumBuilder<TPayload, TResult> Use<TDep>(Func<TPayload, TDep, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next))
        );
    public MediumBuilder<TPayload, TResult> Use<TDep>(Predicate<TPayload> condition, Func<TPayload, TDep, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next))
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next))
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition }
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next))
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, TDep3, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition }
        );

    public MediumBuilder<TPayload, TResult> Use(MiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, MiddlewareFunc<TPayload, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });
    public MediumBuilder<TPayload, TResult> Use(Func<TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });
    public MediumBuilder<TPayload, TResult> Use(Func<IServiceProvider, TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware));
    public MediumBuilder<TPayload, TResult> Use(Predicate<TPayload> condition, Func<IServiceProvider, TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        => Use(new ComponentDescriptor<TPayload, TResult>(middleware) { Condition = condition });
    public MediumBuilder<TPayload, TResult> Use<TDep>(Func<TPayload, TDep, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next))
        );
    public MediumBuilder<TPayload, TResult> Use<TDep>(Predicate<TPayload> condition, Func<TPayload, TDep, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep>(), next)) { Condition = condition }
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Func<TPayload, TDep1, TDep2, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next))
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), next)) { Condition = condition }
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Func<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next))
        );
    public MediumBuilder<TPayload, TResult> Use<TDep1, TDep2, TDep3>(Predicate<TPayload> condition, Func<TPayload, TDep1, TDep2, TDep3, NextMiddlewareDelegate<TResult>, TResult> middleware)
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        => Use(new ComponentDescriptor<TPayload, TResult>((sp, next) => payload => middleware(payload, sp.GetRequiredService<TDep1>(), sp.GetRequiredService<TDep2>(), sp.GetRequiredService<TDep3>(), next)) { Condition = condition }
        );
}