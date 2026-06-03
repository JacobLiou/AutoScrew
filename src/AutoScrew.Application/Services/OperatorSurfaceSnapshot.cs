using AutoScrew.Domain.Models;

namespace AutoScrew.Application.Services;

/// <summary>作业台进度树只读快照（单面）。</summary>
public sealed record OperatorSurfaceSnapshot(
    string SurfaceId,
    string Name,
    int Order,
    SurfaceProgressState ProgressState,
    IReadOnlyList<int> ScrewLocalIndices,
    IReadOnlyList<StationScrewState> ScrewStates);
