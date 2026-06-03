using System.IO;
using AutoScrew.Hmi.Models;

namespace AutoScrew.Hmi.Services;

public static class ProductTemplateValidator
{
    public static IReadOnlyList<string> Validate(ProductTemplateDocument doc)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(doc.ProductId))
            errors.Add("productId 不能为空。");

        if (doc.Surfaces.Count == 0)
            errors.Add("至少需要一个面。");

        if (doc.SurfaceCount != doc.Surfaces.Count)
            errors.Add($"surfaceCount ({doc.SurfaceCount}) 与 surfaces 数量 ({doc.Surfaces.Count}) 不一致。");

        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var surface in doc.Surfaces)
        {
            if (string.IsNullOrWhiteSpace(surface.SurfaceId))
            {
                errors.Add("存在空的 surfaceId。");
                continue;
            }

            if (!ids.Add(surface.SurfaceId))
                errors.Add($"surfaceId 重复：{surface.SurfaceId}");

            var indexes = new HashSet<int>();
            foreach (var m in surface.Markers)
            {
                if (m.Index < 1)
                    errors.Add($"面 {surface.SurfaceId} 存在无效 index。");
                else if (!indexes.Add(m.Index))
                    errors.Add($"面 {surface.SurfaceId} 的 index {m.Index} 重复。");
            }
        }

        return errors;
    }

    public static IReadOnlyList<string> GetWarnings(ProductTemplateDocument doc, string? templateDirectory)
    {
        var warnings = new List<string>();
        foreach (var surface in doc.Surfaces)
        {
            var hasPath = !string.IsNullOrWhiteSpace(surface.ProductImageRelativePath)
                          || !string.IsNullOrWhiteSpace(surface.ProductImageAbsolutePath);
            if (surface.Markers.Count > 0 && !hasPath)
                warnings.Add($"面 {surface.Name} ({surface.SurfaceId}) 有标注但无底图。");

            if (hasPath && !string.IsNullOrWhiteSpace(templateDirectory))
            {
                var resolved = ProductTemplatePathHelper.ResolveSurfaceImagePath(surface, templateDirectory);
                if (resolved is null || !File.Exists(resolved))
                    warnings.Add($"面 {surface.Name} 底图路径无效或文件不存在。");
            }
        }

        return warnings;
    }
}
