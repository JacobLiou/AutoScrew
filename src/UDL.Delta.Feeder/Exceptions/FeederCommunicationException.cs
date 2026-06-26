namespace UDL.Delta.Feeder.Exceptions;

public sealed class FeederCommunicationException : Exception
{
    public FeederCommunicationException(string message) : base(message) { }

    public FeederCommunicationException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public FeederCommunicationException(string message, Exception inner) : base(message, inner) { }

    public string? ErrorCode { get; }
}
