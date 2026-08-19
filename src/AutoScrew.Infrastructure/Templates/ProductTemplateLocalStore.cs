using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Templates;

public sealed class ProductTemplateLocalStore : IProductTemplateLocalStore
{
    private readonly IOptions<AutoScrewAppOptions> _options;

    public ProductTemplateLocalStore(IOptions<AutoScrewAppOptions> options) => _options = options;

    public string GetTemplateDirectory()
    {
        var dir = _options.Value.TemplateDirectory;
        if (string.IsNullOrWhiteSpace(dir))
            dir = Path.Combine(AppContext.BaseDirectory, "Templates");
        Directory.CreateDirectory(dir);
        return dir;
    }

    public string GetProductFolder(string partNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(partNumber);
        return Path.Combine(GetTemplateDirectory(), SanitizePartNumber(partNumber));
    }

    public string GetDefaultTemplatePath(string partNumber)
    {
        var pn = SanitizePartNumber(partNumber);
        return Path.Combine(GetProductFolder(partNumber), $"{pn}.product-template.json");
    }

    public void EnsureProductFolder(string partNumber) =>
        Directory.CreateDirectory(GetProductFolder(partNumber));

    public IReadOnlyList<string> ListLocalPartNumbers()
    {
        var root = GetTemplateDirectory();
        if (!Directory.Exists(root))
            return Array.Empty<string>();

        var list = new List<string>();
        foreach (var file in Directory.EnumerateFiles(root, "*.product-template.json", SearchOption.AllDirectories))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name.EndsWith(".product-template", StringComparison.OrdinalIgnoreCase))
                name = name[..^".product-template".Length];
            if (!string.IsNullOrWhiteSpace(name))
                list.Add(name);
        }

        return list.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public string? TryResolveLocalTemplate(string partNumber)
    {
        var path = GetDefaultTemplatePath(partNumber);
        return File.Exists(path) ? path : null;
    }

    public string? TryResolveTemplatePath(string? templateJsonPath)
    {
        if (string.IsNullOrWhiteSpace(templateJsonPath))
            return null;

        if (Path.IsPathRooted(templateJsonPath) && File.Exists(templateJsonPath))
            return templateJsonPath;

        var root = GetTemplateDirectory();
        var combined = Path.Combine(root, templateJsonPath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(combined))
            return combined;

        return File.Exists(templateJsonPath) ? templateJsonPath : null;
    }

    public string ToRelativePath(string absolutePath)
    {
        var root = GetTemplateDirectory().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var full = Path.GetFullPath(absolutePath);
        if (full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            var rel = full[root.Length..].TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return rel.Replace('\\', '/');
        }

        return full;
    }

    public string ToDisplayPath(string absolutePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(absolutePath);

        var full = Path.GetFullPath(absolutePath);
        var templateRoot = Path.GetFullPath(GetTemplateDirectory());
        if (full.StartsWith(templateRoot, StringComparison.OrdinalIgnoreCase))
        {
            var underTemplates = ToRelativePath(absolutePath);
            var appBase = Path.GetFullPath(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (templateRoot.StartsWith(appBase, StringComparison.OrdinalIgnoreCase))
            {
                var templatesSegment = Path.GetRelativePath(appBase, templateRoot).Replace('\\', '/');
                return $"{templatesSegment}/{underTemplates}";
            }

            return underTemplates;
        }

        var baseDir = Path.GetFullPath(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (full.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
            return Path.GetRelativePath(baseDir, full).Replace('\\', '/');

        return full;
    }

    public void SeedFromSamples() =>
        SeedFromSamples(Path.Combine(AppContext.BaseDirectory, "Samples"));

    public void SeedFromSamples(string samplesDirectory)
    {
        if (string.IsNullOrWhiteSpace(samplesDirectory) || !Directory.Exists(samplesDirectory))
            return;

        var target = Path.GetFullPath(GetTemplateDirectory());
        var samples = Path.GetFullPath(samplesDirectory);
        if (string.Equals(target, samples, StringComparison.OrdinalIgnoreCase))
            return;

        Directory.CreateDirectory(target);

        foreach (var file in Directory.EnumerateFiles(samples))
            CopyIfNewer(file, Path.Combine(target, Path.GetFileName(file)));

        foreach (var dir in Directory.EnumerateDirectories(samples))
            CopyDirectoryIfNewer(dir, Path.Combine(target, Path.GetFileName(dir)));
    }

    private static void CopyDirectoryIfNewer(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.EnumerateFiles(source))
            CopyIfNewer(file, Path.Combine(destination, Path.GetFileName(file)));

        foreach (var dir in Directory.EnumerateDirectories(source))
            CopyDirectoryIfNewer(dir, Path.Combine(destination, Path.GetFileName(dir)));
    }

    private static void CopyIfNewer(string sourceFile, string destinationFile)
    {
        if (File.Exists(destinationFile))
        {
            var sourceTime = File.GetLastWriteTimeUtc(sourceFile);
            var destTime = File.GetLastWriteTimeUtc(destinationFile);
            if (sourceTime <= destTime)
                return;
        }

        var destDir = Path.GetDirectoryName(destinationFile);
        if (!string.IsNullOrEmpty(destDir))
            Directory.CreateDirectory(destDir);

        File.Copy(sourceFile, destinationFile, overwrite: true);
        File.SetLastWriteTimeUtc(destinationFile, File.GetLastWriteTimeUtc(sourceFile));
    }

    private static string SanitizePartNumber(string partNumber) =>
        partNumber.Trim();
}
