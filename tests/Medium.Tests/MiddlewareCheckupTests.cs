using Medium.Tests.Middlewares;
using Medium.Tests.Requests;
using Medium.Tests.Services;

using Microsoft.Extensions.DependencyInjection;

using Medium.DependencyInjection;

namespace Medium.Tests;

public partial class MiddlewareCheckupTests
{
    private static IMedium<CheckupRequest> CreateMedium(Action<MediumBuilder<CheckupRequest>>? mediumBuilderAction)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddOptions();
        services.AddTransient<CheckupService>();
        var mediumBuilder = services.AddMedium<CheckupRequest>();

        if(mediumBuilderAction is not null)
            mediumBuilderAction(mediumBuilder);

        var sp = services.BuildServiceProvider();
        var medium = sp.GetRequiredService<IMedium<CheckupRequest>>();

        return medium;
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddleware>());

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddleware>());

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionAsyncMiddleware>());

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionMiddleware>());

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddlewareSP>());

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddlewareSP>());

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddleware>());

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddleware>());

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionAsyncMiddleware>());

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public void Execute_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupExceptionMiddleware>());

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupAsyncMiddlewareSP>());

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupMiddlewareSP>());

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            p.IsInvokedAsync = true;
            return next();
        }));

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            p.IsInvoked = true;
            next();
        }));

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            p.IsInvokedAsync = true;
            return next();
        }));

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            p.IsInvoked = true;
            next();
        }));

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public void Execute_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithSP()
    {
        var medium = CreateMedium(b => b.Use(async (sp, p, next, _) => {
            var chService = sp.GetRequiredService<CheckupService>();
            await chService.CheckupAsync(p);
            await next();
        }));

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFuncWithSP()
    {
        var medium = CreateMedium(b => b.Use((sp, p, next) => {
            var chService = sp.GetRequiredService<CheckupService>();
            chService.Checkup(p);
            next();
        }));

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>(async (p, chService, next, _) => {
            await chService.CheckupAsync(p);
            await next();
        }));

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>((chService ,p, next) => {
            chService.Checkup(p);
            next();
        }));

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeDefaultAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault((p, _) => {
            p.IsInvokedAsync = true;
            return Task.CompletedTask;
        }));

        var request = new CheckupRequest();
        await medium.ExecuteAsync(request);

        Assert.True(request.IsInvokedAsync);
        Assert.False(request.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeDefaultMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault(p => {
            p.IsInvoked = true;
        }));

        var request = new CheckupRequest();
        medium.Execute(request);

        Assert.True(request.IsInvoked);
        Assert.False(request.IsInvokedAsync);
    }
}

public class MiddlewareWithResultCheckupTests
{
    private static IMedium<CheckupRequest, CheckupResult> CreateMedium(Action<MediumBuilder<CheckupRequest, CheckupResult>>? mediumBuilderAction)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddOptions();
        services.AddTransient<CheckupService>();
        var mediumBuilder = services.AddMedium<CheckupRequest, CheckupResult>();

        if(mediumBuilderAction is not null)
            mediumBuilderAction(mediumBuilder);

        var sp = services.BuildServiceProvider();
        var medium = sp.GetRequiredService<IMedium<CheckupRequest, CheckupResult>>();

        return medium;
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddleware>());

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddleware>());

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionAsyncMiddleware>());

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionMiddleware>());

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddlewareSP>());

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddlewareSP>());

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddleware>());

        var request = new CheckupRequest();
        var res = medium.Execute(request);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddleware()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddleware>());

        var request = new CheckupRequest();
        var res = medium.Execute(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionAsyncMiddleware>());

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public void Execute_InvokeMiddleware_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultExceptionMiddleware>());

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultAsyncMiddlewareSP>());

        var request = new CheckupRequest();
        var res = medium.Execute(request);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareSP()
    {
        var medium = CreateMedium(b => b.Use<CheckupResultMiddlewareSP>());

        var request = new CheckupRequest();
        var res = medium.Execute(request);

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

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

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

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((CheckupRequest p, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken _) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        await Assert.ThrowsAsync<CheckupException>(async () => await medium.ExecuteAsync(request));
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc()
    {
        var medium = CreateMedium(b => b.Use((p, next, _) => {
            return Task.FromResult(new CheckupResult {
                IsInvokedAsync = true
            });
        }));

        var request = new CheckupRequest();
        var res = medium.Execute(request);

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

        var request = new CheckupRequest();
        var res = medium.Execute(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public void Execute_InvokeAsyncMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((CheckupRequest p, NextAsyncMiddlewareDelegate<CheckupResult> next, CancellationToken _) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public void Execute_InvokeMiddlewareFunc_ThrowsException()
    {
        var medium = CreateMedium(b => b.Use((p, next) => {
            throw new CheckupException();
        }));

        var request = new CheckupRequest();

        Assert.Throws<CheckupException>(() => medium.Execute(request));
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithSP()
    {
        var medium = CreateMedium(b => b.Use(async (sp, p, next, _) => {
            var chService = sp.GetRequiredService<CheckupService>();
            return await chService.CheckupResultAsync();
        }));

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

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

        var request = new CheckupRequest();
        var res = medium.Execute(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>(async (chService, p, next, _) => {
            return await chService.CheckupResultAsync();
        }));

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeMiddlewareFuncWithService()
    {
        var medium = CreateMedium(b => b.Use<CheckupService>((p, chService, next) => {
            return chService.CheckupResult();
        }));

        var request = new CheckupRequest();
        var res = medium.Execute(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }

    [Fact]
    public async Task ExecuteAsync_InvokeDefaultAsyncMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault((p, _) => {
            return Task.FromResult(new CheckupResult { IsInvokedAsync = true });
        }));

        var request = new CheckupRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.True(res.IsInvokedAsync);
        Assert.False(res.IsInvoked);
    }

    [Fact]
    public void Execute_InvokeDefaultMiddleware()
    {
        var medium = CreateMedium(b => b.SetDefault(p => {
            return new CheckupResult { IsInvoked = true };
        }));

        var request = new CheckupRequest();
        var res = medium.Execute(request);

        Assert.True(res.IsInvoked);
        Assert.False(res.IsInvokedAsync);
    }
}
