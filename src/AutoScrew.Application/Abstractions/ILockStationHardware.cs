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

    /// <summary>作业开始：#300 手动 + #303 激活工艺库顺序；仿真可为空操作。</summary>
    Task PrepareForJobAsync(CancellationToken cancellationToken = default, int? sequenceId = null);

    /// <summary>清除控制器故障/错误（如 IEMD ClearErrors）；仿真为空操作。无连接时不应抛出让解锁失败。</summary>
    Task ClearErrorsAsync(CancellationToken cancellationToken = default);
}
