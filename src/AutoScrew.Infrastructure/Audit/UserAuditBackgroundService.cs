using System.Threading.Channels;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Audit;

internal sealed class UserAuditBackgroundService : BackgroundService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly JsonlUserAuditStore _jsonlStore;
    private readonly IOptions<AutoScrewAppOptions> _options;
    private readonly ILogger<UserAuditBackgroundService> _logger;
    private readonly Channel<UserAuditEntry> _channel;

    public UserAuditBackgroundService(
        UserAuditService auditService,
        IDbContextFactory<AppDbContext> dbFactory,
        JsonlUserAuditStore jsonlStore,
        IOptions<AutoScrewAppOptions> options,
        ILogger<UserAuditBackgroundService> logger)
    {
        _channel = auditService.AuditChannel;
        _dbFactory = dbFactory;
        _jsonlStore = jsonlStore;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var entry in _channel.Reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                await PersistAsync(entry, stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist user audit entry {Action}", entry.Action);
            }
        }
    }

    private async Task PersistAsync(UserAuditEntry entry, CancellationToken cancellationToken)
    {
        var opts = _options.Value;
        if (!opts.AuditLogEnabled)
            return;

        var auditDir = ResolveAuditDirectory(opts);
        _jsonlStore.TryAppend(auditDir, entry);

        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);
        db.UserAuditLogs.Add(MapToEntity(entry));
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "UserAudit {Category} {Action} user={UserId} role={Role} station={StationId} success={Success}",
            entry.Category,
            entry.Action,
            entry.UserId,
            entry.Role,
            entry.StationId,
            entry.Success);
    }

    internal static string ResolveAuditDirectory(AutoScrewAppOptions opts)
    {
        if (!string.IsNullOrWhiteSpace(opts.AuditDirectory))
            return opts.AuditDirectory;

        var dataRoot = string.IsNullOrWhiteSpace(opts.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : opts.DataDirectory;

        return Path.Combine(dataRoot, "audit");
    }

    private static UserAuditLogEntity MapToEntity(UserAuditEntry entry) =>
        new()
        {
            Timestamp = entry.Timestamp,
            StationId = entry.StationId,
            UserId = entry.UserId,
            DisplayName = entry.DisplayName,
            Role = (int)entry.Role,
            Category = (int)entry.Category,
            Action = entry.Action,
            Target = entry.Target,
            Detail = entry.Detail,
            Success = entry.Success,
            SerialNumber = entry.SerialNumber,
        };
}
