using System.Collections.ObjectModel;

namespace AutoScrew.Application.Abstractions;

public sealed record OperationActivityLogEntry(
    DateTimeOffset Timestamp,
    string Message,
    string? SerialNumber = null)
{
    public string DisplayLine => $"[{Timestamp.LocalDateTime:HH:mm:ss}] {Message}";
}

public interface IOperationActivityLogService
{
    ReadOnlyObservableCollection<OperationActivityLogEntry> Entries { get; }

    void Append(string message, string? serialNumber = null);

    /// <summary>仅清空 UI 内存缓冲，不影响已落盘 JSONL。</summary>
    void ClearRecent();
}
