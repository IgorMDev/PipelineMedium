using Medium.Tests.Requests;
using Medium.Tests.Services;

namespace Medium.Tests.Middlewares;

internal class ICheckupAsyncMiddleware : IAsyncMiddleware<ICheckupRequest>
{
    public Task InvokeAsync(ICheckupRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        request.IsInvokedAsync = true;
        return next();
    }
}

internal class CheckupAsyncMiddleware : IAsyncMiddleware<CheckupRequest>
{
    public Task InvokeAsync(CheckupRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        request.IsInvokedAsync = true;
        return next();
    }
}

internal class ICheckupMiddleware : IMiddleware<ICheckupRequest>
{
    public void Invoke(ICheckupRequest request, NextMiddlewareDelegate next)
    {
        request.IsInvoked = true;
        next();
    }
}

internal class CheckupMiddleware : IMiddleware<CheckupRequest>
{
    public void Invoke(CheckupRequest request, NextMiddlewareDelegate next)
    {
        request.IsInvoked = true;
        next();
    }
}

internal class CheckupExceptionAsyncMiddleware : IAsyncMiddleware<CheckupRequest>
{
    public Task InvokeAsync(CheckupRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        throw new CheckupException();
    }
}

internal class CheckupExceptionMiddleware : IMiddleware<CheckupRequest>
{
    public void Invoke(CheckupRequest request, NextMiddlewareDelegate next)
    {
        throw new CheckupException();
    }
}

internal class ICheckupResultAsyncMiddleware : IAsyncMiddleware<ICheckupRequest, CheckupResult>
{
    public Task<CheckupResult> InvokeAsync(ICheckupRequest request, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CheckupResult {
            IsInvokedAsync = true
        });
    }
}

internal class CheckupResultAsyncMiddleware : IAsyncMiddleware<CheckupRequest, CheckupResult>
{
    public Task<CheckupResult> InvokeAsync(CheckupRequest request, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CheckupResult {
            IsInvokedAsync = true
        });
    }
}

internal class ICheckupResultMiddleware : IMiddleware<ICheckupRequest, CheckupResult>
{
    public CheckupResult Invoke(ICheckupRequest request, NextMiddlewareDelegate<CheckupResult> next)
    {
        return new CheckupResult {
            IsInvoked = true
        };
    }
}

internal class CheckupResultMiddleware : IMiddleware<CheckupRequest, CheckupResult>
{
    public CheckupResult Invoke(CheckupRequest request, NextMiddlewareDelegate<CheckupResult> next)
    {
        return new CheckupResult {
            IsInvoked = true
        };
    }
}

internal class CheckupResultExceptionAsyncMiddleware : IAsyncMiddleware<CheckupRequest, CheckupResult>
{
    public Task<CheckupResult> InvokeAsync(CheckupRequest request, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken cancellationToken)
    {
        throw new CheckupException();
    }
}

internal class CheckupResultExceptionMiddleware : IMiddleware<CheckupRequest, CheckupResult>
{
    public CheckupResult Invoke(CheckupRequest request, NextMiddlewareDelegate<CheckupResult> next)
    {
        throw new CheckupException();
    }
}

internal class CheckupAsyncMiddlewareSP(CheckupService checkupService) : IAsyncMiddleware<CheckupRequest>
{
    public async Task InvokeAsync(CheckupRequest request, NextAsyncMiddlewareDelegate next, CancellationToken cancellationToken)
    {
        await checkupService.CheckupAsync(request);
        await next();
    }
}

internal class CheckupMiddlewareSP(CheckupService checkupService) : IMiddleware<CheckupRequest>
{
    public void Invoke(CheckupRequest request, NextMiddlewareDelegate next)
    {
        checkupService.Checkup(request);
        next();
    }
}

internal class CheckupResultAsyncMiddlewareSP(CheckupService checkupService) : IAsyncMiddleware<CheckupRequest, CheckupResult>
{
    public async Task<CheckupResult> InvokeAsync(CheckupRequest request, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken cancellationToken)
    {
        return await checkupService.CheckupResultAsync();
    }
}

internal class CheckupResultMiddlewareSP(CheckupService checkupService) : IMiddleware<CheckupRequest, CheckupResult>
{
    public CheckupResult Invoke(CheckupRequest request, NextMiddlewareDelegate<CheckupResult> next)
    {
        return checkupService.CheckupResult();
    }
}