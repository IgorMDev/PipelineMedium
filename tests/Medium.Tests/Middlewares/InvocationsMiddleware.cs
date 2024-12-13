using Medium.Tests.Payloads;

namespace Medium.Tests.Middlewares;

internal class Invocation1AsyncMiddleware : IAsyncMiddleware<InvocationsPayload>, IAsyncMiddleware<InvocationsPayload, InvocationsResult>
{
    public Task InvokeAsync(InvocationsPayload payload, NextAsyncMiddlewareDelegate next)
    {
        payload.InvocationList.Add(nameof(Invocation1AsyncMiddleware));
        return next();
    }

    public async Task<InvocationsResult> InvokeAsync(InvocationsPayload payload, NextAsyncMiddlewareDelegate<InvocationsResult> next)
    {
        payload.InvocationList.Add(nameof(Invocation1AsyncMiddleware));

        var res = await next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation1AsyncMiddleware));
        return res;
    }
}
internal class Invocation2AsyncMiddleware : IAsyncMiddleware<InvocationsPayload>, IAsyncMiddleware<InvocationsPayload, InvocationsResult>
{
    public Task InvokeAsync(InvocationsPayload payload, NextAsyncMiddlewareDelegate next)
    {
        payload.InvocationList.Add(nameof(Invocation2AsyncMiddleware));
        return next();
    }

    public async Task<InvocationsResult> InvokeAsync(InvocationsPayload payload, NextAsyncMiddlewareDelegate<InvocationsResult> next)
    {
        payload.InvocationList.Add(nameof(Invocation2AsyncMiddleware));

        var res = await next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation2AsyncMiddleware));
        return res;
    }
}
internal class InvocationTerminateAsyncMiddleware : IAsyncMiddleware<InvocationsPayload>, IAsyncMiddleware<InvocationsPayload, InvocationsResult>
{
    public Task InvokeAsync(InvocationsPayload payload, NextAsyncMiddlewareDelegate next)
    {
        payload.InvocationList.Add(nameof(InvocationTerminateAsyncMiddleware));
        return Task.CompletedTask;
    }

    public Task<InvocationsResult> InvokeAsync(InvocationsPayload payload, NextAsyncMiddlewareDelegate<InvocationsResult> next)
    {
        payload.InvocationList.Add(nameof(InvocationTerminateAsyncMiddleware));

        return Task.FromResult(new InvocationsResult {
            InvocationList = [nameof(InvocationTerminateAsyncMiddleware)]
        });
    }
}

internal class Invocation1Middleware : IMiddleware<InvocationsPayload>, IMiddleware<InvocationsPayload, InvocationsResult>
{
    public void Invoke(InvocationsPayload payload, NextMiddlewareDelegate next)
    {
        payload.InvocationList.Add(nameof(Invocation1Middleware));
        next();
    }

    public InvocationsResult Invoke(InvocationsPayload payload, NextMiddlewareDelegate<InvocationsResult> next)
    {
        payload.InvocationList.Add(nameof(Invocation1Middleware));

        var res = next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation1Middleware));
        return res;
    }
}
internal class Invocation2Middleware : IMiddleware<InvocationsPayload>, IMiddleware<InvocationsPayload, InvocationsResult>
{
    public void Invoke(InvocationsPayload payload, NextMiddlewareDelegate next)
    {
        payload.InvocationList.Add(nameof(Invocation2Middleware));
        next();
    }

    public InvocationsResult Invoke(InvocationsPayload payload, NextMiddlewareDelegate<InvocationsResult> next)
    {
        payload.InvocationList.Add(nameof(Invocation2Middleware));

        var res = next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation2Middleware));
        return res;
    }
}
internal class InvocationTerminateMiddleware : IMiddleware<InvocationsPayload>, IMiddleware<InvocationsPayload, InvocationsResult>
{
    public void Invoke(InvocationsPayload payload, NextMiddlewareDelegate _)
    {
        payload.InvocationList.Add(nameof(InvocationTerminateMiddleware));
        return;
    }

    public InvocationsResult Invoke(InvocationsPayload payload, NextMiddlewareDelegate<InvocationsResult> _)
    {
        payload.InvocationList.Add(nameof(InvocationTerminateMiddleware));

        return new InvocationsResult {
            InvocationList = [nameof(InvocationTerminateMiddleware)]
        };
    }
}