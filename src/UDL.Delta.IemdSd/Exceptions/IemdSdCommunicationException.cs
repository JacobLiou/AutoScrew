namespace UDL.Delta.IemdSd.Exceptions;

public class IemdSdCommunicationException : Exception
{
    public IemdSdCommunicationException(string message) : base(message) { }

    public IemdSdCommunicationException(string message, Exception inner) : base(message, inner) { }

    public int? CommandCode { get; init; }

    public int? DeviceErrorCode { get; init; }
}
