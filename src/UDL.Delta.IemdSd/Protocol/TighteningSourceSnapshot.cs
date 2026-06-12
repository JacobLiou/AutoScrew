namespace UDL.Delta.IemdSd.Protocol;

public sealed class TighteningSourceSnapshot
{
    public int OperatingMode { get; init; }

    public int SwitchingMethod { get; init; }

    public int SourceId { get; init; }

    public int ParameterId { get; init; }

    public int SequenceId { get; init; }

    public int ScrewCount { get; init; }
}
