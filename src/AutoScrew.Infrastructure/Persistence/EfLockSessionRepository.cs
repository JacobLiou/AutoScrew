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

    public async Task<long> SaveLockRecordAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        var record = new LockRecordEntity
        {
            SerialNumber = payload.SerialNumber,
            PartNumber = payload.PartNumber,
            StationId = payload.StationId,
            OperatorId = payload.OperatorId,
            StartedAt = payload.StartedAt,
            EndedAt = payload.CompletedAt,
            Result = payload.OverallResult,
            IsRework = payload.IsRework,
        };

        db.LockRecords.Add(record);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        foreach (var screw in payload.Screws)
        {
            db.ScrewDetails.Add(new ScrewDetailEntity
            {
                LockRecordId = record.Id,
                PositionIndex = screw.PositionIndex,
                PartNo = null,
                FinalTorqueNm = screw.FinalTorqueNm,
                FinalAngleDeg = screw.FinalAngleDeg,
                CurvePath = screw.CurveRelativePath,
                Result = screw.Result,
            });
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return record.Id;
    }
}
