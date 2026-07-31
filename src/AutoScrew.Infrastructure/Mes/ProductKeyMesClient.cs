using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Mes.ProductKey;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Mes;

/// <summary>Opcenter getProductInfo → IMesClient；作业结果仅触发局域网 SN 归档。</summary>
public sealed class ProductKeyMesClient : IMesClient
{
    private readonly ProductKeyMesOptions _options;
    private readonly ISnWorkArchiveSync _archiveSync;
    private readonly ILogger<ProductKeyMesClient> _logger;

    public ProductKeyMesClient(
        ProductKeyMesOptions options,
        ISnWorkArchiveSync archiveSync,
        ILogger<ProductKeyMesClient> logger)
    {
        _options = options;
        _archiveSync = archiveSync;
        _logger = logger;
    }

    public async Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        using var api = new MesProductApi(_options);
        var result = await api.GetProductKeyInfoAsync(serialNumber, cancellationToken).ConfigureAwait(false);
        if (!result.Success || result.Data is null)
            return new SnValidationResult(false, null, result.Error ?? "ProductKey query failed.");

        var info = result.Data;
        if (string.IsNullOrWhiteSpace(info.PartNumber))
            return new SnValidationResult(false, null, "ProductKey: empty part number (PN).");

        if (!info.IsAvailable)
        {
            return new SnValidationResult(
                false,
                info.PartNumber,
                $"ProductKey: SN not available (Status/Hold). Spec={info.Spec}; WO={info.WorkOrder}; Process={info.CurrentProcess}");
        }

        _logger.LogInformation(
            "ProductKey OK SN={Serial} PN={Pn} Spec={Spec} WO={Wo}",
            info.SerialNo,
            info.PartNumber,
            info.Spec,
            info.WorkOrder);

        return new SnValidationResult(true, info.PartNumber, null);
    }

    public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default)
    {
        // 不拉 ATMS；模板由 RecipeProvisioningService 解析本地 Templates/{PN}
        return Task.FromResult(new RecipeBundle(partNumber, null, null, Array.Empty<ScrewRecipeDto>()));
    }

    public async Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default)
    {
        try
        {
            await _archiveSync.SyncSerialFolderAsync(payload.SerialNumber, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ProductKey UploadResult LAN archive failed for {Serial}", payload.SerialNumber);
        }

        return new MesUploadResult(true, "Local accept; LAN archive best-effort (no TAS upload).", null);
    }

    public async Task<MesConnectionTestResult> TestConnectionAsync(string? probeSerial, CancellationToken cancellationToken = default)
    {
        var sn = string.IsNullOrWhiteSpace(probeSerial) ? "PROBE" : probeSerial.Trim();
        using var api = new MesProductApi(_options);
        var result = await api.GetProductInfoAsync(sn, cancellationToken).ConfigureAwait(false);
        if (result.Success)
            return new MesConnectionTestResult(true, $"ProductKey OK: getProductInfo({sn}) returned data.");

        // 探测 SN 不存在仍说明 HTTP/证书可达（业务错误）
        var err = result.Error ?? "unknown";
        if (err.Contains("Can not find", StringComparison.OrdinalIgnoreCase)
            || err.Contains("no product information", StringComparison.OrdinalIgnoreCase)
            || err.Contains("HTTP 404", StringComparison.OrdinalIgnoreCase))
        {
            return new MesConnectionTestResult(true, $"ProductKey reachable; probe SN '{sn}' has no container data ({Truncate(err)}).");
        }

        if (err.StartsWith("HTTP 4", StringComparison.OrdinalIgnoreCase)
            || err.StartsWith("HTTP 5", StringComparison.OrdinalIgnoreCase))
        {
            // 4xx/5xx from server means we talked to the host
            return new MesConnectionTestResult(false, Truncate(err));
        }

        return new MesConnectionTestResult(false, Truncate(err));
    }

    private static string Truncate(string text, int max = 240) =>
        text.Length <= max ? text : text[..max] + "...";
}
