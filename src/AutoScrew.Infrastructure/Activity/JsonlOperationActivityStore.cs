using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Activity;

internal sealed record OperationActivityLogLine(
    DateTimeOffset Timestamp,
    string StationId,
    string? SerialNumber,
    string Message);

internal sealed class JsonlOperationActivityStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };
    private readonly ILogger _logger;
    private readonly object _appendLock = new();

    public JsonlOperationActivityStore(ILogger<OperationActivityLogService> logger) =>
        _logger = logger;

    public void Append(string activityDirectory, OperationActivityLogLine line)
    {
        Directory.CreateDirectory(activityDirectory);
        var fileName = $"operation-activity-{line.Timestamp:yyyy-MM-dd}.jsonl";
        var path = Path.Combine(activityDirectory, fileName);
        var json = JsonSerializer.Serialize(line, JsonOptions) + Environment.NewLine;
        lock (_appendLock)
            File.AppendAllText(path, json);
    }

    public void TryAppend(string activityDirectory, OperationActivityLogLine line)
    {
        try
        {
            Append(activityDirectory, line);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to append operation activity JSONL to {Directory}", activityDirectory);
        }
    }
}
