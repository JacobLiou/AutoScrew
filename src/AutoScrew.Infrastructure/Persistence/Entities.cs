namespace AutoScrew.Infrastructure.Persistence;

public sealed class LockRecordEntity
{
    public long Id { get; set; }

    public string SerialNumber { get; set; } = "";

    public string PartNumber { get; set; } = "";

    public string StationId { get; set; } = "";

    public string OperatorId { get; set; } = "";

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset? EndedAt { get; set; }

    public string Result { get; set; } = "";

    public bool IsRework { get; set; }
}

public sealed class ScrewDetailEntity
{
    public long Id { get; set; }

    public long LockRecordId { get; set; }

    public int PositionIndex { get; set; }

    public string? PartNo { get; set; }

    public double? FinalTorqueNm { get; set; }

    public double? FinalAngleDeg { get; set; }

    public string? CurvePath { get; set; }

    public string Result { get; set; } = "";
}

public sealed class ErrorLogEntity
{
    public long Id { get; set; }

    public long? LockRecordId { get; set; }

    public string ErrorCode { get; set; } = "";

    public string Message { get; set; } = "";

    public string? ResolveBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? ResolveTime { get; set; }
}

public sealed class OutboxUploadEntity
{
    public long Id { get; set; }

    public string IdempotencyKey { get; set; } = "";

    public string PayloadJson { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? SentAt { get; set; }

    public int RetryCount { get; set; }

    public string? LastError { get; set; }
}

public sealed class SessionCheckpointEntity
{
    public int Id { get; set; }

    public string PayloadJson { get; set; } = "";

    public DateTimeOffset UpdatedAt { get; set; }
}
