# Medium

Medium is set of classes and interfaces that provides a flexible and extensible solution of middleware pipeline pattern.

## Installing

You can install it with NuGet:

    dotnet add package IMD.Medium

## Concepts

### Middleware

Middleware is piece of software that's assembled into a pipeline to handle data and produce result. Each component can choose whether to run the next component in the pipeline and perform work before and after the next component. 
```csharp
//example of middleware function
(payload, next) =>
{
    //run before next middleware 
    next(); //invoke next middleware in pipeline
    //run after next middleware 

    //or

    return; //break execution of middleware pipeline (short-circuiting)
}
```
Middleware components can be of two types: consumable that can only read or modify a payload and produceable that also returns a result. 
An individual middleware component can be specified in-line as an anonymous method or it can be defined in a reusable class. Middleware class supports dependency injection via constructor. Default lifetime of middleware class instance is transient, to change lifetime register a middleware in services collection on your own.
```csharp
//produceable middleware class
public class Middleware(IDependency dependency) : IMiddleware<Payload, Result>
{
    public Result Invoke(Payload payload, NextMiddlewareDelegate<Result> next)
    {
        //code
    }
}

//produceable async middleware class
public class AsyncMiddleware(IDependency dependency) : IAsyncMiddleware<Payload, Result>
{
    public async Task<Result> InvokeAsync(Payload payload, NextAsyncMiddlewareDelegate<Result> next)
    {
        //code
    }
}

//consumable middleware class
public class Middleware(IDependency dependency) : IMiddleware<Payload>
{
    public void Invoke(Payload payload, NextMiddlewareDelegate next)
    {
        //code
    }
}

//consumable async middleware class
public class AsyncMiddleware(IDependency dependency) : IAsyncMiddleware<Payload>
{
    public async Task InvokeAsync(Payload payload, NextAsyncMiddlewareDelegate next)
    {
        //code
    }
}

//in-line middleware functions
services.AddMedium<Payload>()
    .Use((payload, next) => { /* code */ })
    .Use((serviceProvider, payload, next) => { /* code */ }); //with IServiceProvider
    .Use<Dependency>((dependency, payload, next) => { /* code */ }); //with Dependency provided

services.AddMedium<Payload, Result>()
    .Use((payload, next) => { /* code */ })
    .Use((serviceProvider, payload, next) => { /* code */ }); //with IServiceProvider
    .Use<Dependency>((dependency, payload, next) => { /* code */ }); //with Dependency provided
```

### Medium

Medium is definition of middleware pipeline. It encapsulates functionality of building, managing a pipeline and provides convenient interface to execute the pipeline.

To define a medium add it to `IserviceCollection` and configure its middleware pipeline
```csharp
//add default consumable Medium
services.AddMedium<Payload>()
    .Use<Middleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(p => p.Param == null)
    .Use((payload, next) => { /* code */ });

//add default produceable Medium 
services.AddMedium<Payload, Result>()
    .Use<Middleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(p => p.Param == null)
    .Use((payload, next) => { /* code */ });
    
//you can also add other mediums for the same payload, result type by specifying a name (default name is "Default")
//name uniquely identifies a medium for specified payload type
services.AddMedium<Payload>("DoDifferentWay")
    .Use<OtherMiddleware>()

//name uniquely identifies a medium for specified payload and result types combination
services.AddMedium<Payload, OtherResult>("DoDifferentWayAndReturnDifferentResult")
    .Use<OtherMiddleware>()
```
To execute a Medium get an instance from service provider by its generic interface `IMedium<TPayload>`, `IMedium<TPayload, TResult>` from service provider.
Also `IMedium` interface available with generic methods for convenient access to different mediums.
```csharp
var medium = serviceProvider.GetRequiredService<IMedium<Payload>>();
await medium.ExecuteAsync(new Payload());
await medium.ExecuteAsync("DoDifferentWay", new Payload());

var medium = serviceProvider.GetRequiredService<IMedium<Payload, Result>>();
var result = await medium.ExecuteAsync(new Payload());
var result2 = await medium.ExecuteAsync("DoDifferentWayAndReturnDifferentResult", new Payload());

var medium = serviceProvider.GetRequiredService<IMedium>();
await medium.ExecuteAsync<Payload>(new Payload());
await medium.ExecuteAsync<OtherPayload>(new OtherPayload());
```

## Usage

Define a payload type
```csharp
public class Payload {
    public string Param { get; set; }
}
```
Define a result type if you want medium to return a result
```csharp
public class Result {
    public string Data { get; set; }
}
```
Create a middleware to handle the payload and to return result if desired. Middleware class must implement middleware interface.
```csharp
//consumable middleware
public class ConsumePayloadMiddleware(IDependency dependency) : IAsyncMiddleware<Payload>
{
    public async Task InvokeAsync(Payload payload, NextAsyncMiddlewareDelegate next)
    {
        //code
        await next(); //invoke next middleware in pipeline
    }
}

//produceable middleware
public class ConsumePayloadAndReturnResultMiddleware(IDependency dependency) : IAsyncMiddleware<Payload, Result>
{
    public async Task<Result> InvokeAsync(Payload payload, NextAsyncMiddlewareDelegate<Result> next)
    {
        //code
        return await next(); //invoke next middleware in pipeline
    }
}
```
Add medium to `IserviceCollection` and configure its middleware pipeline.
```csharp
//add and configure default medium for processing the Payload
services.AddMedium<Payload>()
    .Use<PrecedingMiddleware>()
    .Use<ConsumePayloadMiddleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(p => p.Param == null)
    .Use((p, next) => { });

//add and configure default medium for processing the Payload and return the Result
services.AddMedium<Payload, Result>()
    .Use<PrecedingMiddleware>()
    .Use<ConsumePayloadAndReturnResultMiddleware>()
    .Use<NextMiddleware>()
    .UseWhen<ConditionalMiddleware>(p => p.Param == null)
    .Use((p, next) => { });
```
Get an instance of medium by its interface using dependency injection
```csharp
public class SomeController : ControllerBase
{
    public async Task<ActionResult<Result>> Get(Payload requestModel, [FromServices] IMedium<Payload, Result> medium)
    {
        return await medium.ExecuteAsync(requestModel);
    }

    public async Task<IActionResult> Post(Payload requestModel, [FromServices] IMedium<Payload> medium)
    {
        await medium.ExecuteAsync(requestModel);

        return Ok();
    }

    public async Task<IActionResult> Put(Payload requestModel, [FromServices] IMedium medium)
    {
        await medium.ExecuteAsync<Payload>(requestModel);
        await medium.ExecuteAsync<OtherPayload>(new OtherPayload());

        return Ok();
    }
}
```

## Features

#### Syncronous and asyncronous middleware in one pipeline
```csharp
services.AddMedium<Payload>()
    .Use<SyncMiddleware>()
    .Use<AsyncMiddleware>()
    .Use((p, next) => { })
    .Use(async (p, next, cancellationToken) => { });
```

#### Default or terminate component of a pipeline. When execution of the pipeline goes next the way down, Medium executes a terminate component to end the process and return a result, you can define own behavior of that component.
```csharp
services.AddMedium<Payload>()
    .Use<SomeMiddleware>()
    .Use((p, next) => { next() })
    .SetDefault(p => {
        Console.WriteLine("Execution ended");
    });

services.AddMedium<Payload, Result>()
    .Use<SomeMiddleware>()
    .Use((p, next) => { next() })
    .SetDefault(p => {
        return new Result();
    });
```