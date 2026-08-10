using AutoScrew.Application.Abstractions;
using AutoScrew.Domain.Session;
using AutoScrew.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoScrew.Tests;

public sealed class LockRecordRepositoryTests
{
    [Fact]
    public async Task SaveLockRecordAsync_PersistsHeaderAndScrewDetails()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var db = new AppDbContext(options))
        {
            await db.Database.EnsureCreatedAsync();
        }

        var factory = new TestDbContextFactory(options);
        var repo = new EfLockSessionRepository(factory);
        var payload = new LockJobResultPayload
        {
            SerialNumber = "SN-DB-001",
            PartNumber = "PN-001",
            StationId = "ST-01",
            HostIp = "10.0.0.8",
            HostMac = "AA-BB-CC-DD-EE-FF",
            OperatorId = "operator",
            StartedAt = DateTimeOffset.Parse("2026-06-11T08:00:00Z"),
            CompletedAt = DateTimeOffset.Parse("2026-06-11T08:05:00Z"),
            OverallResult = "OK",
            Screws =
            [
                new ScrewResultDto { PositionIndex = 1, Result = "OK", FinalTorqueNm = 0.35 },
                new ScrewResultDto { PositionIndex = 2, Result = "NG", ErrorCode = "FLOAT_002" },
            ],
        };

        var id = await repo.SaveLockRecordAsync(payload);

        await using var verify = new AppDbContext(options);
        var record = await verify.LockRecords.FindAsync(id);
        Assert.NotNull(record);
        Assert.Equal("SN-DB-001", record!.SerialNumber);
        Assert.Equal("OK", record.Result);
        Assert.Equal("10.0.0.8", record.HostIp);
        Assert.Equal("AA-BB-CC-DD-EE-FF", record.HostMac);

        var details = verify.ScrewDetails.Where(d => d.LockRecordId == id).OrderBy(d => d.PositionIndex).ToList();
        Assert.Equal(2, details.Count);
        Assert.Equal("NG", details[1].Result);
    }

    [Fact]
    public async Task LockHistoryQuery_FiltersAndSummarizes()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var db = new AppDbContext(options))
            await db.Database.EnsureCreatedAsync();

        var factory = new TestDbContextFactory(options);
        var repo = new EfLockSessionRepository(factory);
        var query = new EfLockHistoryQuery(factory);

        await repo.SaveLockRecordAsync(new LockJobResultPayload
        {
            SerialNumber = "SN-A",
            PartNumber = "PN-X",
            StationId = "ST",
            HostMac = "AA-BB-CC-DD-EE-01",
            OperatorId = "op",
            StartedAt = DateTimeOffset.Parse("2026-08-10T02:00:00Z"),
            CompletedAt = DateTimeOffset.Parse("2026-08-10T02:10:00Z"),
            OverallResult = "OK",
            Screws = [new ScrewResultDto { PositionIndex = 1, Result = "OK" }],
        });
        await repo.SaveLockRecordAsync(new LockJobResultPayload
        {
            SerialNumber = "SN-B",
            PartNumber = "PN-Y",
            StationId = "ST",
            HostMac = "AA-BB-CC-DD-EE-01",
            OperatorId = "op",
            StartedAt = DateTimeOffset.Parse("2026-08-10T03:00:00Z"),
            CompletedAt = DateTimeOffset.Parse("2026-08-10T03:10:00Z"),
            OverallResult = "NG",
            Screws =
            [
                new ScrewResultDto { PositionIndex = 1, Result = "OK" },
                new ScrewResultDto { PositionIndex = 2, Result = "NG" },
            ],
        });

        var from = DateTimeOffset.Parse("2026-08-10T00:00:00Z");
        var to = DateTimeOffset.Parse("2026-08-11T00:00:00Z");
        var summary = await query.GetSummaryAsync(from, to);
        Assert.Equal(2, summary.JobTotal);
        Assert.Equal(1, summary.JobOk);
        Assert.Equal(1, summary.JobNg);
        Assert.Equal(3, summary.ScrewTotal);
        Assert.Equal(2, summary.ScrewOk);
        Assert.Equal(1, summary.ScrewNg);

        var page = await query.QueryJobsAsync(new LockHistoryJobFilter(from, to, "SN-B", null, "NG", 0, 50));
        Assert.Equal(1, page.TotalCount);
        Assert.Equal("SN-B", page.Items[0].SerialNumber);
        Assert.Equal(2, page.Items[0].ScrewCount);

        var screws = await query.GetJobScrewsAsync(page.Items[0].Id);
        Assert.Equal(2, screws.Count);
        Assert.Equal("NG", screws[1].Result);
    }

    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }
}
