namespace UDL.Delta.IemdSd.Protocol;

/// <summary>#752 error / exception history entry (7 words from 0xD2).</summary>
public sealed class ErrorReportEntry
{
    public uint ReportId { get; init; }

    public DateTime? Timestamp { get; init; }

    /// <summary>AL/NG numeric code (display as AL#### / NG#### by range).</summary>
    public ushort Code { get; init; }
}

/// <summary>#753 warning history entry (7 words from 0xD2).</summary>
public sealed class WarningReportEntry
{
    public uint ReportId { get; init; }

    public DateTime? Timestamp { get; init; }

    public ushort Code { get; init; }
}

/// <summary>#754 button history entry (12 words from 0xD2).</summary>
public sealed class ButtonReportEntry
{
    public uint ReportId { get; init; }

    public DateTime? Timestamp { get; init; }

    public ushort ButtonId { get; init; }

    public uint ValueBefore { get; init; }

    public uint ValueAfter { get; init; }

    /// <summary>Permissions account (same enum as production UserId).</summary>
    public ushort UserId { get; init; }
}

/// <summary>Latest IDs / counts for paging device history buffers.</summary>
public sealed class DeviceHistoryCounts
{
    /// <summary>Latest production report ID (0x6B/0x6C). 0 = empty.</summary>
    public uint ProductionLatestId { get; init; }

    /// <summary>Error history latest ID or count (0x69).</summary>
    public ushort ErrorLatestId { get; init; }

    /// <summary>Warning history latest ID or count (0x6A).</summary>
    public ushort WarningLatestId { get; init; }

    /// <summary>Latest button report ID (0x6D/0x6E).</summary>
    public uint ButtonLatestId { get; init; }
}
