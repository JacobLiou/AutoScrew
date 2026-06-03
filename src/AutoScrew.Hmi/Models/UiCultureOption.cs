namespace AutoScrew.Hmi.Models;

/// <summary>界面语言下拉项（文化代码 + 国旗 + 显示名）。</summary>
public sealed class UiCultureOption(string cultureName, string flagGlyph, string displayName)
{
    public string CultureName { get; } = cultureName;

    public string FlagGlyph { get; } = flagGlyph;

    public string DisplayName { get; } = displayName;

    public string DisplayText => $"{FlagGlyph}  {displayName}";
}
