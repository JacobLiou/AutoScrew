namespace AutoScrew.Domain.Models;

/// <summary>
/// Three-stage torque program (PRD: pre-lock, lock, tighten).
/// </summary>
public sealed class SegmentedTorqueProgram
{
    public SegmentedTorqueProgram(
        double targetTorqueNm,
        double torqueLowerLimitNm,
        double torqueUpperLimitNm,
        double angleLimitDeg,
        double maxAxisSkewDeg,
        double stripDetectionMinSlopeNmPerDeg,
        double jamTorqueDeltaNm,
        double jamRpmDropRatio)
    {
        TargetTorqueNm = targetTorqueNm;
        TorqueLowerLimitNm = torqueLowerLimitNm;
        TorqueUpperLimitNm = torqueUpperLimitNm;
        AngleLimitDeg = angleLimitDeg;
        MaxAxisSkewDeg = maxAxisSkewDeg;
        StripDetectionMinSlopeNmPerDeg = stripDetectionMinSlopeNmPerDeg;
        JamTorqueDeltaNm = jamTorqueDeltaNm;
        JamRpmDropRatio = jamRpmDropRatio;
    }

    public double TargetTorqueNm { get; }

    public double TorqueLowerLimitNm { get; }

    public double TorqueUpperLimitNm { get; }

    public double AngleLimitDeg { get; }

    public double MaxAxisSkewDeg { get; }

    public double StripDetectionMinSlopeNmPerDeg { get; }

    public double JamTorqueDeltaNm { get; }

    public double JamRpmDropRatio { get; }
}
