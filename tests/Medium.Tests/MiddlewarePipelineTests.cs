using Medium.Tests.Middlewares;
using Medium.Tests.Requests;

using Microsoft.Extensions.DependencyInjection;

using Medium.DependencyInjection;

namespace Medium.Tests;

public class MiddlewarePipelineTests
{
    private static IMedium<InvocationsRequest> CreateMedium(Action<MediumBuilder<InvocationsRequest>>? mediumBuilderAction)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddOptions();
        var mediumBuilder = services.AddMedium<InvocationsRequest>();

        if(mediumBuilderAction is not null)
            mediumBuilderAction(mediumBuilder);

        var sp = services.BuildServiceProvider();
        var medium = sp.GetRequiredService<IMedium<InvocationsRequest>>();

        return medium;
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_Order1()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1AsyncMiddleware>()
            .Use<Invocation2AsyncMiddleware>()
        );

        var request = new InvocationsRequest();
        await medium.ExecuteAsync(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i),
            i => Assert.Equal(nameof(Invocation2AsyncMiddleware), i)
        );
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_Order2()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation2AsyncMiddleware>()
            .Use<Invocation1AsyncMiddleware>()
        );

        var request = new InvocationsRequest();
        await medium.ExecuteAsync(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation2AsyncMiddleware), i),
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i)
        );
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_WithTerminate()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1AsyncMiddleware>()
            .Use<InvocationTerminateAsyncMiddleware>()
            .Use<Invocation2AsyncMiddleware>()
        );

        var request = new InvocationsRequest();
        await medium.ExecuteAsync(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i),
            i => Assert.Equal(nameof(InvocationTerminateAsyncMiddleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_Order1()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1Middleware>()
            .Use<Invocation2Middleware>()
        );

        var request = new InvocationsRequest();
        medium.Execute(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation1Middleware), i),
            i => Assert.Equal(nameof(Invocation2Middleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_Order2()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation2Middleware>()
            .Use<Invocation1Middleware>()
        );

        var request = new InvocationsRequest();
        medium.Execute(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation2Middleware), i),
            i => Assert.Equal(nameof(Invocation1Middleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_WithTerminate()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1Middleware>()
            .Use<InvocationTerminateMiddleware>()
            .Use<Invocation2Middleware>()
        );

        var request = new InvocationsRequest();
        medium.Execute(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation1Middleware), i),
            i => Assert.Equal(nameof(InvocationTerminateMiddleware), i)
        );
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_Conditionally()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1AsyncMiddleware>()
            .UseWhen<Invocation2AsyncMiddleware>(p => !p.InvocationList.Contains(nameof(Invocation1AsyncMiddleware)))
            .UseWhen<InvocationTerminateAsyncMiddleware>(p => p.InvocationList.Contains(nameof(Invocation1AsyncMiddleware)))
        );

        var request = new InvocationsRequest();
        await medium.ExecuteAsync(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i),
            i => Assert.Equal(nameof(InvocationTerminateAsyncMiddleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_Conditionally()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1Middleware>()
            .UseWhen<Invocation2Middleware>(p => !p.InvocationList.Contains(nameof(Invocation1Middleware)))
            .UseWhen<InvocationTerminateMiddleware>(p => p.InvocationList.Contains(nameof(Invocation1Middleware)))
        );

        var request = new InvocationsRequest();
        medium.Execute(request);

        Assert.Collection(request.InvocationList, 
            i => Assert.Equal(nameof(Invocation1Middleware), i),
            i => Assert.Equal(nameof(InvocationTerminateMiddleware), i)
        );
    }
}

public class MiddlewareResultPipelineTests
{
    private static IMedium<InvocationsRequest, InvocationsResult> CreateMedium(Action<MediumBuilder<InvocationsRequest, InvocationsResult>>? mediumBuilderAction)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddOptions();
        var mediumBuilder = services.AddMedium<InvocationsRequest, InvocationsResult>();

        if(mediumBuilderAction is not null)
            mediumBuilderAction(mediumBuilder);

        var sp = services.BuildServiceProvider();
        var medium = sp.GetRequiredService<IMedium<InvocationsRequest, InvocationsResult>>();

        return medium;
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_Order1()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1AsyncMiddleware>()
            .Use<Invocation2AsyncMiddleware>()
        );

        var request = new InvocationsRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(Invocation2AsyncMiddleware), i),
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i)
        );
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_Order2()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation2AsyncMiddleware>()
            .Use<Invocation1AsyncMiddleware>()
        );

        var request = new InvocationsRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i),
            i => Assert.Equal(nameof(Invocation2AsyncMiddleware), i)
        );
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_WithTerminate()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1AsyncMiddleware>()
            .Use<InvocationTerminateAsyncMiddleware>()
            .Use<Invocation2AsyncMiddleware>()
        );

        var request = new InvocationsRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(InvocationTerminateAsyncMiddleware), i),
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_Order1()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1Middleware>()
            .Use<Invocation2Middleware>()
        );

        var request = new InvocationsRequest();
        var res = medium.Execute(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(Invocation2Middleware), i),
            i => Assert.Equal(nameof(Invocation1Middleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_Order2()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation2Middleware>()
            .Use<Invocation1Middleware>()
        );

        var request = new InvocationsRequest();
        var res = medium.Execute(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(Invocation1Middleware), i),
            i => Assert.Equal(nameof(Invocation2Middleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_WithTerminate()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1Middleware>()
            .Use<InvocationTerminateMiddleware>()
            .Use<Invocation2Middleware>()
        );

        var request = new InvocationsRequest();
        var res = medium.Execute(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(InvocationTerminateMiddleware), i),
            i => Assert.Equal(nameof(Invocation1Middleware), i)
        );
    }

    [Fact]
    public async Task ExecuteAsync_InvokeAsyncMiddlewares_Conditionally()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1AsyncMiddleware>()
            .UseWhen<Invocation2AsyncMiddleware>(p => !p.InvocationList.Contains(nameof(Invocation1AsyncMiddleware)))
            .UseWhen<InvocationTerminateAsyncMiddleware>(p => p.InvocationList.Contains(nameof(Invocation1AsyncMiddleware)))
        );

        var request = new InvocationsRequest();
        var res = await medium.ExecuteAsync(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(InvocationTerminateAsyncMiddleware), i),
            i => Assert.Equal(nameof(Invocation1AsyncMiddleware), i)
        );
    }

    [Fact]
    public void Execute_InvokeMiddlewares_Conditionally()
    {
        var medium = CreateMedium(b =>
            b.Use<Invocation1Middleware>()
            .UseWhen<Invocation2Middleware>(p => !p.InvocationList.Contains(nameof(Invocation1Middleware)))
            .UseWhen<InvocationTerminateMiddleware>(p => p.InvocationList.Contains(nameof(Invocation1Middleware)))
        );

        var request = new InvocationsRequest();
        var res = medium.Execute(request);

        Assert.Collection(res.InvocationList, 
            i => Assert.Equal(nameof(InvocationTerminateMiddleware), i),
            i => Assert.Equal(nameof(Invocation1Middleware), i)
        );
    }
}
