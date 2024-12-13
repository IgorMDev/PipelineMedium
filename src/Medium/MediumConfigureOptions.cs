using Microsoft.Extensions.Options;

namespace Medium;

public class MediumConfigureOptions<TPayload> : ConfigureNamedOptions<MediumOptions<TPayload>>
{
    public MediumConfigureOptions(string? name, MediumOptions<TPayload> options) : base(name, o => {
        o.Components = options.Components;
        o.TerminateComponent = options.TerminateComponent;
    })
    {
        ArgumentNullException.ThrowIfNull(options);
    }
}

public class MediumConfigureOptions<TPayload, TResult> : ConfigureNamedOptions<MediumOptions<TPayload, TResult>>
{
    public MediumConfigureOptions(string? name, MediumOptions<TPayload, TResult> options) : base(name, o => {
        o.Components = options.Components;
        o.TerminateComponent = options.TerminateComponent;
    })
    {
        ArgumentNullException.ThrowIfNull(options);
    }
}