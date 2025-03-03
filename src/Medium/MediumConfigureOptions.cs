using Microsoft.Extensions.Options;

namespace Medium;

public class MediumConfigureOptions<TPayload> : ConfigureNamedOptions<MediumOptions<TPayload>>
{
    public MediumConfigureOptions(string? name, MediumOptions<TPayload> options) : base(name, o => {
        o.Components = options.Components;
        o.TerminateComponent = options.TerminateComponent;
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

public class MediumConfigureOptions<TPayload, TResult> : ConfigureNamedOptions<MediumOptions<TPayload, TResult>>
{
    public MediumConfigureOptions(string? name, MediumOptions<TPayload, TResult> options) : base(name, o => {
        o.Components = options.Components;
        o.TerminateComponent = options.TerminateComponent;
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