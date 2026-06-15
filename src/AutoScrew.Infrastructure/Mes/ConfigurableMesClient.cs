using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Mes;

public sealed class ConfigurableMesClient : IMesClient
{
    private readonly IMesSettingsService _settings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ILogger<MesHttpClient> _logger;
    private readonly MockMesClient _mock = new();

    public ConfigurableMesClient(
        IMesSettingsService settings,
        IHttpClientFactory httpClientFactory,
        IOptions<AutoScrewAppOptions> appOptions,
        ILogger<MesHttpClient> logger)
    {
        _settings = settings;
        _httpClientFactory = httpClientFactory;
        _appOptions = appOptions;
        _logger = logger;
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
        if (snapshot.UseMockMes)
            return new MesConnectionTestResult(true, "Mock MES enabled.");

        var http = CreateHttpClient(snapshot);
        var client = new MesHttpClient(http, snapshot, _appOptions.Value.StationId, _logger);
        return await client.TestConnectionAsync(cancellationToken).ConfigureAwait(false);
    }

    private IMesClient ResolveClient()
    {
        var snapshot = _settings.GetSnapshot();
        if (snapshot.UseMockMes)
            return _mock;

        var http = CreateHttpClient(snapshot);
        var inner = new MesHttpClient(http, snapshot, _appOptions.Value.StationId, _logger);
        return new MesHttpClientAdapter(inner);
    }

    private HttpClient CreateHttpClient(MesRuntimeSettings snapshot)
    {
        var http = _httpClientFactory.CreateClient("mes");
        http.Timeout = TimeSpan.FromSeconds(snapshot.TimeoutSeconds);
        return http;
    }
}

public sealed record MesConnectionTestResult(bool Success, string Message);
