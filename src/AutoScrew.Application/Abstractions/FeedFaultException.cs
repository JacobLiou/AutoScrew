namespace AutoScrew.Application.Abstractions;

public sealed class FeedFaultException : Exception
{
    public FeedFaultException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}
