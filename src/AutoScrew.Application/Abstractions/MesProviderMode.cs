namespace AutoScrew.Application.Abstractions;

/// <summary>MES / SN 校验后端：Mock、占位 REST、Opcenter ProductKey HTTP。</summary>
public static class MesProviderMode
{
    public const string Mock = "Mock";
    public const string LegacyHttp = "LegacyHttp";
    public const string ProductKey = "ProductKey";

    public static bool IsKnown(string? mode) =>
        string.Equals(mode, Mock, StringComparison.OrdinalIgnoreCase)
        || string.Equals(mode, LegacyHttp, StringComparison.OrdinalIgnoreCase)
        || string.Equals(mode, ProductKey, StringComparison.OrdinalIgnoreCase);

    public static string Normalize(string? mode, bool useMockMesFallback)
    {
        if (IsKnown(mode))
            return mode!.Equals(Mock, StringComparison.OrdinalIgnoreCase) ? Mock
                : mode.Equals(LegacyHttp, StringComparison.OrdinalIgnoreCase) ? LegacyHttp
                : ProductKey;

        return useMockMesFallback ? Mock : LegacyHttp;
    }
}
