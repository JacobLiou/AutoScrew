using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Mes.ProductKey;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Mes;

public sealed class ConfigurableMesClient : IMesClient
{
    private readonly IMesSettingsService _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ISnWorkArchiveSync _archiveSync;
    private readonly ILogger<MesHttpClient> _httpLogger;
    private readonly ILogger<ProductKeyMesClient> _productKeyLogger;
    private readonly LocalRecipeMesClient _localMock;
    private readonly MockMesClient _legacyMock = new();

    public ConfigurableMesClient(
        IMesSettingsService settings,
        IHttpClientFactory httpClientFactory,
        IOptions<AutoScrewAppOptions> appOptions,
        LocalRecipeMesClient localMock,
        ISnWorkArchiveSync archiveSync,
        ILogger<MesHttpClient> httpLogger,
        ILogger<ProductKeyMesClient> productKeyLogger)
    {
        _settings = settings;
        _httpClientFactory = httpClientFactory;
        _appOptions = appOptions;
        _localMock = localMock;
        _archiveSync = archiveSync;
        _httpLogger = httpLogger;
        _productKeyLogger = productKeyLogger;
    }

    public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
        ResolveClient().ValidateSnAsync(serialNumber, cancellationToken);

    public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default) =>
        ResolveClient().GetRecipeAsync(serialNumber, partNumber, cancellationToken);

    public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
        ResolveClient().UploadResultAsync(payload, cancellationToken);

    public async Task<MesConnectionTestResult> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = _settings.GetSnapshot();
        var mode = MesProviderMode.Normalize(snapshot.MesMode, snapshot.UseMockMes);

        if (mode == MesProviderMode.Mock)
            return new MesConnectionTestResult(true, "Mock MES enabled.");

        if (mode == MesProviderMode.ProductKey)
        {
            var client = CreateProductKeyClient(snapshot);
            return await client.TestConnectionAsync(snapshot.ProbeSerialNumber, cancellationToken).ConfigureAwait(false);
        }

        var http = CreateHttpClient(snapshot);
        var legacy = new MesHttpClient(http, snapshot, _appOptions.Value.StationId, _httpLogger);
        return await legacy.TestConnectionAsync(cancellationToken).ConfigureAwait(false);
    }

    private IMesClient ResolveClient()
    {
        var snapshot = _settings.GetSnapshot();
        var mode = MesProviderMode.Normalize(snapshot.MesMode, snapshot.UseMockMes);

        if (mode == MesProviderMode.Mock)
            return _appOptions.Value.UseLocalRecipes ? _localMock : _legacyMock;

        if (mode == MesProviderMode.ProductKey)
            return CreateProductKeyClient(snapshot);

        var http = CreateHttpClient(snapshot);
        var inner = new MesHttpClient(http, snapshot, _appOptions.Value.StationId, _httpLogger);
        return new MesHttpClientAdapter(inner);
    }

    private ProductKeyMesClient CreateProductKeyClient(MesRuntimeSettings snapshot)
    {
        var timeout = snapshot.TimeoutSeconds > 0
            ? TimeSpan.FromSeconds(snapshot.TimeoutSeconds)
            : TimeSpan.FromSeconds(100);

        var options = new ProductKeyMesOptions
        {
            ContainerApiBaseUrl = string.IsNullOrWhiteSpace(snapshot.BaseUrl)
                ? "https://zuhaip.molex.com:9607"
                : snapshot.BaseUrl.TrimEnd('/'),
            Timeout = timeout,
            AcceptAnyServerCertificate = snapshot.AcceptAnyServerCertificate,
        };

        return new ProductKeyMesClient(options, _archiveSync, _productKeyLogger);
    }

    private HttpClient CreateHttpClient(MesRuntimeSettings snapshot)
    {
        var http = _httpClientFactory.CreateClient("mes");
        http.Timeout = TimeSpan.FromSeconds(snapshot.TimeoutSeconds);
        return http;
    }
}

public sealed record MesConnectionTestResult(bool Success, string Message);
