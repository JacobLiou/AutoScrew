namespace AutoScrew.Application.Configuration;

/// <summary>
/// 脱机验证用本地 SN→PN→模板 注册表（Mock MES 读取 local-recipes.json）。
/// </summary>
public sealed class LocalRecipeDocument
{
    public int Version { get; set; } = 1;

    public List<LocalRecipeProductEntry> Products { get; set; } = new();
}

public sealed class LocalRecipeProductEntry
{
    public string PartNumber { get; set; } = "";

    /// <summary>相对 TemplateDirectory 的模板文件；缺省为 {PartNumber}.product-template.json</summary>
    public string? TemplateFile { get; set; }

    public List<string> SerialNumbers { get; set; } = new();

    public List<LocalRecipeScrewEntry> Screws { get; set; } = new();
}

public sealed class LocalRecipeScrewEntry
{
    public int PositionIndex { get; set; }

    public string? PartNo { get; set; }

    public double TargetTorqueNm { get; set; } = 0.35;

    public double TorqueLowerNm { get; set; } = 0.25;

    public double TorqueUpperNm { get; set; } = 0.38;

    public double AngleLimitDeg { get; set; } = 720;

    public int? ControllerParameterId { get; set; }
}
