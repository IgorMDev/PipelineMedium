using Medium.Tests.Requests;

namespace Medium.Tests.Services;

internal class CheckupService
{
    public Task CheckupAsync(CheckupRequest request)
    {
        request.IsInvokedAsync = true;
        return Task.CompletedTask;
    }

    public void Checkup(CheckupRequest request)
    {
        request.IsInvoked = true;
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