using AutoScrew.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoScrew.Infrastructure.Persistence;

public sealed class EfLockHistoryQuery(IDbContextFactory<AppDbContext> factory) : ILockHistoryQuery
{
    public async Task<LockHistorySummary> GetSummaryAsync(
        DateTimeOffset fromInclusive,
        DateTimeOffset toExclusive,
        CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        // SQLite provider cannot translate DateTimeOffset comparisons; filter in memory.
        var jobs = (await db.LockRecords.AsNoTracking()
                .Select(x => new { x.Id, x.Result, x.StartedAt })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false))
            .Where(x => x.StartedAt >= fromInclusive && x.StartedAt < toExclusive)
            .ToList();

        var jobIds = jobs.Select(j => j.Id).ToList();
        var screws = jobIds.Count == 0
            ? []
            : await db.ScrewDetails.AsNoTracking()
                .Where(s => jobIds.Contains(s.LockRecordId))
                .Select(s => s.Result)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

        static bool IsOk(string? r) =>
            string.Equals(r, "OK", StringComparison.OrdinalIgnoreCase);

        static bool IsNg(string? r) =>
            string.Equals(r, "NG", StringComparison.OrdinalIgnoreCase);

        return new LockHistorySummary(
            JobTotal: jobs.Count,
            JobOk: jobs.Count(j => IsOk(j.Result)),
            JobNg: jobs.Count(j => IsNg(j.Result)),
            ScrewTotal: screws.Count,
            ScrewOk: screws.Count(IsOk),
            ScrewNg: screws.Count(IsNg));
    }

    public async Task<LockHistoryJobPage> QueryJobsAsync(
        LockHistoryJobFilter filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var take = filter.Take <= 0 ? 50 : Math.Min(filter.Take, 500);
        var skip = Math.Max(0, filter.Skip);

        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var matched = (await db.LockRecords.AsNoTracking()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false))
            .Where(x => x.StartedAt >= filter.FromInclusive && x.StartedAt < filter.ToExclusive);

        if (!string.IsNullOrWhiteSpace(filter.SerialNumber))
        {
            var sn = filter.SerialNumber.Trim();
            matched = matched.Where(x => x.SerialNumber.Contains(sn, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.PartNumber))
        {
            var pn = filter.PartNumber.Trim();
            matched = matched.Where(x => x.PartNumber.Contains(pn, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.Result))
        {
            var result = filter.Result.Trim();
            matched = matched.Where(x => string.Equals(x.Result, result, StringComparison.OrdinalIgnoreCase));
        }

        var ordered = matched.OrderByDescending(x => x.StartedAt).ToList();
        var total = ordered.Count;
        var page = ordered.Skip(skip).Take(take).ToList();

        var pageIds = page.Select(p => p.Id).ToList();
        var screwCounts = pageIds.Count == 0
            ? new Dictionary<long, int>()
            : await db.ScrewDetails.AsNoTracking()
                .Where(s => pageIds.Contains(s.LockRecordId))
                .GroupBy(s => s.LockRecordId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken)
                .ConfigureAwait(false);

        var rows = page.Select(r => new LockHistoryJobRow(
            r.Id,
            r.SerialNumber,
            r.PartNumber,
            r.StationId,
            r.HostIp,
            r.HostMac,
            r.OperatorId,
            r.StartedAt,
            r.EndedAt,
            r.Result,
            r.IsRework,
            screwCounts.TryGetValue(r.Id, out var c) ? c : 0)).ToList();

        return new LockHistoryJobPage(rows, total);
    }

    public async Task<IReadOnlyList<LockHistoryScrewRow>> GetJobScrewsAsync(
        long lockRecordId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var screws = await db.ScrewDetails.AsNoTracking()
            .Where(s => s.LockRecordId == lockRecordId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return screws
            .OrderBy(s => s.PositionIndex)
            .Select(s => new LockHistoryScrewRow(
                s.Id,
                s.PositionIndex,
                s.PartNo,
                s.FinalTorqueNm,
                s.FinalAngleDeg,
                s.CurvePath,
                s.Result))
            .ToList();
    }
}
