using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Persistence;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplateSyncRepositoryTests
{
    [Fact]
    public async Task UpsertAsync_InsertsAndUpdatesRecord()
    {
        var (factory, connection) = await CreateFactoryAsync();
        await using (connection)
        {
            var repo = new EfProductTemplateSyncRepository(factory);
            var record = new ProductTemplateSyncRecord(
                "PN-1",
                "PN-1/PN-1.product-template.json",
                ProductTemplateSyncState.PendingUpload,
                "HASH1",
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null);

            await repo.UpsertAsync(record);

            var loaded = await repo.GetAsync("PN-1");
            Assert.NotNull(loaded);
            Assert.Equal(ProductTemplateSyncState.PendingUpload, loaded!.SyncState);
            Assert.Equal("HASH1", loaded.LocalFileHash);

            record = record with { SyncState = ProductTemplateSyncState.Synced, LocalFileHash = "HASH2" };
            await repo.UpsertAsync(record);

            loaded = await repo.GetAsync("PN-1");
            Assert.Equal(ProductTemplateSyncState.Synced, loaded!.SyncState);
            Assert.Equal("HASH2", loaded.LocalFileHash);
        }
    }

    [Fact]
    public async Task ListAllAsync_ReturnsAllRecords()
    {
        var (factory, connection) = await CreateFactoryAsync();
        await using (connection)
        {
            var repo = new EfProductTemplateSyncRepository(factory);
            await repo.UpsertAsync(new ProductTemplateSyncRecord(
                "PN-A",
                "PN-A/PN-A.product-template.json",
                ProductTemplateSyncState.PendingUpload,
                "H1",
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null));
            await repo.UpsertAsync(new ProductTemplateSyncRecord(
                "PN-B",
                "PN-B/PN-B.product-template.json",
                ProductTemplateSyncState.Synced,
                "H2",
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                null));

            var all = await repo.ListAllAsync();
            Assert.Equal(2, all.Count);
            Assert.Contains(all, x => x.PartNumber == "PN-A");
            Assert.Contains(all, x => x.PartNumber == "PN-B");
        }
    }

    private static async Task<(IDbContextFactory<AppDbContext> Factory, SqliteConnection Connection)> CreateFactoryAsync()
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

        return (new TestDbContextFactory(options), connection);
    }

    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }
}
