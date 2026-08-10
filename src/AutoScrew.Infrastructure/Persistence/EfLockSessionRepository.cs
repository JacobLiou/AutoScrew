using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Domain.Session;
using Microsoft.EntityFrameworkCore;

namespace AutoScrew.Infrastructure.Persistence;

public sealed class EfLockSessionRepository(IDbContextFactory<AppDbContext> factory) : ILockSessionRepository
{
    private const int LegacyCheckpointId = 1;

    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SaveJobMemoryAsync(
        SessionCheckpointData data,
        SnJobMemoryStatus status,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (string.IsNullOrWhiteSpace(data.SerialNumber))
            throw new ArgumentException("SerialNumber is required.", nameof(data));

        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await EnsureLegacyMigratedAsync(db, cancellationToken).ConfigureAwait(false);

        var sn = data.SerialNumber.Trim();
        var json = JsonSerializer.Serialize(data, Json);
        var now = DateTimeOffset.UtcNow;
        var row = await db.SnJobMemories.FindAsync([sn], cancellationToken).ConfigureAwait(false);
        if (row is null)
        {
            db.SnJobMemories.Add(new SnJobMemoryEntity
            {
                SerialNumber = sn,
                PartNumber = data.PartNumber ?? "",
                Status = (int)status,
                PayloadJson = json,
                UpdatedAt = now,
                CompletedAt = status == SnJobMemoryStatus.Completed ? now : null,
            });
        }
        else
        {
            row.PartNumber = data.PartNumber ?? "";
            row.Status = (int)status;
            row.PayloadJson = json;
            row.UpdatedAt = now;
            if (status == SnJobMemoryStatus.Completed)
                row.CompletedAt ??= now;
            else
                row.CompletedAt = null;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<SessionCheckpointData?> LoadJobMemoryAsync(
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return null;

        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await EnsureLegacyMigratedAsync(db, cancellationToken).ConfigureAwait(false);

        var row = await db.SnJobMemories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SerialNumber == serialNumber.Trim(), cancellationToken)
            .ConfigureAwait(false);
        return Deserialize(row?.PayloadJson);
    }

    public async Task<SnJobMemoryStatus?> GetJobMemoryStatusAsync(
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return null;

        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await EnsureLegacyMigratedAsync(db, cancellationToken).ConfigureAwait(false);

        var row = await db.SnJobMemories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.SerialNumber == serialNumber.Trim(), cancellationToken)
            .ConfigureAwait(false);
        return row is null ? null : (SnJobMemoryStatus)row.Status;
    }

    public async Task<SessionCheckpointData?> LoadLatestRestorableAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await EnsureLegacyMigratedAsync(db, cancellationToken).ConfigureAwait(false);

        var rows = (await db.SnJobMemories.AsNoTracking()
                .Where(x => x.Status != (int)SnJobMemoryStatus.Completed)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false))
            .OrderByDescending(x => x.UpdatedAt)
            .ToList();

        foreach (var row in rows)
        {
            var data = Deserialize(row.PayloadJson);
            if (data is not null && IsRestorablePhase(data.Phase))
                return data;
        }

        return null;
    }

    public async Task MarkJobCompletedAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return;

        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        await EnsureLegacyMigratedAsync(db, cancellationToken).ConfigureAwait(false);

        var sn = serialNumber.Trim();
        var row = await db.SnJobMemories.FindAsync([sn], cancellationToken).ConfigureAwait(false);
        var now = DateTimeOffset.UtcNow;
        if (row is null)
        {
            db.SnJobMemories.Add(new SnJobMemoryEntity
            {
                SerialNumber = sn,
                PartNumber = "",
                Status = (int)SnJobMemoryStatus.Completed,
                PayloadJson = "{}",
                UpdatedAt = now,
                CompletedAt = now,
            });
        }
        else
        {
            row.Status = (int)SnJobMemoryStatus.Completed;
            row.UpdatedAt = now;
            row.CompletedAt = now;
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveJobMemoryAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return;

        await using var db = await factory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        var row = await db.SnJobMemories.FindAsync([serialNumber.Trim()], cancellationToken).ConfigureAwait(false);
        if (row is null)
            return;

        db.SnJobMemories.Remove(row);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task SaveCheckpointAsync(SessionCheckpointData data, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        var status = data.Phase == JobSessionPhase.NgLocked
            ? SnJobMemoryStatus.NgPaused
            : SnJobMemoryStatus.InProgress;
        return SaveJobMemoryAsync(data, status, cancellationToken);
    }

    public Task<SessionCheckpointData?> LoadLatestCheckpointAsync(CancellationToken cancellationToken = default) =>
        LoadLatestRestorableAsync(cancellationToken);

    public Task ClearCheckpointAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

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

    private async Task EnsureLegacyMigratedAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.SnJobMemories.AsNoTracking().AnyAsync(cancellationToken).ConfigureAwait(false))
            return;

        var legacy = await db.SessionCheckpoints.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == LegacyCheckpointId, cancellationToken)
            .ConfigureAwait(false);
        if (legacy is null || string.IsNullOrWhiteSpace(legacy.PayloadJson))
            return;

        var data = Deserialize(legacy.PayloadJson);
        if (data is null || string.IsNullOrWhiteSpace(data.SerialNumber))
            return;

        var status = data.Phase == JobSessionPhase.NgLocked
            ? SnJobMemoryStatus.NgPaused
            : data.Phase == JobSessionPhase.Completed
                ? SnJobMemoryStatus.Completed
                : SnJobMemoryStatus.InProgress;

        db.SnJobMemories.Add(new SnJobMemoryEntity
        {
            SerialNumber = data.SerialNumber.Trim(),
            PartNumber = data.PartNumber ?? "",
            Status = (int)status,
            PayloadJson = legacy.PayloadJson,
            UpdatedAt = legacy.UpdatedAt == default ? DateTimeOffset.UtcNow : legacy.UpdatedAt,
            CompletedAt = status == SnJobMemoryStatus.Completed ? DateTimeOffset.UtcNow : null,
        });

        var tracked = await db.SessionCheckpoints.FindAsync([LegacyCheckpointId], cancellationToken)
            .ConfigureAwait(false);
        if (tracked is not null)
            db.SessionCheckpoints.Remove(tracked);

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static SessionCheckpointData? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
            return null;

        try
        {
            return JsonSerializer.Deserialize<SessionCheckpointData>(json, Json);
        }
        catch
        {
            return null;
        }
    }

    private static bool IsRestorablePhase(JobSessionPhase phase) =>
        phase is JobSessionPhase.Running or JobSessionPhase.AwaitFlip or JobSessionPhase.NgLocked;
}
