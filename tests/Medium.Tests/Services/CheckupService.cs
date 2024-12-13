using Medium.Tests.Payloads;

namespace Medium.Tests.Services;

internal class CheckupService
{
    public Task CheckupAsync(CheckupPayload payload)
    {
        payload.IsInvokedAsync = true;
        return Task.CompletedTask;
    }

    public void Checkup(CheckupPayload payload)
    {
        payload.IsInvoked = true;
    }

    public Task<CheckupResult> CheckupResultAsync()
    {
        return Task.FromResult(new CheckupResult { IsInvokedAsync = true });
    }

    public CheckupResult CheckupResult()
    {
        return new CheckupResult { IsInvoked = true };
    }
}