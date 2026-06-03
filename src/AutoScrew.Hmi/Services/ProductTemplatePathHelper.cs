using System.IO;
using AutoScrew.Hmi.Models;

namespace AutoScrew.Hmi.Services;

public static class ProductTemplatePathHelper
{
    public static string? ResolveSurfaceImagePath(SurfaceLayoutDocument surface, string templateDirectory)
    {
        if (!string.IsNullOrWhiteSpace(surface.ProductImageRelativePath))
        {
            var combined = Path.GetFullPath(Path.Combine(templateDirectory, surface.ProductImageRelativePath));
            if (File.Exists(combined))
                return combined;
        }

        if (!string.IsNullOrWhiteSpace(surface.ProductImageAbsolutePath) && File.Exists(surface.ProductImageAbsolutePath))
            return surface.ProductImageAbsolutePath;

        return null;
    }

    public static (string? Relative, string? Absolute) BuildImagePathsForSave(
        string? productImageAbsolutePath,
        string? templateDirectory)
    {
        if (string.IsNullOrEmpty(productImageAbsolutePath) || !File.Exists(productImageAbsolutePath))
            return (null, null);

        if (!string.IsNullOrEmpty(templateDirectory))
        {
            try
            {
                var rel = Path.GetRelativePath(templateDirectory, productImageAbsolutePath);
                if (!rel.StartsWith("..", StringComparison.Ordinal) && !Path.IsPathRooted(rel))
                    return (rel, null);
            }
            catch
            {
                // fall through
            }
        }

        return (null, productImageAbsolutePath);
    }
}
