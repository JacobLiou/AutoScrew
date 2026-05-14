using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using Xunit;

namespace AutoScrew.Tests;

public class JobSessionPhaseMachineTests
{
    [Fact]
    public void Idle_to_SnPending_on_RequestScan()
    {
        Assert.True(JobSessionPhaseMachine.TryTransition(JobSessionPhase.Idle, JobSessionTrigger.RequestScan, out var next));
        Assert.Equal(JobSessionPhase.SnPending, next);
    }

    [Fact]
    public void SnPending_to_LoadingRecipe_on_SnValidated()
    {
        Assert.True(JobSessionPhaseMachine.TryTransition(JobSessionPhase.SnPending, JobSessionTrigger.SnValidated, out var next));
        Assert.Equal(JobSessionPhase.LoadingRecipe, next);
    }

    [Fact]
    public void Running_to_NgLocked_on_ScrewNg()
    {
        Assert.True(JobSessionPhaseMachine.TryTransition(JobSessionPhase.Running, JobSessionTrigger.ScrewNg, out var next));
        Assert.Equal(JobSessionPhase.NgLocked, next);
    }
}

public class LockCurveEvaluatorTests
{
    private static SegmentedTorqueProgram DefaultProgram() =>
        new(
            targetTorqueNm: 0.35,
            torqueLowerLimitNm: 0.25,
            torqueUpperLimitNm: 0.38,
            angleLimitDeg: 720,
            maxAxisSkewDeg: 3.0,
            stripDetectionMinSlopeNmPerDeg: 0.5,
            jamTorqueDeltaNm: 0.08,
            jamRpmDropRatio: 0.35);

    [Fact]
    public void Good_rising_curve_is_ok()
    {
        var samples = new TorqueAngleSample[40];
        for (var i = 0; i < samples.Length; i++)
        {
            var t = i / (double)(samples.Length - 1);
            samples[i] = new TorqueAngleSample(i * 2, 0.34 * t, 400 * t, 200, null);
        }

        var r = LockCurveEvaluator.Evaluate(samples, DefaultProgram());
        Assert.True(r.IsOk);
    }

    [Fact]
    public void Over_torque_is_ng()
    {
        var samples = new[]
        {
            new TorqueAngleSample(0, 0.5, 10, 200, null)
        };
        var r = LockCurveEvaluator.Evaluate(samples, DefaultProgram());
        Assert.False(r.IsOk);
        Assert.Equal(NgReason.OverTorque, r.Reason);
    }

    [Fact]
    public void Float_lock_when_peak_below_lower()
    {
        var samples = new[]
        {
            new TorqueAngleSample(0, 0.1, 200, 200, null),
            new TorqueAngleSample(1, 0.12, 400, 200, null)
        };
        var r = LockCurveEvaluator.Evaluate(samples, DefaultProgram());
        Assert.False(r.IsOk);
        Assert.Equal(NgReason.FloatLock, r.Reason);
    }
}
