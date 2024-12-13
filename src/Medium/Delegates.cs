namespace Medium;

public delegate void ContextualMiddlewareDelegate<TPayload>(MediumContext<TPayload> context);
public delegate Task ContextualAsyncMiddlewareDelegate<TPayload>(MediumContext<TPayload> context);

public delegate TResult ContextualMiddlewareDelegate<TPayload, TResult>(MediumContext<TPayload, TResult> context);
public delegate Task<TResult> ContextualAsyncMiddlewareDelegate<TPayload, TResult>(MediumContext<TPayload, TResult> context);