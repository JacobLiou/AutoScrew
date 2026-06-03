using System.Globalization;
using System.IO;
using System.Windows;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Hmi.Services;

public sealed class LocalizationService
{
    public const string ZhCn = "zh-CN";
    public const string EnUs = "en-US";

    private readonly ILogger<LocalizationService> _logger;
    private readonly string _preferencesPath;
    private string _cultureName = ZhCn;
    private ResourceDictionary? _stringsDictionary;

    public LocalizationService(ILogger<LocalizationService> logger, IOptions<AutoScrewAppOptions> options)
    {
        _logger = logger;
        var dataDir = options.Value.DataDirectory;
        if (string.IsNullOrWhiteSpace(dataDir))
            dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew");
        Directory.CreateDirectory(dataDir);
        _preferencesPath = Path.Combine(dataDir, "ui-culture.txt");
    }

    public event EventHandler? CultureChanged;

    public string CurrentCultureName => _cultureName;

    public IReadOnlyList<string> SupportedCultures { get; } = [ZhCn, EnUs];

    public void Initialize(string? cultureFromConfig)
    {
        var saved = TryLoadSavedCulture();
        var culture = NormalizeCulture(saved ?? cultureFromConfig ?? ZhCn);
        ApplyCulture(culture, persist: false);
    }

    public void SetCulture(string cultureName, bool persist = true)
    {
        var culture = NormalizeCulture(cultureName);
        if (string.Equals(culture, _cultureName, StringComparison.OrdinalIgnoreCase))
            return;

        ApplyCulture(culture, persist);
        CultureChanged?.Invoke(this, EventArgs.Empty);
    }

    public string GetString(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        if (_stringsDictionary?.Contains(key) == true && _stringsDictionary[key] is string s)
            return s;

        if (System.Windows.Application.Current?.TryFindResource(key) is string app)
            return app;

#if DEBUG
        _logger.LogWarning("Missing localization key: {Key}", key);
#endif
        return $"[{key}]";
    }

    public string Format(string key, params object[] args) =>
        string.Format(CultureInfo.CurrentCulture, GetString(key), args);

    private void ApplyCulture(string culture, bool persist)
    {
        _cultureName = culture;
        var cultureInfo = CultureInfo.GetCultureInfo(culture);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        ReplaceStringsDictionary(culture);

        if (persist)
            SaveCulture(culture);
    }

    private void ReplaceStringsDictionary(string culture)
    {
        var app = System.Windows.Application.Current
                  ?? throw new InvalidOperationException("Application not initialized.");

        var uri = new Uri($"/Themes/Strings.{culture}.xaml", UriKind.Relative);
        var newDict = new ResourceDictionary { Source = uri };

        var merged = app.Resources.MergedDictionaries;
        ResourceDictionary? existing = null;
        for (var i = merged.Count - 1; i >= 0; i--)
        {
            var src = merged[i].Source?.OriginalString ?? "";
            if (src.Contains("Strings.", StringComparison.OrdinalIgnoreCase)
                && src.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
            {
                existing = merged[i];
                merged.RemoveAt(i);
                break;
            }
        }

        var insertIndex = 0;
        for (var i = 0; i < merged.Count; i++)
        {
            var src = merged[i].Source?.OriginalString ?? "";
            if (src.Contains("ThemesDictionary", StringComparison.OrdinalIgnoreCase)
                || src.Contains("ControlsDictionary", StringComparison.OrdinalIgnoreCase))
            {
                insertIndex = i + 1;
            }
        }

        merged.Insert(insertIndex, newDict);
        _stringsDictionary = newDict;
    }

    private static string NormalizeCulture(string? name) =>
        string.Equals(name, EnUs, StringComparison.OrdinalIgnoreCase) ? EnUs : ZhCn;

    private string? TryLoadSavedCulture()
    {
        try
        {
            if (!File.Exists(_preferencesPath))
                return null;
            return File.ReadAllText(_preferencesPath).Trim();
        }
        catch
        {
            return null;
        }
    }

    private void SaveCulture(string culture)
    {
        try
        {
            File.WriteAllText(_preferencesPath, culture);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save UI culture preference.");
        }
    }
}

/// <summary>静态访问器，供对话框与 VM 使用。</summary>
public static class Loc
{
    private static LocalizationService? _service;

    public static void Initialize(LocalizationService service) => _service = service;

    public static LocalizationService Service =>
        _service ?? throw new InvalidOperationException("LocalizationService not initialized.");

    public static string Get(string key) => Service.GetString(key);

    public static string Format(string key, params object[] args) => Service.Format(key, args);
}
