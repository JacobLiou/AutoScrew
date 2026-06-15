namespace AutoScrew.Application.Abstractions;

public sealed class MesRuntimeSettings
{
    public bool UseMockMes { get; set; } = true;

    public string BaseUrl { get; set; } = "https://localhost/";

    public string? ApiKey { get; set; }

    public int TimeoutSeconds { get; set; } = 15;

    public MesRuntimeSettings Clone() =>
        new()
        {
            UseMockMes = UseMockMes,
            BaseUrl = BaseUrl,
            ApiKey = ApiKey,
            TimeoutSeconds = TimeoutSeconds,
        };
}

public interface IMesSettingsService
{
    MesRuntimeSettings GetSnapshot();

    Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default);

    void ApplySnapshot(MesRuntimeSettings settings);
}
