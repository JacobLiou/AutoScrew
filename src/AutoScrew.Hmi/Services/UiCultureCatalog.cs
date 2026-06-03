using AutoScrew.Hmi.Models;

namespace AutoScrew.Hmi.Services;

public static class UiCultureCatalog
{
    public static IReadOnlyList<UiCultureOption> CreateOptions() =>
    [
        new(LocalizationService.ZhCn, "🇨🇳", Loc.Get("S.Shell.LanguageZh")),
        new(LocalizationService.EnUs, "🇺🇸", Loc.Get("S.Shell.LanguageEn")),
    ];
}
