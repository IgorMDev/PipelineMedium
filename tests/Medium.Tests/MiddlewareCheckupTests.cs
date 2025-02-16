using Medium.Tests.Middlewares;
using Medium.Tests.Payloads;
using Medium.Tests.Services;

using Microsoft.Extensions.DependencyInjection;

using Medium.DependencyInjection;

namespace Medium.Tests;

public partial class MiddlewareCheckupTests
{
    private static IMedium<CheckupPayload> CreateMedium(Action<MediumBuilder<CheckupPayload>>? mediumBuilderAction)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddOptions();
        services.AddTransient<CheckupService>();
        var mediumBuilder = services.AddMedium<CheckupPayload>();

        if(mediumBuilderAction is not null)
            mediumBuilderAction(mediumBuilder);

        var sp = services.BuildServiceProvider();
        var medium = sp.GetRequiredService<IMedium<CheckupPayload>>();

        return medium;
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddleware>());

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddleware>());

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionAsyncMiddleware>());

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionMiddleware>());

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddlewareSP>());

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddlewareSP>());

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddleware>());

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddleware>());

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionAsyncMiddleware>());

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public void Execute_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionMiddleware>());

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddlewareSP>());

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddlewareSP>());

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            p.IsInvokedAsync = true;
            return next();
        }));

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            p.IsInvoked = true;
            next();
        }));

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            p.IsInvokedAsync = true;
            return next();
        }));

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            p.IsInvoked = true;
            next();
        }));

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public void Execute_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithSP()
    {
        var medium = CreateMedium(b => b.Use(async (sp, p, next, _) => {
            var chService = sp.GetRequiredService<CheckupService>();
            await chService.CheckupAsync(p);
            await next();
        }));

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFuncWithSP()
    {
        var medium = CreateMedium(b => b.Use((sp, p, next) => {
            var chService = sp.GetRequiredService<CheckupService>();
            chService.Checkup(p);
            next();
        }));

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>(async (p, chService, next, _) => {
            await chService.CheckupAsync(p);
            await next();
        }));

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>((chService ,p, next) => {
            chService.Checkup(p);
            next();
        }));

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeDefaultAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault((p, _) => {
            p.IsInvokedAsync = true;
            return Task.CompletedTask;
        }));

        var payload = new CheckupPayload();
        await medium.ExecuteAsync(payload);

        Assert.True(payload.IsInvokedAsync);
        Assert.False(payload.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeDefaultMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault(p => {
            p.IsInvoked = true;
        }));

        var payload = new CheckupPayload();
        medium.Execute(payload);

        Assert.True(payload.IsInvoked);
        Assert.False(payload.IsInvokedAsync);
    }
}

public class MiddlewareWithResultCheckupTests
{
    private static IMedium<CheckupPayload, CheckupResult> CreateMedium(Action<MediumBuilder<CheckupPayload, CheckupResult>>? mediumBuilderAction)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddOptions();
        services.AddTransient<CheckupService>();
        var mediumBuilder = services.AddMedium<CheckupPayload, CheckupResult>();

        if(mediumBuilderAction is not null)
            mediumBuilderAction(mediumBuilder);

        var sp = services.BuildServiceProvider();
        var medium = sp.GetRequiredService<IMedium<CheckupPayload, CheckupResult>>();

        return medium;
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddleware>());

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddleware>());

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionAsyncMiddleware>());

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionMiddleware>());

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddlewareSP>());

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddlewareSP>());

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddleware>());

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddleware>());

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionAsyncMiddleware>());

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public void Execute_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionMiddleware>());

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddlewareSP>());

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddlewareSP>());

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            return Task.FromResult(new CheckupResult {
                IsInvokedAsync = true
            });
        }));

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            return new CheckupResult {
                IsInvoked = true
            };
        }));

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((CheckupPayload p, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken _) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(payload));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            return Task.FromResult(new CheckupResult {
                IsInvokedAsync = true
            });
        }));

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            return new CheckupResult {
                IsInvoked = true
            };
        }));

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((CheckupPayload p, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken _) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public void Execute_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var payload = new CheckupPayload();

        Assert.Throws<CheckupException>(() => medium.Execute(payload));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithSP()
    {
        var medium = CreateMedium(b => b.Use(async (sp, p, next, _) => {
            var chService = sp.GetRequiredService<CheckupService>();
            return await chService.CheckupResultAsync();
        }));

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFuncWithSP()
    {
        var medium = CreateMedium(b => b.Use((sp, p, next) => {
            var chService = sp.GetRequiredService<CheckupService>();
            return chService.CheckupResult();
        }));

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>(async (chService, p, next, _) => {
            return await chService.CheckupResultAsync();
        }));

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>((p, chService, next) => {
            return chService.CheckupResult();
        }));

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeDefaultAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault((p, _) => {
            return Task.FromResult(new CheckupResult { IsInvokedAsync = true });
        }));

        var payload = new CheckupPayload();
        var res = await medium.ExecuteAsync(payload);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeDefaultMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault(p => {
            return new CheckupResult { IsInvoked = true };
        }));

        var payload = new CheckupPayload();
        var res = medium.Execute(payload);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }
}
