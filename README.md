PipelineMedium is set of classes and interfaces that provides a flexible and extensible solution of middleware pipeline pattern.

## Installing

You can install it with NuGet:

    dotnet add package PipelineMedium

## Concepts

### Middleware

Middleware is piece of software that's assembled into a pipeline to handle data and produce result. Each component can choose whether to run the next component in the pipeline and perform work before and after the next component. 
```csharp
//example of middleware function
(request, next) =>
{
    //run before next middleware 
    next(); //invoke next middleware in pipeline
    //run after next middleware 

    //or

    return; //break execution of middleware pipeline (short-circuiting)
}
```
Middleware components can be of two types: consumable that can only read or modify a request and producible that also returns a result. 
An individual middleware component can be specified in-line as an anonymous method or it can be defined in a reusable class. Middleware class supports dependency injection via constructor. Default lifetime of middleware class instance is transient, to change lifetime register a middleware in services collection on your own.
```csharp
//producible middleware class
public class Middleware(IDependency dependency) : IMiddleware<Request, Result>
{
    public Result Invoke(Request request, NextMiddlewareDelegate<Result> next)
    {
        //code
    }
}

//producible async middleware class
public class AsyncMiddleware(IDependency dependency) : IAsyncMiddleware<Request, Result>
{
    public async Task<Result> InvokeAsync(Request request, NextAsyncMiddlewareDelegate<Result> next)
    {
        //code
    }
}

//consumable middleware class
public class Middleware(IDependency dependency) : IMiddleware<Request>
{
    public void Invoke(Request request, NextMiddlewareDelegate next)
    {
        //code
    }
}

//consumable async middleware class
public class AsyncMiddleware(IDependency dependency) : IAsyncMiddleware<Request>
{
    public async Task InvokeAsync(Request request, NextAsyncMiddlewareDelegate next)
    {
        //code
    }
}

//in-line middleware functions
services.AddMedium<Request>()
    .Use((request, next) => { /* code */ })
    .Use((serviceProvider, request, next) => { /* code */ }) //with IServiceProvider
    .Use<Dependency>((dependency, request, next) => { /* code */ }); //with Dependency provided

services.AddMedium<Request, Result>()
    .Use((request, next) => { /* code */ })
    .Use((serviceProvider, request, next) => { /* code */ }) //with IServiceProvider
    .Use<Dependency>((dependency, request, next) => { /* code */ }); //with Dependency provided
```

### Medium

Medium is definition of middleware pipeline. It encapsulates functionality of building, managing a pipeline and provides convenient interface to execute the pipeline.

To define a medium add it to `IserviceCollection` and configure its middleware pipeline
```csharp
//add default consumable Medium
services.AddMedium<Request>()
    .Use<Middleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(r => r.Param == null)
    .Use((request, next) => { /* code */ });

//add default producible Medium 
services.AddMedium<Request, Result>()
    .Use<Middleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(r => r.Param == null)
    .Use((request, next) => { /* code */ });
    
//you can also add other mediums for the same request, result type by specifying a name (default name is "Default")
//name uniquely identifies a medium for specified request type
services.AddMedium<Request>("DoDifferentWay")
    .Use<OtherMiddleware>()

//name uniquely identifies a medium for specified request and result types combination
services.AddMedium<Request, OtherResult>("DoDifferentWayAndReturnDifferentResult")
    .Use<OtherMiddleware>()
```
To execute a Medium get an instance from service provider by its generic interface `IMedium<TRequest>`, `IMedium<TRequest, TResult>` from service provider.
Also `IMedium` interface available with generic methods for convenient access to different mediums.
```csharp
var medium = serviceProvider.GetRequiredService<IMedium<Request>>();
await medium.ExecuteAsync(new Request());
await medium.ExecuteAsync("DoDifferentWay", new Request());

var medium = serviceProvider.GetRequiredService<IMedium<Request, Result>>();
var result = await medium.ExecuteAsync(new Request());
var result2 = await medium.ExecuteAsync("DoDifferentWayAndReturnDifferentResult", new Request());

var medium = serviceProvider.GetRequiredService<IMedium>();
await medium.ExecuteAsync<Request>(new Request());
await medium.ExecuteAsync<OtherRequest>(new OtherRequest());
```

## Usage

Define a request type
```csharp
public class Request {
    public string Param { get; set; }
}
```
Define a result type if you want medium to return a result
```csharp
public class Result {
    public string Data { get; set; }
}
```
Create a middleware to handle the request and to return result if desired. Middleware class must implement middleware interface.
```csharp
//consumable middleware
public class ConsumeRequestMiddleware(IDependency dependency) : IAsyncMiddleware<Request>
{
    public async Task InvokeAsync(Request request, NextAsyncMiddlewareDelegate next)
    {
        //code
        await next(); //invoke next middleware in pipeline
    }
}

//producible middleware
public class ConsumeRequestAndReturnResultMiddleware(IDependency dependency) : IAsyncMiddleware<Request, Result>
{
    public async Task<Result> InvokeAsync(Request request, NextAsyncMiddlewareDelegate<Result> next)
    {
        //code
        return await next(); //invoke next middleware in pipeline
    }
}
```
Add medium to `IserviceCollection` and configure its middleware pipeline.
```csharp
//add and configure default medium for processing the Request
services.AddMedium<Request>()
    .Use<PrecedingMiddleware>()
    .Use<ConsumeRequestMiddleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(r => r.Param == null)
    .Use((r, next) => { });

//add and configure default medium for processing the Request and return the Result
services.AddMedium<Request, Result>()
    .Use<PrecedingMiddleware>()
    .Use<ConsumeRequestAndReturnResultMiddleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(r => r.Param == null)
    .Use((r, next) => { });
```
Get an instance of medium by its interface using dependency injection
```csharp
public class SomeController : ControllerBase
{
    public async Task<ActionResult<Result>> Get(Request requestModel, [FromServices] IMedium<Request, Result> medium)
    {
        return await medium.ExecuteAsync(requestModel);
    }

    public async Task<IActionResult> Post(Request requestModel, [FromServices] IMedium<Request> medium)
    {
        await medium.ExecuteAsync(requestModel);

        return Ok();
    }

    public async Task<IActionResult> Put(Request requestModel, [FromServices] IMedium medium)
    {
        await medium.ExecuteAsync<Request>(requestModel);
        await medium.ExecuteAsync<OtherRequest>(new OtherRequest());

        return Ok();
    }
}
```

## Features

#### Synchronous and asynchronous middleware in one pipeline
```csharp
services.AddMedium<Request>()
    .Use<SyncMiddleware>()
    .Use<AsyncMiddleware>()
    .Use((r, next) => { })
    .Use(async (r, next, cancellationToken) => { });
```

#### Default or termination component of a pipeline. When execution of the pipeline goes next the way down, Medium executes a termination component to end the process and return a result, you can define own behavior of that component.
```csharp
services.AddMedium<Request>()
    .Use<SomeMiddleware>()
    .Use((r, next) => { next() })
    .UseTermination(r => {
        Console.WriteLine("Execution ended");
    });

services.AddMedium<Request, Result>()
    .Use<SomeMiddleware>()
    .Use((r, next) => { next() })
    .UseTermination(r => {
        return new Result();
    });
```