using Medium.Resources;

using Microsoft.Extensions.Options;

namespace Medium;

public class MediumValidateOptions<TRequest>(string? name) : IValidateOptions<MediumOptions<TRequest>>
{
    public string? Name { get; set; } = name;

    public ValidateOptionsResult Validate(string? name, MediumOptions<TRequest> options)
    {
        if(name is null || name == Name) {
            if(options.Middlewares.Any(c => !c.IsValid) || !options.TerminationMiddleware.IsValid)
                return ValidateOptionsResult.Fail(Errors.InvalidComponentDescriptor);

            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Skip;
    }
}

public class MediumValidateOptions<TRequest, TResult>(string? name) : IValidateOptions<MediumOptions<TRequest, TResult>>
{
    public string? Name { get; set; } = name;

    public ValidateOptionsResult Validate(string? name, MediumOptions<TRequest, TResult> options)
    {
        if(name is null || name == Name) {
            if(options.Middlewares.Any(c => !c.IsValid) || !options.TerminationMiddleware.IsValid)
                return ValidateOptionsResult.Fail(Errors.InvalidComponentDescriptor);

            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Skip;
    }
}