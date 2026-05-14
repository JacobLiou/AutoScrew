using AutoScrew.Domain.Curves;

namespace AutoScrew.Application.Abstractions;

public interface ILockStationHardware
{
    IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(CancellationToken cancellationToken = default);

    Task PickScrewAsync(CancellationToken cancellationToken = default);
}
