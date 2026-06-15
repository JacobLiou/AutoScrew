using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;

namespace AutoScrew.Application.Abstractions;

public interface ILockSessionRepository
{
    Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default);

    Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default);

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
