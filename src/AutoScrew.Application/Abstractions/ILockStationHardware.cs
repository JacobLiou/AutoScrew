using AutoScrew.Domain.Curves;

namespace AutoScrew.Application.Abstractions;

public interface ILockStationHardware
{
    /// <summary>控制器侧最近一次拧紧结果；仿真驱动为 null。</summary>
    LockHardwareOutcome? LastOutcome { get; }

    IAsyncEnumerable<TorqueAngleSample> RunTighteningAsync(
        TighteningContext context,
        CancellationToken cancellationToken = default);

    Task PickScrewAsync(CancellationToken cancellationToken = default);
}
