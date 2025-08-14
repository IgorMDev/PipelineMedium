namespace Medium.Tests.Requests;

internal interface ICheckupRequest
{
    internal bool IsInvokedAsync { get; set; }
    internal bool IsInvoked { get; set; }
}

internal class CheckupRequest : ICheckupRequest
{
    public bool IsInvokedAsync { get; set; }
    public bool IsInvoked { get; set; }
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