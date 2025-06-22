using Microsoft.Extensions.Options;

namespace Medium;

public class MediumConfigureOptions<TRequest> : ConfigureNamedOptions<MediumOptions<TRequest>>
{
    public MediumConfigureOptions(string? name, MediumOptions<TRequest> options) : base(name, o => {
        o.Middlewares = options.Middlewares;
        o.TerminationMiddleware = options.TerminationMiddleware;
    })
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(options);
#else
        if (options is null) {
            throw new ArgumentNullException(nameof(options));
        }
#endif
    }
}

public class MediumConfigureOptions<TRequest, TResult> : ConfigureNamedOptions<MediumOptions<TRequest, TResult>>
{
    public MediumConfigureOptions(string? name, MediumOptions<TRequest, TResult> options) : base(name, o => {
        o.Middlewares = options.Middlewares;
        o.TerminationMiddleware = options.TerminationMiddleware;
    })
    {
#if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(options);
#else
        if (options is null) {
            throw new ArgumentNullException(nameof(options));
        }
#endif
    }
}