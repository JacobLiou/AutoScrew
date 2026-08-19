using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Mes;

/// <summary>
/// Mock MES：优先读 <see cref="LocalJsonRecipeStore"/>（local-recipes.json），未配置时回退旧 PNDEMO 行为。
/// </summary>
public sealed class LocalRecipeMesClient : IMesClient
{
    private readonly LocalJsonRecipeStore _store;
    private readonly IOptions<AutoScrewAppOptions> _options;
    private readonly ILogger<LocalRecipeMesClient> _logger;
    private readonly MockMesClient _legacy = new();

    public LocalRecipeMesClient(
        LocalJsonRecipeStore store,
        IOptions<AutoScrewAppOptions> options,
        ILogger<LocalRecipeMesClient> logger)
    {
        _store = store;
        _options = options;
        _logger = logger;
    }

    public async Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        var sn = serialNumber.Trim();
        if (sn.Length == 0)
            return new SnValidationResult(false, null, "SN is empty.");

        var loaded = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        if (!loaded.Exists)
            return await _legacy.ValidateSnAsync(sn, cancellationToken).ConfigureAwait(false);

        var product = FindProductBySn(loaded.Document, sn);
        if (product is null)
        {
            _logger.LogInformation("SN {SerialNumber} not in local-recipes.json", sn);
            return new SnValidationResult(false, null, $"SN not registered in local-recipes.json: {sn}");
        }

        return new SnValidationResult(true, product.PartNumber, null);
    }

    public async Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default)
    {
        var loaded = await _store.LoadAsync(cancellationToken).ConfigureAwait(false);
        if (!loaded.Exists)
            return await _legacy.GetRecipeAsync(serialNumber, partNumber, cancellationToken).ConfigureAwait(false);

        var product = FindProductByPn(loaded.Document, partNumber)
                      ?? FindProductBySn(loaded.Document, serialNumber.Trim());
        if (product is null)
            throw new InvalidOperationException($"Part number '{partNumber}' not found in local-recipes.json.");

        var templateFile = product.TemplateFile ?? $"{product.PartNumber}.product-template.json";
        if (!TemplateFileExists(templateFile))
        {
            throw new InvalidOperationException(
                $"Template file not found for PN {product.PartNumber}: {templateFile} (TemplateDirectory={_options.Value.TemplateDirectory})");
        }

        var screws = product.Screws
            .Select(s => new ScrewRecipeDto(
                s.PositionIndex,
                s.PartNo,
                s.TargetTorqueNm,
                s.TorqueLowerNm,
                s.TorqueUpperNm,
                s.AngleLimitDeg,
                s.ControllerParameterId))
            .ToList();

        return new RecipeBundle(product.PartNumber, templateFile, null, screws);
    }

    public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
        _legacy.UploadResultAsync(payload, cancellationToken);

    private static LocalRecipeProductEntry? FindProductBySn(LocalRecipeDocument doc, string sn) =>
        doc.Products.FirstOrDefault(p =>
            p.SerialNumbers.Any(s => string.Equals(s, sn, StringComparison.OrdinalIgnoreCase)));

    private static LocalRecipeProductEntry? FindProductByPn(LocalRecipeDocument doc, string pn) =>
        doc.Products.FirstOrDefault(p =>
            string.Equals(p.PartNumber, pn.Trim(), StringComparison.OrdinalIgnoreCase));

    private bool TemplateFileExists(string templateFile)
    {
        if (Path.IsPathRooted(templateFile) && File.Exists(templateFile))
            return true;

        var dir = _options.Value.TemplateDirectory;
        if (!string.IsNullOrWhiteSpace(dir))
        {
            var combined = Path.Combine(dir, templateFile);
            if (File.Exists(combined))
                return true;
        }

        return File.Exists(templateFile);
    }
}
