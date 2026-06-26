namespace UDL.Delta.ToolDock.Protocol;

public sealed class ToolDockStateChange
{
    public required ToolDockState Previous { get; init; }

    public required ToolDockState Current { get; init; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
