namespace Medium;

public interface IAsyncMiddleware<TPayload>
{
    Task InvokeAsync(TPayload payload, NextAsyncMiddlewareDelegate next);
}

public interface IMiddleware<TPayload>
{
    void Invoke(TPayload payload, NextMiddlewareDelegate next);
}

public interface IAsyncMiddleware<TPayload, TResult>
{
    Task<TResult> InvokeAsync(TPayload payload, NextAsyncMiddlewareDelegate<TResult> next);
}

public interface IMiddleware<TPayload, TResult>
{
    TResult Invoke(TPayload payload, NextMiddlewareDelegate<TResult> next);
}
