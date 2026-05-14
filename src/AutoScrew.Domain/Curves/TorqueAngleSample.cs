namespace AutoScrew.Domain.Curves;

public readonly record struct TorqueAngleSample(
    double TimeMs,
    double TorqueNm,
    double AngleDeg,
    double Rpm,
    double? AxisSkewDeg = null);
