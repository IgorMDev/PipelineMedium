using Medium.Tests.Payloads;
using Medium.Tests.Services;

namespace Medium.Tests.Middlewares;

internal class CheckupAsyncMiddleware : IAsyncMiddleware<CheckupPayload>
{
    public Task InvokeAsync(CheckupPayload payload, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        payload.IsInvokedAsync = true;
        return next();
    }
}

internal class CheckupMiddleware : IMiddleware<CheckupPayload>
{
    public void Invoke(CheckupPayload payload, NextMiddlewareDelegate next)
    {
        payload.IsInvoked = true;
        next();
    }
}

internal class CheckupExceptionAsyncMiddleware : IAsyncMiddleware<CheckupPayload>
{
    public Task InvokeAsync(CheckupPayload payload, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        throw new CheckupException();
    }
}

internal class CheckupExceptionMiddleware : IMiddleware<CheckupPayload>
{
    public void Invoke(CheckupPayload payload, NextMiddlewareDelegate next)
    {
        throw new CheckupException();
    }
}

internal class CheckupResultAsyncMiddleware : IAsyncMiddleware<CheckupPayload, CheckupResult>
{
    public Task<CheckupResult> InvokeAsync(CheckupPayload payload, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CheckupResult {
            IsInvokedAsync = true
        });
    }
}

internal class CheckupResultMiddleware : IMiddleware<CheckupPayload, CheckupResult>
{
    public CheckupResult Invoke(CheckupPayload payload, NextMiddlewareDelegate<CheckupResult> next)
    {
        return new CheckupResult {
            IsInvoked = true
        };
    }
}

internal class CheckupResultExceptionAsyncMiddleware : IAsyncMiddleware<CheckupPayload, CheckupResult>
{
    public Task<CheckupResult> InvokeAsync(CheckupPayload payload, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken cancellationToken)
    {
        throw new CheckupException();
    }
}

internal class CheckupResultExceptionMiddleware : IMiddleware<CheckupPayload, CheckupResult>
{
    public CheckupResult Invoke(CheckupPayload payload, NextMiddlewareDelegate<CheckupResult> next)
    {
        throw new CheckupException();
    }
}

internal class CheckupAsyncMiddlewareSP(CheckupService checkupService) : IAsyncMiddleware<CheckupPayload>
{
    public async Task InvokeAsync(CheckupPayload payload, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        await checkupService.CheckupAsync(payload);
        await next();
    }
}

internal class CheckupMiddlewareSP(CheckupService checkupService) : IMiddleware<CheckupPayload>
{
    public void Invoke(CheckupPayload payload, NextMiddlewareDelegate next)
    {
        checkupService.Checkup(payload);
        next();
    }
}

internal class CheckupResultAsyncMiddlewareSP(CheckupService checkupService) : IAsyncMiddleware<CheckupPayload, CheckupResult>
{
    public async Task<CheckupResult> InvokeAsync(CheckupPayload payload, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken cancellationToken)
    {
        return await checkupService.CheckupResultAsync();
    }
}

internal class CheckupResultMiddlewareSP(CheckupService checkupService) : IMiddleware<CheckupPayload, CheckupResult>
{
    public CheckupResult Invoke(CheckupPayload payload, NextMiddlewareDelegate<CheckupResult> next)
    {
        return checkupService.CheckupResult();
    }
}