using System.Text.Json;
using System.Text.Json.Serialization;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Mes;

public sealed class LocalJsonRecipeStore
{
    public const string FileName = "local-recipes.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly IOptions<AutoScrewAppOptions> _options;

    public LocalJsonRecipeStore(IOptions<AutoScrewAppOptions> options) => _options = options;

    public async Task<LocalRecipeLoadResult> LoadAsync(CancellationToken cancellationToken = default)
    {
        var path = ResolvePath();
        if (path is null || !File.Exists(path))
            return LocalRecipeLoadResult.NotFound();

        cancellationToken.ThrowIfCancellationRequested();
        var json = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        var doc = JsonSerializer.Deserialize<LocalRecipeDocument>(json, JsonOptions) ?? new LocalRecipeDocument();
        Normalize(doc);
        return LocalRecipeLoadResult.Found(doc, path);
    }

    public string? ResolvePath()
    {
        var templateDir = _options.Value.TemplateDirectory;
        if (!string.IsNullOrWhiteSpace(templateDir))
        {
            var inTemplate = Path.Combine(templateDir, FileName);
            if (File.Exists(inTemplate))
                return inTemplate;
        }

        var dataDir = ResolveDataDirectory();
        var inData = Path.Combine(dataDir, FileName);
        return File.Exists(inData) ? inData : null;
    }

    public string GetDefaultWritePath()
    {
        var templateDir = _options.Value.TemplateDirectory;
        if (!string.IsNullOrWhiteSpace(templateDir))
        {
            Directory.CreateDirectory(templateDir);
            return Path.Combine(templateDir, FileName);
        }

        var dataDir = ResolveDataDirectory();
        Directory.CreateDirectory(dataDir);
        return Path.Combine(dataDir, FileName);
    }

    public async Task SaveAsync(LocalRecipeDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        Normalize(document);
        var path = GetDefaultWritePath();
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, document, JsonOptions, cancellationToken).ConfigureAwait(false);
    }

    private string ResolveDataDirectory()
    {
        var root = _options.Value.DataDirectory;
        if (string.IsNullOrWhiteSpace(root))
            root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data");
        return root;
    }

    private static void Normalize(LocalRecipeDocument doc)
    {
        foreach (var product in doc.Products)
        {
            product.PartNumber = product.PartNumber.Trim();
            if (string.IsNullOrWhiteSpace(product.TemplateFile))
                product.TemplateFile = $"{product.PartNumber}.product-template.json";

            product.SerialNumbers = product.SerialNumbers
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}

public sealed class LocalRecipeLoadResult
{
    public bool Exists { get; init; }

    public LocalRecipeDocument Document { get; init; } = new();

    public string? FilePath { get; init; }

    public static LocalRecipeLoadResult NotFound() => new() { Exists = false };

    public static LocalRecipeLoadResult Found(LocalRecipeDocument doc, string path) =>
        new() { Exists = true, Document = doc, FilePath = path };
}
