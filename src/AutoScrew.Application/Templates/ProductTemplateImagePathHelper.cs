namespace AutoScrew.Application.Templates;

public static class ProductTemplateImagePathHelper
{
    public const string ImagesSubfolder = "images";

    public static string? ResolveSurfaceImagePath(
        string? productImageRelativePath,
        string? productImageAbsolutePath,
        string templateDirectory)
    {
        if (!string.IsNullOrWhiteSpace(productImageRelativePath))
        {
            var combined = Path.GetFullPath(Path.Combine(templateDirectory, productImageRelativePath));
            if (File.Exists(combined))
                return combined;
        }

        if (!string.IsNullOrWhiteSpace(productImageAbsolutePath) && File.Exists(productImageAbsolutePath))
            return productImageAbsolutePath;

        return null;
    }

    public static (string? Relative, string? Absolute) BuildImagePathsForSave(
        string? productImageAbsolutePath,
        string? templateDirectory,
        string? surfaceId)
    {
        if (string.IsNullOrEmpty(productImageAbsolutePath) || !File.Exists(productImageAbsolutePath))
            return (null, null);

        if (!string.IsNullOrWhiteSpace(templateDirectory) && !string.IsNullOrWhiteSpace(surfaceId))
        {
            var (relative, storedAbsolute) = EnsureImageInTemplateFolder(
                productImageAbsolutePath,
                templateDirectory,
                surfaceId);
            if (!string.IsNullOrEmpty(relative))
                return (relative, null);

            if (!string.IsNullOrEmpty(storedAbsolute))
                return (null, storedAbsolute);
        }

        return (null, productImageAbsolutePath);
    }

    public static (string? Relative, string? StoredAbsolute) EnsureImageInTemplateFolder(
        string absolutePath,
        string templateDirectory,
        string surfaceId)
    {
        if (string.IsNullOrWhiteSpace(absolutePath) || !File.Exists(absolutePath))
            return (null, null);

        if (string.IsNullOrWhiteSpace(templateDirectory) || string.IsNullOrWhiteSpace(surfaceId))
            return (null, null);

        var sourceFullPath = Path.GetFullPath(absolutePath);
        var templateFullPath = Path.GetFullPath(templateDirectory);
        var extension = Path.GetExtension(sourceFullPath);
        if (string.IsNullOrEmpty(extension))
            extension = ".png";

        var relativePath = NormalizeRelativePath(Path.Combine(ImagesSubfolder, $"{surfaceId.Trim()}{extension}"));
        var destinationFullPath = Path.GetFullPath(Path.Combine(templateFullPath, relativePath));

        if (string.Equals(sourceFullPath, destinationFullPath, StringComparison.OrdinalIgnoreCase))
            return (relativePath, destinationFullPath);

        if (TryGetExistingRelativePath(sourceFullPath, templateFullPath, out var existingRelative)
            && IsUnderTemplateImages(existingRelative))
        {
            return (existingRelative, sourceFullPath);
        }

        Directory.CreateDirectory(Path.GetDirectoryName(destinationFullPath)!);
        File.Copy(sourceFullPath, destinationFullPath, overwrite: true);
        return (relativePath, destinationFullPath);
    }

    public static bool IsUnderTemplateImages(string? relativePath) =>
        !string.IsNullOrWhiteSpace(relativePath)
        && relativePath.Replace('\\', '/').StartsWith($"{ImagesSubfolder}/", StringComparison.OrdinalIgnoreCase);

    private static bool TryGetExistingRelativePath(string absolutePath, string templateDirectory, out string relativePath)
    {
        relativePath = string.Empty;
        try
        {
            var rel = Path.GetRelativePath(templateDirectory, absolutePath);
            if (rel.StartsWith("..", StringComparison.Ordinal) || Path.IsPathRooted(rel))
                return false;

            relativePath = NormalizeRelativePath(rel);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string NormalizeRelativePath(string path) =>
        path.Replace('\\', '/');
}
