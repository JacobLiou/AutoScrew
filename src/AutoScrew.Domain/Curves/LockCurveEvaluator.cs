using AutoScrew.Domain.Models;

namespace AutoScrew.Domain.Curves;

/// <summary>
/// Pure evaluation of a completed tightening curve against PRD-style thresholds (simplified heuristics for α).
/// </summary>
public static class LockCurveEvaluator
{
    /// <summary>PRD: immediate stop if actual torque &gt; 110% setpoint.</summary>
    public const double OverTorqueFactor = 1.10;

    public static LockEvaluationResult Evaluate(ReadOnlySpan<TorqueAngleSample> samples, SegmentedTorqueProgram program)
    {
        if (samples.Length == 0)
            return LockEvaluationResult.Ng(NgReason.FloatLock, "FLOAT_001", "No curve samples.");

        var maxTorque = double.MinValue;
        var maxAngle = double.MinValue;
        var maxSkew = 0.0;
        var hasSkew = false;

        for (var i = 0; i < samples.Length; i++)
        {
            var s = samples[i];
            if (s.TorqueNm > maxTorque)
                maxTorque = s.TorqueNm;
            if (s.AngleDeg > maxAngle)
                maxAngle = s.AngleDeg;
            if (s.AxisSkewDeg is { } skew)
            {
                hasSkew = true;
                var a = Math.Abs(skew);
                if (a > maxSkew)
                    maxSkew = a;
            }
        }

        var overLimit = program.TargetTorqueNm * OverTorqueFactor;
        if (maxTorque > overLimit)
            return LockEvaluationResult.Ng(NgReason.OverTorque, "OVER_TORQUE_001", $"Peak {maxTorque:F3} N·m exceeds {overLimit:F3} N·m.");

        if (hasSkew && maxSkew > program.MaxAxisSkewDeg)
            return LockEvaluationResult.Ng(NgReason.AxisSkew, "SKEW_003", $"Axis skew {maxSkew:F2}° exceeds {program.MaxAxisSkewDeg:F2}°.");

        if (maxTorque < program.TorqueLowerLimitNm)
            return LockEvaluationResult.Ng(NgReason.FloatLock, "FLOAT_002", $"Peak torque {maxTorque:F3} N·m below lower limit {program.TorqueLowerLimitNm:F3} N·m.");

        if (maxAngle > program.AngleLimitDeg && !TorqueGrowing(samples, program.TargetTorqueNm))
            return LockEvaluationResult.Ng(NgReason.StripSlip, "STRIP_001", "Angle high with limited torque growth (strip/slip heuristic).");

        if (DetectJam(samples, program))
            return LockEvaluationResult.Ng(NgReason.JammedScrew, "JAM_001", "Sudden torque rise with RPM drop (heuristic).");

        return LockEvaluationResult.Ok();
    }

    private static bool TorqueGrowing(ReadOnlySpan<TorqueAngleSample> samples, double targetTorqueNm)
    {
        if (samples.Length < 4)
            return true;

        var last = samples[^1].TorqueNm;
        return last >= targetTorqueNm * 0.85;
    }

    private static bool DetectJam(ReadOnlySpan<TorqueAngleSample> samples, SegmentedTorqueProgram program)
    {
        if (samples.Length < 5)
            return false;

        for (var i = 2; i < samples.Length - 2; i++)
        {
            var prevT = samples[i - 1].TorqueNm;
            var t = samples[i].TorqueNm;
            var nextT = samples[i + 1].TorqueNm;
            var prevR = samples[i - 1].Rpm;
            var r = samples[i].Rpm;

            var delta = Math.Max(t, nextT) - Math.Min(prevT, t);
            if (delta < program.JamTorqueDeltaNm)
                continue;

            if (prevR > 1 && r < prevR * (1.0 - program.JamRpmDropRatio))
                return true;
        }

        return false;
    }
}
