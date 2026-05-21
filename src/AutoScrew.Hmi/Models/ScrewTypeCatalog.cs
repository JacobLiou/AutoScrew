namespace AutoScrew.Hmi.Models;

/// <summary>
/// 六种常用螺钉示意类型（仅用于画板视觉直径，非工程公差定义）。
/// </summary>
public static class ScrewTypeCatalog
{
    /// <summary>预设列表，Id 1..6 稳定用于 JSON。</summary>
    public static IReadOnlyList<ScrewTypePreset> All { get; } = new[]
    {
        new ScrewTypePreset(1, "M1.0", "M1.0 / 极小", 18),
        new ScrewTypePreset(2, "M1.4", "M1.4 / 很小", 22),
        new ScrewTypePreset(3, "M2", "M2（默认）", 26),
        new ScrewTypePreset(4, "M2.5", "M2.5", 30),
        new ScrewTypePreset(5, "M3", "M3", 34),
        new ScrewTypePreset(6, "M4", "M4 / 较大", 40),
    }.ToList();

    public static ScrewTypePreset Default => All[2];

    public static ScrewTypePreset? TryGetById(int id) => All.FirstOrDefault(t => t.Id == id);
}
