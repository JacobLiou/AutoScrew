namespace UDL.Delta.Feeder.Protocol;

public sealed class FeedRequest
{
    public string? PartNo { get; init; }

    public int Channel { get; init; }

    public int ProgramId { get; init; }
}
