namespace Medium.Tests.Payloads;

internal class CheckupPayload
{
    internal bool IsInvokedAsync { get; set; }
    internal bool IsInvoked { get; set; }
}

internal class CheckupResult
{
    internal bool IsInvokedAsync { get; set; }
    internal bool IsInvoked { get; set; }
}

internal class CheckupException : Exception
{
    internal CheckupException() : base() { }
    internal CheckupException(string message) : base(message) { }
}