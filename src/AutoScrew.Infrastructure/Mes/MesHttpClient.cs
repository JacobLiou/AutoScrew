using System.Net.Http.Json;
using System.Text.Json;
using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Mes;

/// <summary>
/// HTTP placeholder for real MES integration (endpoints to be wired when IT contract is available).
/// </summary>
public sealed class MesHttpClient(HttpClient http, ILogger<MesHttpClient> logger)
    : IMesClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var uri = $"api/sn/validate?sn={Uri.EscapeDataString(serialNumber)}";
            var dto = await http.GetFromJsonAsync<SnValidateDto>(uri, JsonOptions, cancellationToken).ConfigureAwait(false);
            if (dto is null)
                return new SnValidationResult(false, null, "Empty MES response.");

            return new SnValidationResult(dto.Valid, dto.PartNumber, dto.Message);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "MES validate failed; treating as invalid.");
            return new SnValidationResult(false, null, "MES validate failed.");
        }
    }

    public async Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default)
    {
        var uri = $"api/recipe?sn={Uri.EscapeDataString(serialNumber)}&pn={Uri.EscapeDataString(partNumber)}";
        var dto = await http.GetFromJsonAsync<RecipeDto>(uri, JsonOptions, cancellationToken).ConfigureAwait(false);
        if (dto is null)
            throw new InvalidOperationException("MES returned empty recipe.");

        var screws = dto.Screws?.Select(s => new ScrewRecipeDto(s.Index, s.PartNo, s.TargetTorqueNm, s.TorqueLowerNm, s.TorqueUpperNm, s.AngleLimitDeg)).ToList()
                     ?? new List<ScrewRecipeDto>();
        return new RecipeBundle(partNumber, dto.TemplateJsonPath, dto.ProductImageUrl, screws);
    }

    public async Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default)
    {
        const string uri = "api/results";
        using var response = await http.PostAsJsonAsync(uri, payload, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return new MesUploadResult(false, $"HTTP {(int)response.StatusCode}: {body}", null);
        }

        return new MesUploadResult(true, null, $"{payload.SerialNumber}:{payload.CompletedAt:O}");
    }

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
    }
}
