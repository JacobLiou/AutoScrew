namespace AutoScrew.Application.Abstractions;

public interface ILockHistoryQuery
{
    Task<LockHistorySummary> GetSummaryAsync(
        DateTimeOffset fromInclusive,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken = default);

    Task<LockHistoryJobPage> QueryJobsAsync(
        LockHistoryJobFilter filter,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LockHistoryScrewRow>> GetJobScrewsAsync(
        long lockRecordId,
        CancellationToken cancellationToken = default);
}

public sealed record LockHistoryJobFilter(
    DateTimeOffset FromInclusive,
    DateTimeOffset ToExclusive,
    string? SerialNumber,
    string? PartNumber,
    string? Result,
    int Skip,
    int Take);

public sealed record LockHistorySummary(
    int JobTotal,
    int JobOk,
    int JobNg,
    int ScrewTotal,
    int ScrewOk,
    int ScrewNg);

public sealed record LockHistoryJobPage(
    IReadOnlyList<LockHistoryJobRow> Items,
    int TotalCount);

public sealed record LockHistoryJobRow(
    long Id,
    string SerialNumber,
    string PartNumber,
    string StationId,
    string? HostIp,
    string? HostMac,
    string OperatorId,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    string Result,
    bool IsRework,
    int ScrewCount);

public sealed record LockHistoryScrewRow(
    long Id,
    int PositionIndex,
    string? PartNo,
    double? FinalTorqueNm,
    double? FinalAngleDeg,
    string? CurvePath,
    string Result);
