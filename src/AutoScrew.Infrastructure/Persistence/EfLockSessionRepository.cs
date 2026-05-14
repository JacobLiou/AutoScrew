using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Domain.Session;
using Microsoft.EntityFrameworkCore;

namespace AutoScrew.Infrastructure.Persistence;

public sealed class EfLockSessionRepository(IDbContextFactory<AppDbContext> factory) : ILockSessionRepository
{
    private const int CheckpointId = 1;

    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var json = JsonSerializer.Serialize(data, Json);
        var row = await db.SessionCheckpoints.FindAsync([CheckpointId], cancellationToken).ConfigureAwait(false);
        if (row is null)
        {
            db.SessionCheckpoints.Add(new SessionCheckpointEntity
            {
                Id = CheckpointId,
                PayloadJson = json,
                UpdatedAt = DateTimeOffset.UtcNow
            });
        }
        else
        {
            row.PayloadJson = json;
            row.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var row = await db.SessionCheckpoints.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == CheckpointId, cancellationToken)
            .ConfigureAwait(false);
        if (row is null || string.IsNullOrWhiteSpace(row.PayloadJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<SessionCheckpointData>(row.PayloadJson, Json);
        }
        catch
        {
            return null;
        }
    }

    public async Task ClearCheckpointAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var row = await db.SessionCheckpoints.FindAsync([CheckpointId], cancellationToken).ConfigureAwait(false);
        if (row is not null)
        {
            db.SessionCheckpoints.Remove(row);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
