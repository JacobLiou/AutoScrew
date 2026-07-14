namespace UDL.Delta.IemdSd.Protocol;

/// <summary>
/// One exclusive production cycle: GetResultStatus + optional #750/#751 under the same device session.
/// </summary>
public sealed class ProductionTighteningArtifacts
{
    public required TighteningResult Cycle { get; init; }

    public uint ReportId { get; init; }

    public ProductionReport? Report { get; init; }

    public CurveSnapshot? Curve { get; init; }

    public string? ArtifactReadError { get; init; }
}
