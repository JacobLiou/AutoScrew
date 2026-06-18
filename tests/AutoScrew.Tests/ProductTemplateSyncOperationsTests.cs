using AutoScrew.Application.Services;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplateSyncOperationsTests
{
    [Fact]
    public void ComputePackageHash_IsStable_ForFolderWithImages()
    {
        var root = CreateFixtureRoot();
        try
        {
            var pnFolder = Path.Combine(root, "PN-TEST");
            Directory.CreateDirectory(Path.Combine(pnFolder, "images"));
            File.WriteAllText(Path.Combine(pnFolder, "PN-TEST.product-template.json"), """{"schemaVersion":2,"productId":"PN-TEST"}""");
            File.WriteAllBytes(Path.Combine(pnFolder, "images", "top.png"), [1, 2, 3, 4]);

            var hash1 = ProductTemplateSyncOperations.ComputePackageHash(pnFolder);
            var hash2 = ProductTemplateSyncOperations.ComputePackageHash(pnFolder);

            Assert.Equal(hash1, hash2);
            Assert.False(string.IsNullOrWhiteSpace(hash1));
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    [Fact]
    public void ComputePackageHash_Changes_WhenImageChanges()
    {
        var root = CreateFixtureRoot();
        try
        {
            var pnFolder = Path.Combine(root, "PN-TEST");
            Directory.CreateDirectory(Path.Combine(pnFolder, "images"));
            File.WriteAllText(Path.Combine(pnFolder, "PN-TEST.product-template.json"), """{"schemaVersion":2,"productId":"PN-TEST"}""");
            var imagePath = Path.Combine(pnFolder, "images", "top.png");
            File.WriteAllBytes(imagePath, [1, 2, 3, 4]);

            var before = ProductTemplateSyncOperations.ComputePackageHash(pnFolder);
            File.WriteAllBytes(imagePath, [9, 8, 7, 6]);
            var after = ProductTemplateSyncOperations.ComputePackageHash(pnFolder);

            Assert.NotEqual(before, after);
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    [Fact]
    public void GetPackageModifiedUtc_UsesLatestFileWriteTime()
    {
        var root = CreateFixtureRoot();
        try
        {
            var pnFolder = Path.Combine(root, "PN-TEST");
            Directory.CreateDirectory(pnFolder);
            var jsonPath = Path.Combine(pnFolder, "PN-TEST.product-template.json");
            File.WriteAllText(jsonPath, "{}");
            File.SetLastWriteTimeUtc(jsonPath, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            var images = Path.Combine(pnFolder, "images");
            Directory.CreateDirectory(images);
            var imagePath = Path.Combine(images, "top.png");
            File.WriteAllBytes(imagePath, [1]);
            File.SetLastWriteTimeUtc(imagePath, new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));

            var modified = ProductTemplatePackageHash.GetPackageModifiedUtc(pnFolder);
            Assert.Equal(new DateTimeOffset(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)), modified);
        }
        finally
        {
            TryDeleteDirectory(root);
        }
    }

    private static string CreateFixtureRoot() =>
        Path.Combine(Path.GetTempPath(), "AutoScrew.Tests", Guid.NewGuid().ToString("N"));

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
        catch
        {
            // Best effort cleanup for temp tests.
        }
    }
}
