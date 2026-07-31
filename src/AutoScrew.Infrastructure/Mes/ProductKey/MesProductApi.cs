using System.Net;
using System.Text.Json;
using AutoScrew.Infrastructure.Mes.ProductKey.Models;

namespace AutoScrew.Infrastructure.Mes.ProductKey;

public sealed class MesProductApi : IDisposable
{
    private const string GetProductInfoPath = "api/v2/container/query/getProductInfo";

    private readonly HttpClient _http;
    private readonly bool _ownsHttp;

    public MesProductApi(ProductKeyMesOptions? options = null)
        : this(ProductKeyHttp.CreateClient(options ?? new ProductKeyMesOptions()), ownsHttp: true)
    {
    }

    public MesProductApi(HttpClient http, bool ownsHttp = false)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _ownsHttp = ownsHttp;
    }

    public async Task<MesResult<ProductInfoDto>> GetProductInfoAsync(
        string serialNo,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNo))
            return MesResult<ProductInfoDto>.Fail("SN is null or empty(SN为空).");

        try
        {
            var relative = $"{GetProductInfoPath}?container={Uri.EscapeDataString(serialNo.Trim())}";
            using var response = await _http.GetAsync(relative, cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return MesResult<ProductInfoDto>.Fail(
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {Truncate(body)}");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return MesResult<ProductInfoDto>.Fail(
                    $"container:{serialNo} --- Can not find any information for this container!");
            }

            var dto = JsonSerializer.Deserialize<ProductInfoDto>(body, ProductKeyHttp.JsonOptions);
            if (dto is null)
                return MesResult<ProductInfoDto>.Fail($"Unexpected JSON from getProductInfo: {Truncate(body)}");

            dto.Spec = ProcessNameMap.Normalize(dto.Spec);
            return MesResult<ProductInfoDto>.Ok(dto);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return MesResult<ProductInfoDto>.Fail(Flatten(ex));
        }
    }

    public async Task<MesResult<ProductKeyInfo>> GetProductKeyInfoAsync(
        string serialNo,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductInfoAsync(serialNo, cancellationToken).ConfigureAwait(false);
        if (!product.Success || product.Data is null)
        {
            return MesResult<ProductKeyInfo>.Fail(
                product.Error
                ?? "OPC interface error: There is no product information from EMS.");
        }

        var d = product.Data;
        var pn = FirstNonEmpty(d.Product, d.OplinkPn, d.TopPn);
        var onHold = bool.TryParse(d.IsOnHold, out var hold) && hold;
        var available = d.Status == "1" && !onHold;

        return MesResult<ProductKeyInfo>.Ok(new ProductKeyInfo
        {
            SerialNo = serialNo.Trim(),
            PartNumber = pn,
            Spec = d.Spec ?? string.Empty,
            WorkOrder = d.MfgOrder ?? string.Empty,
            CurrentProcess = FirstNonEmpty(d.Operation, d.Spec),
            IsAvailable = available,
            Raw = d,
        });
    }

    public void Dispose()
    {
        if (_ownsHttp)
            _http.Dispose();
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var v in values)
        {
            if (!string.IsNullOrWhiteSpace(v))
                return v;
        }

        return string.Empty;
    }

    private static string Truncate(string? text, int max = 500)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text.Length <= max ? text : text[..max] + "...";
    }

    private static string Flatten(Exception ex)
    {
        if (ex is HttpRequestException httpEx)
        {
            return httpEx.StatusCode is HttpStatusCode code
                ? $"HTTP {(int)code}: {httpEx.Message}"
                : httpEx.Message;
        }

        return ex.InnerException?.Message is { Length: > 0 } inner
            ? ex.Message + " " + inner
            : ex.Message;
    }
}
