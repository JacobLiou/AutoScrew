using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;

namespace AutoScrew.Application.Abstractions;

public enum SnJobMemoryStatus
{
    InProgress = 0,
    NgPaused = 1,
    Completed = 2,
}

public interface ILockSessionRepository
{
    /// <summary>按 SN upsert 作业记忆（未完成 / NG）。</summary>
    Task SaveJobMemoryAsync(
        SessionCheckpointData data,
        SnJobMemoryStatus status,
        CancellationToken cancellationToken = default);

    Task<SessionCheckpointData?> LoadJobMemoryAsync(
        string serialNumber,
        CancellationToken cancellationToken = default);

    Task<SnJobMemoryStatus?> GetJobMemoryStatusAsync(
        string serialNumber,
        CancellationToken cancellationToken = default);

    /// <summary>最近一条可恢复记忆（非 Completed，且 phase 可恢复）。</summary>
    Task<SessionCheckpointData?> LoadLatestRestorableAsync(CancellationToken cancellationToken = default);

    /// <summary>全部成功：标记 Completed，保留 payload。</summary>
    Task MarkJobCompletedAsync(string serialNumber, CancellationToken cancellationToken = default);

    /// <summary>删除指定 SN 记忆（如恢复失败）。</summary>
    Task RemoveJobMemoryAsync(string serialNumber, CancellationToken cancellationToken = default);

    /// <summary>兼容：按 payload 内 SN 保存为 InProgress/NgPaused。</summary>
    Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default);

    /// <summary>兼容：等同 LoadLatestRestorableAsync。</summary>
    Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default);

    /// <summary>兼容：不再清空全部记忆（成功用 MarkJobCompleted）。</summary>
    Task ClearCheckpointAsync(CancellationToken cancellationToken = default);

    Task<long> SaveLockRecordAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default);
}

public sealed record SessionCheckpointData(
    JobSessionPhase Phase,
    string SerialNumber,
    string PartNumber,
    int ActiveSurfaceOrdinal,
    int CurrentScrewIndex,
    List<SurfaceCheckpointSurface> Surfaces,
    DateTimeOffset UpdatedAt);

/// <summary>Checkpoint 单面螺钉状态（含 surface_id 草案字段）。</summary>
public sealed record SurfaceCheckpointSurface(
    string SurfaceId,
    SurfaceProgressState ProgressState,
    List<StationScrewState> ScrewStates);
