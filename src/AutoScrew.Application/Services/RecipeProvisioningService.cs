using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Application.Services;

public sealed class RecipeProvisioningService : IRecipeProvisioningService
{
    private readonly IMesClient _mesClient;
    private readonly IMesSettingsService _mesSettings;
    private readonly IProductTemplateLocalStore _localStore;
    private readonly IProductTemplateLanShare _lanShare;
    private readonly IMesTemplatePackageClient _packageClient;
    private readonly IProductTemplateSyncRepository _syncRepository;
    private readonly IOptions<AutoScrewAppOptions> _options;
    private readonly ILogger<RecipeProvisioningService> _logger;

    public RecipeProvisioningService(
        IMesClient mesClient,
        IMesSettingsService mesSettings,
        IProductTemplateLocalStore localStore,
        IProductTemplateLanShare lanShare,
        IMesTemplatePackageClient packageClient,
        IProductTemplateSyncRepository syncRepository,
        IOptions<AutoScrewAppOptions> options,
        ILogger<RecipeProvisioningService> logger)
    {
        _mesClient = mesClient;
        _mesSettings = mesSettings;
        _localStore = localStore;
        _lanShare = lanShare;
        _packageClient = packageClient;
        _syncRepository = syncRepository;
        _options = options;
        _logger = logger;
    }

    public async Task<ProvisionedRecipe> GetProvisionedRecipeAsync(
        string serialNumber,
        string partNumber,
        CancellationToken cancellationToken = default)
    {
        RecipeBundle recipe;
        try
        {
            recipe = await _mesClient.GetRecipeAsync(serialNumber, partNumber, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MES GetRecipe failed for SN={SerialNumber}, PN={PartNumber}", serialNumber, partNumber);
            return await ResolveLocalOnlyAsync(partNumber, $"MES recipe unavailable: {ex.Message}", cancellationToken)
                .ConfigureAwait(false);
        }

        var snapshot = _mesSettings.GetSnapshot();
        string? resolvedPath = null;
        string? infoMessage = null;
        var source = RecipeTemplateSource.Local;

        var mode = MesProviderMode.Normalize(snapshot.MesMode, snapshot.UseMockMes);
        if (mode == MesProviderMode.LegacyHttp && !string.IsNullOrWhiteSpace(recipe.TemplatePackageUrl))
        {
            try
            {
                resolvedPath = await _packageClient
                    .DownloadTemplatePackageAsync(partNumber, recipe.TemplatePackageUrl!, cancellationToken)
                    .ConfigureAwait(false);
                source = RecipeTemplateSource.Mes;
                await ProductTemplateSyncOperations.UpsertFromFileAsync(
                    _syncRepository,
                    _localStore,
                    partNumber,
                    ProductTemplateSyncState.DownloadedFromMes,
                    null,
                    DateTimeOffset.UtcNow,
                    null,
                    null,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MES template package download failed for PN={PartNumber}", partNumber);
                infoMessage = $"MES template download failed: {ex.Message}";
            }
        }

        if (resolvedPath is null)
            resolvedPath = _localStore.TryResolveTemplatePath(recipe.TemplateJsonPath);

        if (resolvedPath is null)
            resolvedPath = _localStore.TryResolveLocalTemplate(partNumber);

        if (resolvedPath is null)
        {
            resolvedPath = _lanShare.TryMaterializeLanTemplateToLocal(partNumber);
            if (resolvedPath is not null)
                infoMessage ??= "Using screw template from process library LAN share.";
        }

        if (resolvedPath is null)
            throw new InvalidOperationException($"Template file not found for PN {partNumber}.");

        if (source == RecipeTemplateSource.Local)
        {
            if (infoMessage is null && !snapshot.UseMockMes)
                infoMessage = "Using local template (MES download unavailable or skipped).";

            await ProductTemplateSyncOperations.UpsertFromFileAsync(
                _syncRepository,
                _localStore,
                partNumber,
                ProductTemplateSyncState.LocalOnly,
                infoMessage,
                null,
                null,
                null,
                cancellationToken).ConfigureAwait(false);
        }

        return new ProvisionedRecipe(recipe, resolvedPath, source, infoMessage);
    }

    private async Task<ProvisionedRecipe> ResolveLocalOnlyAsync(
        string partNumber,
        string infoMessage,
        CancellationToken cancellationToken)
    {
        var path = _localStore.TryResolveLocalTemplate(partNumber)
                   ?? _lanShare.TryMaterializeLanTemplateToLocal(partNumber);
        if (path is null)
            throw new InvalidOperationException($"Template file not found for PN {partNumber} (local fallback).");

        var recipe = new RecipeBundle(partNumber, _localStore.ToRelativePath(path), null, Array.Empty<ScrewRecipeDto>());
        await ProductTemplateSyncOperations.UpsertFromFileAsync(
            _syncRepository,
            _localStore,
            partNumber,
            ProductTemplateSyncState.LocalOnly,
            infoMessage,
            null,
            null,
            null,
            cancellationToken).ConfigureAwait(false);

        return new ProvisionedRecipe(recipe, path, RecipeTemplateSource.Local, infoMessage);
    }
}
