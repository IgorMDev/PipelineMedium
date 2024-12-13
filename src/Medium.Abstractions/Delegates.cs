namespace Medium;

public delegate void NextMiddlewareDelegate();
public delegate Task NextAsyncMiddlewareDelegate();

public delegate void MiddlewareDelegate<in TPayload>(TPayload payload);
public delegate Task AsyncMiddlewareDelegate<in TPayload>(TPayload payload);

public delegate MiddlewareDelegate<TPayload> MiddlewareFunc<TPayload>(NextMiddlewareDelegate next);
public delegate AsyncMiddlewareDelegate<TPayload> AsyncMiddlewareFunc<TPayload>(NextAsyncMiddlewareDelegate next);

public delegate MiddlewareDelegate<TPayload> MiddlewareSPFunc<TPayload>(IServiceProvider serviceProvider, NextMiddlewareDelegate next);
public delegate AsyncMiddlewareDelegate<TPayload> AsyncMiddlewareSPFunc<TPayload>(IServiceProvider serviceProvider, NextAsyncMiddlewareDelegate next);


public delegate TResult NextMiddlewareDelegate<TResult>();
public delegate Task<TResult> NextAsyncMiddlewareDelegate<TResult>();

public delegate TResult MiddlewareDelegate<in TPayload, TResult>(TPayload payload);
public delegate Task<TResult> AsyncMiddlewareDelegate<in TPayload, TResult>(TPayload payload);

public delegate MiddlewareDelegate<TPayload, TResult> MiddlewareFunc<TPayload, TResult>(NextMiddlewareDelegate<TResult> next);
public delegate AsyncMiddlewareDelegate<TPayload, TResult> AsyncMiddlewareFunc<TPayload, TResult>(NextAsyncMiddlewareDelegate<TResult> next);

public delegate MiddlewareDelegate<TPayload, TResult> MiddlewareSPFunc<TPayload, TResult>(IServiceProvider serviceProvider, NextMiddlewareDelegate<TResult> next);
public delegate AsyncMiddlewareDelegate<TPayload, TResult> AsyncMiddlewareSPFunc<TPayload, TResult>(IServiceProvider serviceProvider, NextAsyncMiddlewareDelegate<TResult> next);