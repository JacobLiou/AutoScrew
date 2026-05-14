using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AutoScrew.Infrastructure.Persistence;

public sealed class EfOutboundMesQueue(IDbContextFactory<AppDbContext> factory) : IOutboundMesQueue
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public async Task EnqueueAsync(LockJobResultPayload payload, string? failureReason, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var key = $"{payload.SerialNumber}:{payload.CompletedAt:O}";
        var json = JsonSerializer.Serialize(payload, Json);
        db.OutboxUploads.Add(new OutboxUploadEntity
        {
            IdempotencyKey = key,
            PayloadJson = json,
            CreatedAt = DateTimeOffset.UtcNow,
            LastError = failureReason
        });
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
