namespace Medium;

public delegate void ContextualMiddlewareDelegate<TRequest>(MediumContext<TRequest> context);
public delegate Task ContextualAsyncMiddlewareDelegate<TRequest>(MediumContext<TRequest> context);

public delegate TResult ContextualMiddlewareDelegate<TRequest, TResult>(MediumContext<TRequest, TResult> context);
public delegate Task<TResult> ContextualAsyncMiddlewareDelegate<TRequest, TResult>(MediumContext<TRequest, TResult> context);