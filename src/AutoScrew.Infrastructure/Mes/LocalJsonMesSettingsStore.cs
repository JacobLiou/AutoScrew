using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Mes;

public sealed class LocalJsonMesSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string _settingsPath;
    private readonly IConfiguration _configuration;

    public LocalJsonMesSettingsStore(IOptions<AutoScrewAppOptions> appOptions, IConfiguration configuration)
    {
        _configuration = configuration;
        var root = string.IsNullOrWhiteSpace(appOptions.Value.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : appOptions.Value.DataDirectory;
        Directory.CreateDirectory(root);
        _settingsPath = Path.Combine(root, "mes-settings.json");
    }

    public async Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_settingsPath))
            return CreateSeedSettings();

        cancellationToken.ThrowIfCancellationRequested();
        var json = await File.ReadAllTextAsync(_settingsPath, cancellationToken).ConfigureAwait(false);
        var settings = JsonSerializer.Deserialize<MesRuntimeSettings>(json, JsonOptions) ?? CreateSeedSettings();
        Normalize(settings);
        return settings;
    }

    public async Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Normalize(settings);
        await using var stream = File.Create(_settingsPath);
        await JsonSerializer.SerializeAsync(stream, settings, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    private MesRuntimeSettings CreateSeedSettings()
    {
        var app = _configuration.GetSection(AutoScrewAppOptions.SectionName).Get<AutoScrewAppOptions>()
                  ?? new AutoScrewAppOptions();
        return new MesRuntimeSettings
        {
            UseMockMes = app.UseMockMes,
            BaseUrl = app.MesBaseUrl,
            TimeoutSeconds = 15,
        };
    }

    private static void Normalize(MesRuntimeSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            settings.BaseUrl = "https://localhost/";
        if (!settings.BaseUrl.EndsWith('/'))
            settings.BaseUrl += "/";
        if (settings.TimeoutSeconds <= 0)
            settings.TimeoutSeconds = 15;
    }
}
