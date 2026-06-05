using System.Text.Json;
using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Audit;

internal sealed class JsonlUserAuditStore(ILogger<JsonlUserAuditStore> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public void Append(string auditDirectory, UserAuditEntry entry)
    {
        Directory.CreateDirectory(auditDirectory);
        var fileName = $"user-audit-{entry.Timestamp:yyyy-MM-dd}.jsonl";
        var path = Path.Combine(auditDirectory, fileName);
        var line = JsonSerializer.Serialize(entry, JsonOptions) + Environment.NewLine;
        File.AppendAllText(path, line);
    }

    public void TryAppend(string auditDirectory, UserAuditEntry entry)
    {
        try
        {
            Append(auditDirectory, entry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to append user audit JSONL to {Directory}", auditDirectory);
        }
    }
}
