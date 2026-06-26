namespace UDL.Delta.Feeder.Protocol;

public sealed class FeedResult
{
    public required bool Success { get; init; }

    public int DurationMs { get; init; }

    public string? ErrorCode { get; init; }

    public string? Message { get; init; }
}
