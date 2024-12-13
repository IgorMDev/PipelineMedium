namespace Medium;

public interface IMedium
{
    Task ExecuteAsync<TPayload>(string name, TPayload payload);
    Task ExecuteAsync<TPayload>(TPayload payload);

    void Execute<TPayload>(string name, TPayload payload);
    void Execute<TPayload>(TPayload payload);

    Task<TResult> ExecuteAsync<TPayload, TResult>(string name, TPayload payload);
    Task<TResult> ExecuteAsync<TPayload, TResult>(TPayload payload);

    TResult Execute<TPayload, TResult>(string name, TPayload payload);
    TResult Execute<TPayload, TResult>(TPayload payload);
}

public interface IMedium<TPayload>
{
    Task ExecuteAsync(string name, TPayload payload);
    Task ExecuteAsync(TPayload payload);

    void Execute(string name, TPayload payload);
    void Execute(TPayload payload);
}

public interface IMedium<TPayload, TResult>
{
    Task<TResult> ExecuteAsync(string name, TPayload input);
    Task<TResult> ExecuteAsync(TPayload input);

    TResult Execute(string name, TPayload input);
    TResult Execute(TPayload input);
}
