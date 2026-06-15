using System.Runtime.CompilerServices;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Domain.Curves;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>
/// Generates a synthetic torque–angle curve for UI and rule tests without hardware.
/// </summary>
public sealed class SimulatedLockStationHardware : ILockStationHardware
{
    private readonly SimulationOptions _simulation;
    private int _pickCount;

    public SimulatedLockStationHardware(IOptions<SimulationOptions> simulation)
    {
        _simulation = simulation.Value;
    }

    public LockHardwareOutcome? LastOutcome => null;

    public async Task PickScrewAsync(CancellationToken cancellationToken = default)
    {
        _pickCount++;
        await Task.Delay(80, cancellationToken).ConfigureAwait(false);

        if (!ShouldFailFeed(_pickCount))
            return;

        var (code, message) = _simulation.FeedFailureMode switch
        {
            SimulatedFeedFailureMode.Timeout => ("FEED_TIMEOUT", "Simulated feeder timeout."),
            SimulatedFeedFailureMode.Empty => ("FEED_EMPTY", "Simulated feeder empty."),
            SimulatedFeedFailureMode.Jam => ("FEED_JAM", "Simulated feeder jam."),
            _ => throw new InvalidOperationException("Unexpected feed failure mode."),
        };

        throw new FeedFaultException(code, message);
    }

    public Task PrepareForJobAsync(CancellationToken cancellationToken = default)
    {
        ResetPickCount();
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
        TighteningContext context,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const int steps = 60;
        var peakTorque = _simulation.TighteningProfile switch
        {
            SimulatedTighteningProfile.FloatLock => 0.12,
            SimulatedTighteningProfile.OverTorque => 0.50,
            _ => 0.36,
        };

        for (var i = 0; i <= steps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var t = i / (double)steps;
            var torque = peakTorque * t;
            var angle = 520 * t;
            var rpm = 220 * (1 - 0.15 * t);
            yield return new TorqueAngleSample(i * 3.0, torque, angle, rpm, null);
            await Task.Delay(4, cancellationToken).ConfigureAwait(false);
        }
    }

    public void ResetPickCount() => _pickCount = 0;

    private bool ShouldFailFeed(int pickIndex)
    {
        if (_simulation.FeedFailureMode == SimulatedFeedFailureMode.None)
            return false;

        var target = _simulation.FeedFailureOnScrewIndex;
        if (target == 0)
            return false;
        if (target < 0)
            return true;

        return pickIndex == target;
    }
}
