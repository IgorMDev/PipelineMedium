using Medium.Resources;

using Microsoft.Extensions.Options;

namespace Medium;

public class MediumValidateOptions<TPayload>(string? name) : IValidateOptions<MediumOptions<TPayload>>
{
    public string? Name { get; set; } = name;

    public ValidateOptionsResult Validate(string? name, MediumOptions<TPayload> options)
    {
        if(name is null || name == Name) {
            if(options.Components.Any(c => !c.IsValid) || !options.TerminateComponent.IsValid)
                return ValidateOptionsResult.Fail(Errors.InvalidComponentDescriptor);

            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Skip;
    }
}

public class MediumValidateOptions<TPayload, TResult>(string? name) : IValidateOptions<MediumOptions<TPayload, TResult>>
{
    public string? Name { get; set; } = name;

    public ValidateOptionsResult Validate(string? name, MediumOptions<TPayload, TResult> options)
    {
        if(name is null || name == Name) {
            if(options.Components.Any(c => !c.IsValid) || !options.TerminateComponent.IsValid)
                return ValidateOptionsResult.Fail(Errors.InvalidComponentDescriptor);

            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Skip;
    }
}