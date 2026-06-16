using Microsoft.EntityFrameworkCore;

namespace AutoScrew.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LockRecordEntity> LockRecords => Set<LockRecordEntity>();

    public DbSet<ScrewDetailEntity> ScrewDetails => Set<ScrewDetailEntity>();

    public DbSet<ErrorLogEntity> ErrorLogs => Set<ErrorLogEntity>();

    public DbSet<OutboxUploadEntity> OutboxUploads => Set<OutboxUploadEntity>();

    public DbSet<SessionCheckpointEntity> SessionCheckpoints => Set<SessionCheckpointEntity>();

    public DbSet<UserAuditLogEntity> UserAuditLogs => Set<UserAuditLogEntity>();

    public DbSet<ProductTemplateSyncEntity> ProductTemplateSyncs => Set<ProductTemplateSyncEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LockRecordEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.SerialNumber);
            e.HasIndex(x => x.StartedAt);
        });

        modelBuilder.Entity<ScrewDetailEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.LockRecordId);
        });

        modelBuilder.Entity<ErrorLogEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.LockRecordId);
        });

        modelBuilder.Entity<OutboxUploadEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.SentAt);
            e.HasIndex(x => x.IdempotencyKey);
        });

        modelBuilder.Entity<SessionCheckpointEntity>(e => { e.HasKey(x => x.Id); });

        modelBuilder.Entity<UserAuditLogEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Timestamp);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.Category);
            e.HasIndex(x => x.Action);
        });

        modelBuilder.Entity<ProductTemplateSyncEntity>(e =>
        {
            e.HasKey(x => x.PartNumber);
            e.HasIndex(x => x.SyncState);
        });
    }
}
