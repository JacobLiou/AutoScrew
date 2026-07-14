namespace UDL.Delta.IemdSd.Exceptions;

/// <summary>Device exclusive session is held (e.g. tightening cycle in progress).</summary>
public sealed class IemdSdDeviceBusyException : IemdSdCommunicationException
{
    public IemdSdDeviceBusyException(string message = "IEMD-SD device is busy (another command or cycle owns the session).")
        : base(message)
    {
    }
}
