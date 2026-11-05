using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Application.Services;

public sealed class ProductTemplateMesSyncService : IProductTemplateMesSyncService
{
    private readonly IProductTemplateSyncRepository _syncRepository;
    private readonly IProductTemplateLocalStore _localStore;
    private readonly IMesTemplateUploadService _uploadService;
    private readonly IProductTemplateLanShare _lanShare;
    private readonly IMesSettingsService _mesSettings;
    private readonly ILogger<ProductTemplateMesSyncService> _logger;

    public ProductTemplateMesSyncService(
        IProductTemplateSyncRepository syncRepository,
        IProductTemplateLocalStore localStore,
        IMesTemplateUploadService uploadService,
        IProductTemplateLanShare lanShare,
        IMesSettingsService mesSettings,
        ILogger<ProductTemplateMesSyncService> logger)
    {
        _syncRepository = syncRepository;
        _localStore = localStore;
        _uploadService = uploadService;
        _lanShare = lanShare;
        _mesSettings = mesSettings;
        _logger = logger;
    }

    public async Task<ProductTemplateMesSyncResult> SyncWithMesAsync(
        string? partNumber = null,
        CancellationToken cancellationToken = default)
    {
        var snapshot = _mesSettings.GetSnapshot();
        var mode = MesProviderMode.Normalize(snapshot.MesMode, snapshot.UseMockMes);

        return mode switch
        {
            MesProviderMode.Mock => SyncMockNoOp(),
            MesProviderMode.LegacyHttp => await SyncLegacyHttpAsync(partNumber, cancellationToken)
                .ConfigureAwait(false),
            MesProviderMode.ProductKey => await SyncProductKeyLanAsync(partNumber, cancellationToken)
                .ConfigureAwait(false),
            _ => SyncMockNoOp(),
        };
    }

    private static ProductTemplateMesSyncResult SyncMockNoOp() =>
        new(
            0,
            0,
            0,
            Array.Empty<string>(),
            Array.Empty<string>(),
            "Mock MES: template sync skipped (no transfer).");

    private async Task<ProductTemplateMesSyncResult> SyncLegacyHttpAsync(
        string? partNumber,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(partNumber))
        {
            return Fail("LegacyHttp MES sync requires an open product PN.");
        }

        var pn = partNumber.Trim();
        var folder = _localStore.GetProductFolder(pn);
        if (!Directory.Exists(folder) || _localStore.TryResolveLocalTemplate(pn) is null)
            return Fail($"{pn}: local screw template not found.");

        try
        {
            var uploadResult = await _uploadService
                .UploadTemplateAsync(pn, folder, cancellationToken)
                .ConfigureAwait(false);
            if (uploadResult is null)
                return Fail($"{pn}: LegacyHttp template upload not available.");

            await ProductTemplateSyncOperations.UpsertFromFileAsync(
                _syncRepository,
                _localStore,
                pn,
                ProductTemplateSyncState.Synced,
                null,
                null,
                DateTimeOffset.UtcNow,
                uploadResult.Revision,
                cancellationToken).ConfigureAwait(false);

            return new ProductTemplateMesSyncResult(
                1,
                0,
                0,
                Array.Empty<string>(),
                Array.Empty<string>(),
                $"LegacyHttp MES sync: uploaded {pn}.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LegacyHttp template upload failed for PN={PartNumber}", pn);
            await MarkFailedAsync(pn, ex.Message, cancellationToken).ConfigureAwait(false);
            return Fail($"{pn}: {ex.Message}");
        }
    }

    private async Task<ProductTemplateMesSyncResult> SyncProductKeyLanAsync(
        string? partNumber,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(partNumber))
            return Fail("ProductKey LAN sync requires an open product PN.");

        var pn = partNumber.Trim();
        try
        {
            var published = await _lanShare.PublishAsync(pn, cancellationToken).ConfigureAwait(false);

            var uploaded = published.Outcome == ProductTemplateLanPublishOutcome.Unchanged ? 0 : 1;
            var skipped = published.Outcome == ProductTemplateLanPublishOutcome.Unchanged ? 1 : 0;
            var outcomeNote = published.Outcome switch
            {
                ProductTemplateLanPublishOutcome.Created => "created",
                ProductTemplateLanPublishOutcome.Updated => "updated",
                ProductTemplateLanPublishOutcome.Unchanged => "unchanged",
                _ => published.Outcome.ToString(),
            };

            await ProductTemplateSyncOperations.UpsertFromFileAsync(
                _syncRepository,
                _localStore,
                pn,
                ProductTemplateSyncState.Synced,
                $"LAN {outcomeNote}: {published.TargetFolder}",
                null,
                DateTimeOffset.UtcNow,
                published.LocalContentHash,
                cancellationToken).ConfigureAwait(false);

            return new ProductTemplateMesSyncResult(
                uploaded,
                0,
                skipped,
                Array.Empty<string>(),
                Array.Empty<string>(),
                $"ProductKey LAN sync: {outcomeNote} {pn} → {published.TargetFolder}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ProductKey LAN template publish failed for PN={PartNumber}", pn);
            return Fail($"{pn}: {ex.Message}");
        }
    }

    private static ProductTemplateMesSyncResult Fail(string message) =>
        new(0, 0, 0, Array.Empty<string>(), [message], message);

    private async Task MarkFailedAsync(string partNumber, string message, CancellationToken cancellationToken)
    {
        try
        {
            await ProductTemplateSyncOperations.UpsertFromFileAsync(
                _syncRepository,
                _localStore,
                partNumber,
                ProductTemplateSyncState.Failed,
                message,
                null,
                null,
                null,
                cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to mark sync failure for PN={PartNumber}", partNumber);
        }
    }
}
