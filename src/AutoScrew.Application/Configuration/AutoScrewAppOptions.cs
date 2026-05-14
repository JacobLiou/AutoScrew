namespace AutoScrew.Application.Configuration;

public sealed class AutoScrewAppOptions
{
    public const string SectionName = "AutoScrew";

    public bool UseMockMes { get; set; }

    public bool UseSimulatedHardware { get; set; }

    public string MesBaseUrl { get; set; } = "https://localhost/";

    /// <summary>SQLite DB and local work files root.</summary>
    public string DataDirectory { get; set; } = "";

    /// <summary>Optional folder scanned for PN templates when MES returns only a file name.</summary>
    public string TemplateDirectory { get; set; } = "";

    public string? OptionalNetworkArchiveRoot { get; set; }

    public string StationId { get; set; } = "STATION-01";
}
