namespace UDL.Delta.ToolDock.Exceptions;

public sealed class ToolDockCommunicationException : Exception
{
    public ToolDockCommunicationException(string message) : base(message) { }

    public ToolDockCommunicationException(string message, Exception inner) : base(message, inner) { }
}
