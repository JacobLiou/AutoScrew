namespace AutoScrew.Application.Services;

public enum ScrewCycleProgressStep
{
    Started,
    Picking,
    PickCompleteWaitTrigger,
    Tightening,
    CompletedOk,
    CompletedNg,
    FeedFailed
}

public sealed class ScrewCycleProgressEventArgs : EventArgs
{
    public ScrewCycleProgressStep Step { get; init; }

    public string SurfaceName { get; init; } = "";

    public int LocalScrewIndex { get; init; }

    public string? ErrorMessage { get; init; }

    public string? ErrorCode { get; init; }
}
