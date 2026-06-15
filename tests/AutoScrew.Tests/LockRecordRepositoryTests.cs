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

        var details = verify.ScrewDetails.Where(d => d.LockRecordId == id).OrderBy(d => d.PositionIndex).ToList();
        Assert.Equal(2, details.Count);
        Assert.Equal("NG", details[1].Result);
    }

    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }
}
