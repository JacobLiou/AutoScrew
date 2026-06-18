using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoScrew.Infrastructure.Templates;

public sealed class EfProductTemplateSyncRepository(IDbContextFactory<AppDbContext> factory)
    : IProductTemplateSyncRepository
{
    public async Task<ProductTemplateSyncRecord?> GetAsync(string partNumber, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var entity = await db.ProductTemplateSyncs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PartNumber == partNumber, cancellationToken)
            .ConfigureAwait(false);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<ProductTemplateSyncRecord>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var entities = await db.ProductTemplateSyncs
            .AsNoTracking()
            .OrderBy(x => x.PartNumber)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return entities.Select(Map).ToList();
    }

    public async Task UpsertAsync(ProductTemplateSyncRecord record, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var entity = await db.ProductTemplateSyncs
            .FirstOrDefaultAsync(x => x.PartNumber == record.PartNumber, cancellationToken)
            .ConfigureAwait(false);

        if (entity is null)
        {
            entity = new ProductTemplateSyncEntity { PartNumber = record.PartNumber };
            db.ProductTemplateSyncs.Add(entity);
        }

        entity.LocalRelativePath = record.LocalRelativePath;
        entity.SyncState = (int)record.SyncState;
        entity.LocalFileHash = record.LocalFileHash;
        entity.LocalModifiedUtc = record.LocalModifiedUtc;
        entity.LastMesPullUtc = record.LastMesPullUtc;
        entity.LastMesPushUtc = record.LastMesPushUtc;
        entity.MesRevision = record.MesRevision;
        entity.LastError = record.LastError;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static ProductTemplateSyncRecord Map(ProductTemplateSyncEntity e) =>
        new(
            e.PartNumber,
            e.LocalRelativePath,
            (ProductTemplateSyncState)e.SyncState,
            e.LocalFileHash,
            e.LocalModifiedUtc,
            e.LastMesPullUtc,
            e.LastMesPushUtc,
            e.MesRevision,
            e.LastError);
}
