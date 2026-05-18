namespace UDL.Delta.IemdSd.Protocol;

public sealed class CurveSnapshot
{
    public uint ReportId { get; init; }

    public uint TotalPoints { get; init; }

    public IReadOnlyList<CurvePoint> Points { get; init; } = Array.Empty<CurvePoint>();

    public CurveScaleInfo? Scale { get; init; }

    public ushort ParameterId { get; init; }
}

public readonly record struct CurvePoint(double TimeMs, double AngleDeg, double TorqueNm);

public sealed class CurveScaleInfo
{
    public uint TotalPoints { get; init; }

    public float MaxTorqueNm { get; init; }

    public int MaxAngle { get; init; }

    public float MaxTimeSec { get; init; }
}
