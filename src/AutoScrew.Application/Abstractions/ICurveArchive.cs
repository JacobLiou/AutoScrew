using AutoScrew.Domain.Curves;

namespace AutoScrew.Application.Abstractions;

public interface ICurveArchive
{
    Task<string> SaveCurveCsvAsync(
        string serialNumber,
        int positionIndex,
        IReadOnlyList<TorqueAngleSample> samples,
        CancellationToken cancellationToken = default);

    Task SaveLockLogJsonAsync(string serialNumber, string json, CancellationToken cancellationToken = default);
}
