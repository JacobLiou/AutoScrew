using AutoScrew.Application.Templates;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplatePathHelperTests : IDisposable
{
    private readonly string _root;
    private readonly List<string> _cleanup = new();

    public ProductTemplatePathHelperTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "autoscrew-template-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
        _cleanup.Add(_root);
    }

    public void Dispose()
    {
        foreach (var path in _cleanup)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive: true);
            }
            catch
            {
                // best effort
            }
        }
    }

    [Fact]
    public void EnsureImageInTemplateFolder_CopiesExternalImageToSurfaceNamedFile()
    {
        var templateDir = Path.Combine(_root, "PN-1");
        Directory.CreateDirectory(templateDir);
        var external = Path.Combine(_root, "external-board.png");
        File.WriteAllBytes(external, [1, 2, 3]);

        var (relative, storedAbsolute) = ProductTemplateImagePathHelper.EnsureImageInTemplateFolder(
            external,
            templateDir,
            "TOP");

        Assert.Equal("images/TOP.png", relative);
        Assert.NotNull(storedAbsolute);
        Assert.True(File.Exists(storedAbsolute));
        Assert.Equal(File.ReadAllBytes(external), File.ReadAllBytes(storedAbsolute!));
    }

    [Fact]
    public void EnsureImageInTemplateFolder_KeepsExistingImagesRelativePath()
    {
        var templateDir = Path.Combine(_root, "PN-DEMO");
        var imagesDir = Path.Combine(templateDir, "images");
        Directory.CreateDirectory(imagesDir);
        var boardPath = Path.Combine(imagesDir, "board.png");
        File.WriteAllBytes(boardPath, [4, 5, 6]);

        var (relative, storedAbsolute) = ProductTemplateImagePathHelper.EnsureImageInTemplateFolder(
            boardPath,
            templateDir,
            "S1");

        Assert.Equal("images/board.png", relative);
        Assert.Equal(boardPath, storedAbsolute);
    }

    [Fact]
    public void BuildImagePathsForSave_WritesRelativePathOnly()
    {
        var templateDir = Path.Combine(_root, "PN-2");
        Directory.CreateDirectory(templateDir);
        var external = Path.Combine(_root, "picked.jpg");
        File.WriteAllBytes(external, [7, 8, 9]);

        var (relative, absolute) = ProductTemplateImagePathHelper.BuildImagePathsForSave(
            external,
            templateDir,
            "S1");

        Assert.Equal("images/S1.jpg", relative);
        Assert.Null(absolute);
        Assert.True(File.Exists(Path.Combine(templateDir, "images", "S1.jpg")));
    }

    [Fact]
    public void BuildImagePathsForSave_FallsBackToAbsoluteWhenTemplateDirectoryMissing()
    {
        var external = Path.Combine(_root, "orphan.png");
        File.WriteAllBytes(external, [1]);

        var (relative, absolute) = ProductTemplateImagePathHelper.BuildImagePathsForSave(
            external,
            templateDirectory: null,
            surfaceId: "S1");

        Assert.Null(relative);
        Assert.Equal(external, absolute);
    }

    [Fact]
    public void IsUnderTemplateImages_RecognizesImagesPrefix()
    {
        Assert.True(ProductTemplateImagePathHelper.IsUnderTemplateImages("images/board.png"));
        Assert.False(ProductTemplateImagePathHelper.IsUnderTemplateImages("board.png"));
    }
}
