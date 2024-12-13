namespace Medium;

public class ComponentDescriptor<TPayload>
{
    internal ComponentDescriptor() { }
    public ComponentDescriptor(Type middlewareType)
    {
        ArgumentNullException.ThrowIfNull(middlewareType);
        MiddlewareType = middlewareType;
    }

    public ComponentDescriptor(AsyncMiddlewareSPFunc<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        AsyncMiddlewareAction = middlewareAction;
    }
    public ComponentDescriptor(AsyncMiddlewareFunc<TPayload> middleware)
        : this((_, next) => middleware(next))
    { }
    public ComponentDescriptor(Func<TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        : this((_, next) => payload => middleware(payload, next))
    { }
    public ComponentDescriptor(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate, Task> middleware)
        : this((sp, next) => payload => middleware(sp, payload, next))
    { }

    public ComponentDescriptor(MiddlewareSPFunc<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        MiddlewareAction = middlewareAction;
    }
    public ComponentDescriptor(MiddlewareFunc<TPayload> middleware)
        : this((_, next) => middleware(next))
    { }
    public ComponentDescriptor(Action<TPayload, NextMiddlewareDelegate> middleware)
        : this((_, next) => payload => middleware(payload, next))
    { }
    public ComponentDescriptor(Action<IServiceProvider, TPayload, NextMiddlewareDelegate> middleware)
        : this((sp, next) => payload => middleware(sp, payload, next))
    { }

    public Predicate<TPayload>? Condition { get; set; }

    public Type? MiddlewareType { get; }
    public AsyncMiddlewareSPFunc<TPayload>? AsyncMiddlewareAction { get; }
    public MiddlewareSPFunc<TPayload>? MiddlewareAction { get; }

    public bool IsValid => MiddlewareType is not null || AsyncMiddlewareAction is not null || MiddlewareAction is not null;
}

public class TerminateComponentDescriptor<TPayload>
{
    internal TerminateComponentDescriptor()
    {
        Action = _ => { };
        AsyncAction = _ => Task.CompletedTask;
    }
    public TerminateComponentDescriptor(AsyncMiddlewareDelegate<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        AsyncAction = middlewareAction;
    }
    public TerminateComponentDescriptor(MiddlewareDelegate<TPayload> middlewareAction)
    {
        ArgumentNullException.ThrowIfNull(middlewareAction);
        Action = middlewareAction;
    }

    public AsyncMiddlewareDelegate<TPayload>? AsyncAction { get; set; }
    public MiddlewareDelegate<TPayload>? Action { get; set; }

    public bool IsValid => AsyncAction is not null || Action is not null;
}


public class ComponentDescriptor<TPayload, TResult>
{
    internal ComponentDescriptor() { }
    public ComponentDescriptor(Type middlewareType)
    {
        ArgumentNullException.ThrowIfNull(middlewareType);
        MiddlewareType = middlewareType;
    }

    public ComponentDescriptor(AsyncMiddlewareSPFunc<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        AsyncMiddlewareFunc = middlewareFunc;
    }
    public ComponentDescriptor(AsyncMiddlewareFunc<TPayload, TResult> middlewareFunc)
        : this((_, next) => middlewareFunc(next))
    { }
    public ComponentDescriptor(Func<TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        : this((_, next) => payload => middleware(payload, next))
    { }
    public ComponentDescriptor(Func<IServiceProvider, TPayload, NextAsyncMiddlewareDelegate<TResult>, Task<TResult>> middleware)
        : this((sp, next) => payload => middleware(sp, payload, next))
    { }

    public ComponentDescriptor(MiddlewareSPFunc<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        MiddlewareFunc = middlewareFunc;
    }
    public ComponentDescriptor(MiddlewareFunc<TPayload, TResult> middlewareFunc)
        : this((_, next) => middlewareFunc(next))
    { }
    public ComponentDescriptor(Func<TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        : this((_, next) => payload => middleware(payload, next))
    { }
    public ComponentDescriptor(Func<IServiceProvider, TPayload, NextMiddlewareDelegate<TResult>, TResult> middleware)
        : this((sp, next) => payload => middleware(sp, payload, next))
    { }

    public Predicate<TPayload>? Condition { get; set; }

    public Type? MiddlewareType { get; }
    public AsyncMiddlewareSPFunc<TPayload, TResult>? AsyncMiddlewareFunc { get; }
    public MiddlewareSPFunc<TPayload, TResult>? MiddlewareFunc { get; }

    public bool IsValid => MiddlewareType is not null || AsyncMiddlewareFunc is not null || MiddlewareFunc is not null;
}

public class TerminateComponentDescriptor<TPayload, TResult>
{
    internal TerminateComponentDescriptor()
    {
        Func = _ => default!;
        AsyncFunc = _ => Task.FromResult<TResult>(default!);
    }
    public TerminateComponentDescriptor(AsyncMiddlewareDelegate<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        AsyncFunc = middlewareFunc;
    }
    public TerminateComponentDescriptor(MiddlewareDelegate<TPayload, TResult> middlewareFunc)
    {
        ArgumentNullException.ThrowIfNull(middlewareFunc);
        Func = middlewareFunc;
    }

    public AsyncMiddlewareDelegate<TPayload, TResult>? AsyncFunc { get; set; }
    public MiddlewareDelegate<TPayload, TResult>? Func { get; set; }

    public bool IsValid => AsyncFunc is not null || Func is not null;
}