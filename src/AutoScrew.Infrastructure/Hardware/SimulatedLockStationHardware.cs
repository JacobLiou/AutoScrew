using System.Runtime.CompilerServices;
using AutoScrew.Application.Abstractions;
using AutoScrew.Domain.Curves;

namespace AutoScrew.Infrastructure.Hardware;

/// <summary>
/// Generates a synthetic torque–angle curve for UI and rule tests without hardware.
/// </summary>
public sealed class SimulatedLockStationHardware : ILockStationHardware
{
    public Task PickScrewAsync(CancellationToken cancellationToken = default) =>
        Task.Delay(80, cancellationToken);

    public async IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        const int steps = 60;
        for (var i = 0; i <= steps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var t = i / (double)steps;
            var torque = 0.36 * t;
            var angle = 520 * t;
            var rpm = 220 * (1 - 0.15 * t);
            yield return new TorqueAngleSample(i * 3.0, torque, angle, rpm, null);
            await Task.Delay(4, cancellationToken).ConfigureAwait(false);
        }
    }
}
