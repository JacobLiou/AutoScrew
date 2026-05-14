using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;

namespace AutoScrew.Application.Abstractions;

public interface ILockSessionRepository
{
    Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default);

    Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default);

    Task ClearCheckpointAsync(CancellationToken cancellationToken = default);
}

public sealed record SessionCheckpointData(
    JobSessionPhase Phase,
    string SerialNumber,
    string PartNumber,
    int CurrentScrewIndex,
    List<StationScrewState> ScrewStates,
    DateTimeOffset UpdatedAt);
