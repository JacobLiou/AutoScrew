using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Services;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Application.Services;

public sealed class ProductTemplateMesSyncService : IProductTemplateMesSyncService
{
    private readonly IProductTemplateSyncRepository _syncRepository;
    private readonly IProductTemplateLocalStore _localStore;
    private readonly IMesTemplateUploadService _uploadService;
    private readonly IMesTemplateCatalogClient _catalogClient;
    private readonly IMesTemplatePackageClient _packageClient;
    private readonly IMesSettingsService _mesSettings;
    private readonly ILogger<ProductTemplateMesSyncService> _logger;

    public ProductTemplateMesSyncService(
        IProductTemplateSyncRepository syncRepository,
        IProductTemplateLocalStore localStore,
        IMesTemplateUploadService uploadService,
        IMesTemplateCatalogClient catalogClient,
        IMesTemplatePackageClient packageClient,
        IMesSettingsService mesSettings,
        ILogger<ProductTemplateMesSyncService> logger)
    {
        _syncRepository = syncRepository;
        _localStore = localStore;
        _uploadService = uploadService;
        _catalogClient = catalogClient;
        _packageClient = packageClient;
        _mesSettings = mesSettings;
        _logger = logger;
    }

    public async Task<ProductTemplateMesSyncResult> SyncWithMesAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = _mesSettings.GetSnapshot();
        if (!snapshot.UseMockMes)
        {
            return new ProductTemplateMesSyncResult(
                0,
                0,
                0,
                Array.Empty<string>(),
                Array.Empty<string>(),
                "Production MES template sync is not available yet.");
        }

        var errors = new List<string>();
        var uploaded = 0;
        var downloaded = 0;
        var skipped = 0;
        var downloadedPartNumbers = new List<string>();

        await UploadPendingAsync(errors, () => uploaded++, cancellationToken).ConfigureAwait(false);
        await DownloadRemoteAsync(
            errors,
            downloadedPartNumbers,
            () => downloaded++,
            () => skipped++,
            cancellationToken).ConfigureAwait(false);

        var summary = BuildSummary(uploaded, downloaded, skipped, errors.Count);
        return new ProductTemplateMesSyncResult(
            uploaded,
            downloaded,
            skipped,
            downloadedPartNumbers,
            errors,
            summary);
    }

    private async Task UploadPendingAsync(
        List<string> errors,
        Action onUploaded,
        CancellationToken cancellationToken)
    {
        var allRecords = await _syncRepository.ListAllAsync(cancellationToken).ConfigureAwait(false);
        var pending = allRecords
            .Where(r => r.SyncState is ProductTemplateSyncState.PendingUpload or ProductTemplateSyncState.Failed)
            .Select(r => r.PartNumber)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var partNumber in _localStore.ListLocalPartNumbers())
        {
            if (_localStore.TryResolveLocalTemplate(partNumber) is null)
                continue;

            var existing = allRecords.FirstOrDefault(r =>
                string.Equals(r.PartNumber, partNumber, StringComparison.OrdinalIgnoreCase));
            if (existing is null)
                pending.Add(partNumber);
        }

        foreach (var partNumber in pending.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                var folder = _localStore.GetProductFolder(partNumber);
                if (!Directory.Exists(folder) || _localStore.TryResolveLocalTemplate(partNumber) is null)
                    continue;

                var uploadResult = await _uploadService
                    .UploadTemplateAsync(partNumber, folder, cancellationToken)
                    .ConfigureAwait(false);
                if (uploadResult is null)
                {
                    errors.Add($"{partNumber}: upload not available.");
                    continue;
                }

                await ProductTemplateSyncOperations.UpsertFromFileAsync(
                    _syncRepository,
                    _localStore,
                    partNumber,
                    ProductTemplateSyncState.Synced,
                    null,
                    null,
                    DateTimeOffset.UtcNow,
                    uploadResult.Revision,
                    cancellationToken).ConfigureAwait(false);

                onUploaded();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MES template upload failed for PN={PartNumber}", partNumber);
                errors.Add($"{partNumber}: {ex.Message}");
                await MarkFailedAsync(partNumber, ex.Message, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private async Task DownloadRemoteAsync(
        List<string> errors,
        List<string> downloadedPartNumbers,
        Action onDownloaded,
        Action onSkipped,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<MesRemoteTemplateEntry> remoteEntries;
        try
        {
            remoteEntries = await _catalogClient.ListRemoteTemplatesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MES template catalog request failed.");
            errors.Add($"catalog: {ex.Message}");
            return;
        }

        var allRecords = await _syncRepository.ListAllAsync(cancellationToken).ConfigureAwait(false);

        foreach (var remote in remoteEntries.OrderBy(x => x.PartNumber, StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                if (await ShouldSkipDownloadAsync(remote, allRecords, cancellationToken).ConfigureAwait(false))
                {
                    onSkipped();
                    continue;
                }

                await _packageClient
                    .DownloadTemplatePackageAsync(remote.PartNumber, remote.PackageUrl, cancellationToken)
                    .ConfigureAwait(false);

                await ProductTemplateSyncOperations.UpsertFromFileAsync(
                    _syncRepository,
                    _localStore,
                    remote.PartNumber,
                    ProductTemplateSyncState.DownloadedFromMes,
                    null,
                    DateTimeOffset.UtcNow,
                    null,
                    null,
                    cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(remote.ContentHash))
                {
                    var record = await _syncRepository.GetAsync(remote.PartNumber, cancellationToken).ConfigureAwait(false);
                    if (record is not null)
                    {
                        await _syncRepository.UpsertAsync(
                            record with { LocalFileHash = remote.ContentHash },
                            cancellationToken).ConfigureAwait(false);
                    }
                }

                downloadedPartNumbers.Add(remote.PartNumber);
                onDownloaded();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MES template download failed for PN={PartNumber}", remote.PartNumber);
                errors.Add($"{remote.PartNumber}: {ex.Message}");
            }
        }
    }

    private Task<bool> ShouldSkipDownloadAsync(
        MesRemoteTemplateEntry remote,
        IReadOnlyList<ProductTemplateSyncRecord> allRecords,
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        var folder = _localStore.GetProductFolder(remote.PartNumber);
        var localExists = _localStore.TryResolveLocalTemplate(remote.PartNumber) is not null;
        if (!localExists)
            return Task.FromResult(false);

        var syncRecord = allRecords.FirstOrDefault(r =>
            string.Equals(r.PartNumber, remote.PartNumber, StringComparison.OrdinalIgnoreCase));

        var localHash = ProductTemplateSyncOperations.ComputePackageHash(folder);

        if (syncRecord?.SyncState == ProductTemplateSyncState.PendingUpload
            && string.Equals(localHash, syncRecord.LocalFileHash, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(true);
        }

        if (!string.IsNullOrWhiteSpace(remote.ContentHash)
            && string.Equals(localHash, remote.ContentHash, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(true);
        }

        if (string.IsNullOrWhiteSpace(remote.ContentHash))
        {
            var localModified = ProductTemplatePackageHash.GetPackageModifiedUtc(folder);
            if (remote.ModifiedUtc <= localModified)
                return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

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

    private static string BuildSummary(int uploaded, int downloaded, int skipped, int errorCount)
    {
        if (errorCount > 0)
            return $"MES sync: uploaded {uploaded}, downloaded {downloaded}, skipped {skipped}, errors {errorCount}.";

        return $"MES sync: uploaded {uploaded}, downloaded {downloaded}, skipped {skipped}.";
    }
}
