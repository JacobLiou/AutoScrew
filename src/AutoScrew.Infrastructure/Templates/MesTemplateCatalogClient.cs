using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.Mes;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Templates;

public sealed class MesTemplateCatalogClient : IMesTemplateCatalogClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMesSettingsService _mesSettings;
    private readonly ILogger<MesTemplateCatalogClient> _logger;

    public MesTemplateCatalogClient(
        IHttpClientFactory httpClientFactory,
        IMesSettingsService mesSettings,
        ILogger<MesTemplateCatalogClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mesSettings = mesSettings;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MesRemoteTemplateEntry>> ListRemoteTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        var snapshot = _mesSettings.GetSnapshot();
        if (!snapshot.UseMockMes)
        {
            _logger.LogInformation("MES template catalog not implemented for production MES.");
            return Array.Empty<MesRemoteTemplateEntry>();
        }

        var uri = BuildUri("api/templates");
        var http = _httpClientFactory.CreateClient("mes");
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var response = await http.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var items = await response.Content.ReadFromJsonAsync<List<CatalogItemDto>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        if (items is null || items.Count == 0)
            return Array.Empty<MesRemoteTemplateEntry>();

        return items
            .Where(x => !string.IsNullOrWhiteSpace(x.PartNumber))
            .Select(x => new MesRemoteTemplateEntry(
                x.PartNumber!,
                x.ContentHash ?? "",
                x.ModifiedUtc ?? DateTimeOffset.MinValue,
                x.PackageUrl ?? $"api/templates/{x.PartNumber}/package"))
            .ToList();
    }

    private Uri BuildUri(string relativeOrAbsolute)
    {
        if (Uri.TryCreate(relativeOrAbsolute, UriKind.Absolute, out var absolute))
            return absolute;

        var snapshot = _mesSettings.GetSnapshot();
        var baseUrl = snapshot.BaseUrl.Trim();
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";
        return new Uri(new Uri(baseUrl, UriKind.Absolute), relativeOrAbsolute.TrimStart('/'));
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, Uri uri)
    {
        var request = new HttpRequestMessage(method, uri);
        var snapshot = _mesSettings.GetSnapshot();
        if (!string.IsNullOrWhiteSpace(snapshot.ApiKey))
            request.Headers.TryAddWithoutValidation("X-Api-Key", snapshot.ApiKey);
        return request;
    }

    private sealed class CatalogItemDto
    {
        [JsonPropertyName("partNumber")]
        public string? PartNumber { get; set; }

        [JsonPropertyName("contentHash")]
        public string? ContentHash { get; set; }

        [JsonPropertyName("modifiedUtc")]
        public DateTimeOffset? ModifiedUtc { get; set; }

        [JsonPropertyName("packageUrl")]
        public string? PackageUrl { get; set; }
    }
}
