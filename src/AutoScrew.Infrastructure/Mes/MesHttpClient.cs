using System.Net.Http.Json;
using System.Text.Json;
using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Mes;

public sealed class MesHttpClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;
    private readonly MesRuntimeSettings _settings;
    private readonly string _stationId;
    private readonly ILogger<MesHttpClient> _logger;

    public MesHttpClient(
        HttpClient http,
        MesRuntimeSettings settings,
        string stationId,
        ILogger<MesHttpClient> logger)
    {
        _http = http;
        _settings = settings;
        _stationId = stationId;
        _logger = logger;
    }

    public async Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var uri = BuildUri($"api/sn/validate?sn={Uri.EscapeDataString(serialNumber)}&stationId={Uri.EscapeDataString(_stationId)}");
            using var request = CreateRequest(HttpMethod.Get, uri);
            using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return new SnValidationResult(false, null, $"MES validate HTTP {(int)response.StatusCode}: {TrimBody(body)}");
            }

            var dto = await response.Content.ReadFromJsonAsync<SnValidateDto>(JsonOptions, cancellationToken).ConfigureAwait(false);
            if (dto is null)
                return new SnValidationResult(false, null, "Empty MES response.");

            return new SnValidationResult(dto.Valid, dto.PartNumber, dto.Message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MES validate failed for SN={SerialNumber}", serialNumber);
            return new SnValidationResult(false, null, $"MES validate failed: {ex.Message}");
        }
    }

    public async Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default)
    {
        var uri = BuildUri(
            $"api/recipe?sn={Uri.EscapeDataString(serialNumber)}&pn={Uri.EscapeDataString(partNumber)}&stationId={Uri.EscapeDataString(_stationId)}");
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException($"MES recipe HTTP {(int)response.StatusCode}: {TrimBody(body)}");
        }

        var dto = await response.Content.ReadFromJsonAsync<RecipeDto>(JsonOptions, cancellationToken).ConfigureAwait(false)
                  ?? throw new InvalidOperationException("MES returned empty recipe.");

        var screws = dto.Screws?.Select(s => new ScrewRecipeDto(
            s.Index,
            s.PartNo,
            s.TargetTorqueNm,
            s.TorqueLowerNm,
            s.TorqueUpperNm,
            s.AngleLimitDeg,
            s.ControllerParameterId)).ToList() ?? new List<ScrewRecipeDto>();

        return new RecipeBundle(partNumber, dto.TemplateJsonPath, dto.ProductImageUrl, screws);
    }

    public async Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default)
    {
        var uri = BuildUri("api/results");
        using var request = CreateRequest(HttpMethod.Post, uri);
        request.Content = JsonContent.Create(payload, options: JsonOptions);
        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return new MesUploadResult(false, $"HTTP {(int)response.StatusCode}: {TrimBody(body)}", null);
        }

        return new MesUploadResult(true, null, $"{payload.SerialNumber}:{payload.CompletedAt:O}");
    }

    public async Task<MesConnectionTestResult> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var healthUri = BuildUri("api/health");
            using (var healthRequest = CreateRequest(HttpMethod.Get, healthUri))
            {
                using var healthResponse = await _http.SendAsync(healthRequest, cancellationToken).ConfigureAwait(false);
                if (healthResponse.IsSuccessStatusCode)
                {
                    var body = await healthResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    return new MesConnectionTestResult(true, $"Health OK ({(int)healthResponse.StatusCode}): {TrimBody(body)}");
                }

                if (healthResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var body = await healthResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    return new MesConnectionTestResult(
                        false,
                        $"Health HTTP {(int)healthResponse.StatusCode}: {TrimBody(body)}");
                }
            }

            var ping = await ValidateSnAsync("__PING__", cancellationToken).ConfigureAwait(false);
            return new MesConnectionTestResult(
                true,
                $"Validate fallback: valid={ping.IsValid}; message={ping.Message ?? "OK"}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MES connection test failed.");
            return new MesConnectionTestResult(false, ex.Message);
        }
    }

    private Uri BuildUri(string relativePath)
    {
        var baseUrl = _settings.BaseUrl.Trim();
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";
        return new Uri(new Uri(baseUrl, UriKind.Absolute), relativePath);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, Uri uri)
    {
        var request = new HttpRequestMessage(method, uri);
        if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
            request.Headers.TryAddWithoutValidation("X-Api-Key", _settings.ApiKey);
        return request;
    }

    private static string TrimBody(string body) =>
        body.Length > 200 ? body[..200] + "…" : body;

    private sealed class SnValidateDto
    {
        public bool Valid { get; set; }

        public string? PartNumber { get; set; }

        public string? Message { get; set; }
    }

    private sealed class RecipeDto
    {
        public string? TemplateJsonPath { get; set; }

        public string? ProductImageUrl { get; set; }

        public List<ScrewRecipeRow>? Screws { get; set; }
    }

    private sealed class ScrewRecipeRow
    {
        public int Index { get; set; }

        public string? PartNo { get; set; }

        public double TargetTorqueNm { get; set; }

        public double TorqueLowerNm { get; set; }

        public double TorqueUpperNm { get; set; }

        public double AngleLimitDeg { get; set; }

        public int? ControllerParameterId { get; set; }
    }
}
