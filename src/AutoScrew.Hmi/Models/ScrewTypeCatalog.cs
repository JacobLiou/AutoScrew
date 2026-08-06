using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Hmi.Models;

/// <summary>
/// 螺钉示意类型（画板视觉直径），由 <c>{DataDirectory}/screw-types.json</c> 配置。
/// </summary>
public static class ScrewTypeCatalog
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static IReadOnlyList<ScrewTypePreset> _all = CreateBuiltInDefaults();
    private static ScrewTypePreset _default = _all.First(t => t.Id == 3);
    private static bool _loaded;

    public static IReadOnlyList<ScrewTypePreset> All => _all;

    public static ScrewTypePreset Default => _default;

    public static ScrewTypePreset? TryGetById(int id) => _all.FirstOrDefault(t => t.Id == id);

    /// <summary>从 DataDirectory 加载；文件不存在则写入默认后加载。非法配置回落内置默认。</summary>
    public static void LoadFromDataDirectory(AutoScrewAppOptions options, ILogger? logger = null)
    {
        var root = ResolveDataRoot(options);
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "screw-types.json");

        try
        {
            if (!File.Exists(path))
            {
                WriteDefaultFile(path);
                logger?.LogInformation("Created default screw-types.json at {Path}", path);
            }

            var text = File.ReadAllText(path);
            var doc = JsonSerializer.Deserialize<ScrewTypesDocument>(text, JsonOptions);
            if (!TryApplyDocument(doc, out var error))
            {
                logger?.LogWarning("Invalid screw-types.json ({Error}); using built-in defaults", error);
                ApplyBuiltIn();
            }
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Failed to load screw-types.json; using built-in defaults");
            ApplyBuiltIn();
        }

        _loaded = true;
    }

    /// <summary>未显式加载时保证可用内置表（设计时/单测）。</summary>
    public static void EnsureLoaded()
    {
        if (_loaded)
            return;
        ApplyBuiltIn();
        _loaded = true;
    }

    private static void ApplyBuiltIn()
    {
        _all = CreateBuiltInDefaults();
        _default = _all.First(t => t.Id == 3);
    }

    private static bool TryApplyDocument(ScrewTypesDocument? doc, out string error)
    {
        error = string.Empty;
        if (doc?.Types is null || doc.Types.Count == 0)
        {
            error = "types empty";
            return false;
        }

        var list = new List<ScrewTypePreset>();
        var seen = new HashSet<int>();
        foreach (var t in doc.Types)
        {
            if (t.Id <= 0)
            {
                error = $"invalid id {t.Id}";
                return false;
            }

            if (!seen.Add(t.Id))
            {
                error = $"duplicate id {t.Id}";
                return false;
            }

            if (t.DiameterPx <= 0)
            {
                error = $"invalid diameterPx for id {t.Id}";
                return false;
            }

            var code = string.IsNullOrWhiteSpace(t.Code) ? t.Id.ToString() : t.Code.Trim();
            var name = string.IsNullOrWhiteSpace(t.DisplayName) ? code : t.DisplayName.Trim();
            list.Add(new ScrewTypePreset(t.Id, code, name, t.DiameterPx));
        }

        list.Sort(static (a, b) => a.Id.CompareTo(b.Id));
        var defaultId = doc.DefaultId > 0 ? doc.DefaultId : list[0].Id;
        var def = list.FirstOrDefault(x => x.Id == defaultId);
        if (def is null)
        {
            error = $"defaultId {defaultId} not in types";
            return false;
        }

        _all = list;
        _default = def;
        return true;
    }

    private static void WriteDefaultFile(string path)
    {
        var doc = new ScrewTypesDocument
        {
            SchemaVersion = 1,
            DefaultId = 3,
            Types = CreateBuiltInDefaults()
                .Select(t => new ScrewTypeEntry
                {
                    Id = t.Id,
                    Code = t.Code,
                    DisplayName = t.DisplayName,
                    DiameterPx = t.DiameterPx,
                })
                .ToList(),
        };
        File.WriteAllText(path, JsonSerializer.Serialize(doc, JsonOptions));
    }

    private static IReadOnlyList<ScrewTypePreset> CreateBuiltInDefaults() =>
    [
        new ScrewTypePreset(1, "M1.0", "M1.0 / 极小", 18),
        new ScrewTypePreset(2, "M1.4", "M1.4 / 很小", 22),
        new ScrewTypePreset(3, "M2", "M2（默认）", 26),
        new ScrewTypePreset(4, "M2.5", "M2.5", 30),
        new ScrewTypePreset(5, "M3", "M3", 34),
        new ScrewTypePreset(6, "M4", "M4 / 较大", 40),
    ];

    private static string ResolveDataRoot(AutoScrewAppOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.DataDirectory))
            return Path.GetFullPath(options.DataDirectory);

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoScrew",
            "data");
    }

    private sealed class ScrewTypesDocument
    {
        public int SchemaVersion { get; set; } = 1;
        public int DefaultId { get; set; } = 3;
        public List<ScrewTypeEntry> Types { get; set; } = [];
    }

    private sealed class ScrewTypeEntry
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public double DiameterPx { get; set; }
    }
}
