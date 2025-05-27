using Medium.Tests.Requests;

namespace Medium.Tests.Middlewares;

internal class Invocation1AsyncMiddleware : IAsyncMiddleware<InvocationsRequest>, IAsyncMiddleware<InvocationsRequest, InvocationsResult>
{
    public Task InvokeAsync(InvocationsRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        request.InvocationList.Add(nameof(Invocation1AsyncMiddleware));
        return next();
    }

    public async Task<InvocationsResult> InvokeAsync(InvocationsRequest request, NextAsyncMiddlewareDelegate<InvocationsResult> next, CancellationToken cancellationToken)
    {
        request.InvocationList.Add(nameof(Invocation1AsyncMiddleware));

        var res = await next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation1AsyncMiddleware));
        return res;
    }
}
internal class Invocation2AsyncMiddleware : IAsyncMiddleware<InvocationsRequest>, IAsyncMiddleware<InvocationsRequest, InvocationsResult>
{
    public Task InvokeAsync(InvocationsRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        request.InvocationList.Add(nameof(Invocation2AsyncMiddleware));
        return next();
    }

    public async Task<InvocationsResult> InvokeAsync(InvocationsRequest request, NextAsyncMiddlewareDelegate<InvocationsResult> next, CancellationToken cancellationToken)
    {
        request.InvocationList.Add(nameof(Invocation2AsyncMiddleware));

        var res = await next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation2AsyncMiddleware));
        return res;
    }
}
internal class InvocationTerminateAsyncMiddleware : IAsyncMiddleware<InvocationsRequest>, IAsyncMiddleware<InvocationsRequest, InvocationsResult>
{
    public Task InvokeAsync(InvocationsRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        request.InvocationList.Add(nameof(InvocationTerminateAsyncMiddleware));
        return Task.CompletedTask;
    }

    public Task<InvocationsResult> InvokeAsync(InvocationsRequest request, NextAsyncMiddlewareDelegate<InvocationsResult> next, CancellationToken cancellationToken)
    {
        request.InvocationList.Add(nameof(InvocationTerminateAsyncMiddleware));

        return Task.FromResult(new InvocationsResult {
            InvocationList = [nameof(InvocationTerminateAsyncMiddleware)]
        });
    }
}

internal class Invocation1Middleware : IMiddleware<InvocationsRequest>, IMiddleware<InvocationsRequest, InvocationsResult>
{
    public void Invoke(InvocationsRequest request, NextMiddlewareDelegate next)
    {
        request.InvocationList.Add(nameof(Invocation1Middleware));
        next();
    }

    public InvocationsResult Invoke(InvocationsRequest request, NextMiddlewareDelegate<InvocationsResult> next)
    {
        request.InvocationList.Add(nameof(Invocation1Middleware));

        var res = next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation1Middleware));
        return res;
    }
}
internal class Invocation2Middleware : IMiddleware<InvocationsRequest>, IMiddleware<InvocationsRequest, InvocationsResult>
{
    public void Invoke(InvocationsRequest request, NextMiddlewareDelegate next)
    {
        request.InvocationList.Add(nameof(Invocation2Middleware));
        next();
    }

    public InvocationsResult Invoke(InvocationsRequest request, NextMiddlewareDelegate<InvocationsResult> next)
    {
        request.InvocationList.Add(nameof(Invocation2Middleware));

        var res = next();
        res ??= new();
        res.InvocationList.Add(nameof(Invocation2Middleware));
        return res;
    }
}
internal class InvocationTerminateMiddleware : IMiddleware<InvocationsRequest>, IMiddleware<InvocationsRequest, InvocationsResult>
{
    public void Invoke(InvocationsRequest request, NextMiddlewareDelegate _)
    {
        request.InvocationList.Add(nameof(InvocationTerminateMiddleware));
        return;
    }

    public InvocationsResult Invoke(InvocationsRequest request, NextMiddlewareDelegate<InvocationsResult> _)
    {
        request.InvocationList.Add(nameof(InvocationTerminateMiddleware));

        return new InvocationsResult {
            InvocationList = [nameof(InvocationTerminateMiddleware)]
        };
    }
}