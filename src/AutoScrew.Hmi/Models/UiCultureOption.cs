namespace AutoScrew.Hmi.Models;

/// <summary>界面语言下拉项（文化代码 + 国旗 + 显示名）。</summary>
public sealed class UiCultureOption
{
    public UiCultureOption(string cultureName, string flagGlyph, string displayName)
    {
        CultureName = cultureName;
        FlagGlyph = flagGlyph;
        DisplayName = displayName;
    }

    public string CultureName { get; }

    public string FlagGlyph { get; }

    public string DisplayName { get; }

    public string DisplayText => $"{FlagGlyph}  {DisplayName}";
}
