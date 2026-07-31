namespace AutoScrew.Application.Abstractions;

public sealed class MesRuntimeSettings
{
    /// <summary><see cref="MesProviderMode"/>：Mock / LegacyHttp / ProductKey。</summary>
    public string MesMode { get; set; } = MesProviderMode.Mock;

    /// <summary>兼容旧 mes-settings；与 MesMode 同步。</summary>
    public bool UseMockMes { get; set; } = true;

    public string BaseUrl { get; set; } = "https://localhost/";

    public string? ApiKey { get; set; }

    public int TimeoutSeconds { get; set; } = 15;

    /// <summary>ProductKey：放宽服务端证书校验（现场默认 true）。</summary>
    public bool AcceptAnyServerCertificate { get; set; } = true;

    /// <summary>Mes 页测试连接用的探测 SN。</summary>
    public string? ProbeSerialNumber { get; set; }

    /// <summary>局域网 SN 归档根 UNC；凭证不在此文件，见 AutoScrew:LanSharePasswordAes256。</summary>
    public string? LanShareRoot { get; set; }

    public MesRuntimeSettings Clone() =>
        new()
        {
            MesMode = MesMode,
            UseMockMes = UseMockMes,
            BaseUrl = BaseUrl,
            ApiKey = ApiKey,
            TimeoutSeconds = TimeoutSeconds,
            AcceptAnyServerCertificate = AcceptAnyServerCertificate,
            ProbeSerialNumber = ProbeSerialNumber,
            LanShareRoot = LanShareRoot,
        };
}

public interface IMesSettingsService
{
    MesRuntimeSettings GetSnapshot();

    Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default);

    void ApplySnapshot(MesRuntimeSettings settings);
}
