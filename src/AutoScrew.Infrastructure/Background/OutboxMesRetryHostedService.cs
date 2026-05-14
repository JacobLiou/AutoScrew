using System.Text.Json;
using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Background;

public sealed class OutboxMesRetryHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxMesRetryHostedService> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox retry loop error.");
            }

            await Task.Delay(TimeSpan.FromSeconds(8), stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        var mes = scope.ServiceProvider.GetRequiredService<IMesClient>();

        await using var db = await factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        var pending = await db.OutboxUploads
            .Where(x => x.SentAt == null)
            .OrderBy(x => x.Id)
            .Take(20)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        foreach (var row in pending)
        {
            LockJobResultPayload? payload;
            try
            {
                payload = JsonSerializer.Deserialize<LockJobResultPayload>(row.PayloadJson, Json);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bad outbox payload id {Id}", row.Id);
                row.LastError = "Deserialize failed";
                row.RetryCount++;
                continue;
            }

            if (payload is null)
                continue;

            var result = await mes.UploadResultAsync(payload, ct).ConfigureAwait(false);
            if (result.Accepted)
            {
                row.SentAt = DateTimeOffset.UtcNow;
                row.LastError = null;
            }
            else
            {
                row.RetryCount++;
                row.LastError = result.Message;
            }
        }

        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
