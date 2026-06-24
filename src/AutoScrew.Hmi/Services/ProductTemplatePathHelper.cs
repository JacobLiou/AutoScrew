using AutoScrew.Application.Templates;
using AutoScrew.Hmi.Models;

namespace AutoScrew.Hmi.Services;

public static class ProductTemplatePathHelper
{
    public const string ImagesSubfolder = ProductTemplateImagePathHelper.ImagesSubfolder;

    public static string? ResolveSurfaceImagePath(SurfaceLayoutDocument surface, string templateDirectory) =>
        ProductTemplateImagePathHelper.ResolveSurfaceImagePath(
            surface.ProductImageRelativePath,
            surface.ProductImageAbsolutePath,
            templateDirectory);

    public static (string? Relative, string? Absolute) BuildImagePathsForSave(
        string? productImageAbsolutePath,
        string? templateDirectory,
        string? surfaceId) =>
        ProductTemplateImagePathHelper.BuildImagePathsForSave(productImageAbsolutePath, templateDirectory, surfaceId);

    public static (string? Relative, string? StoredAbsolute) EnsureImageInTemplateFolder(
        string absolutePath,
        string templateDirectory,
        string surfaceId) =>
        ProductTemplateImagePathHelper.EnsureImageInTemplateFolder(absolutePath, templateDirectory, surfaceId);

    public static bool IsUnderTemplateImages(string? relativePath) =>
        ProductTemplateImagePathHelper.IsUnderTemplateImages(relativePath);
}
