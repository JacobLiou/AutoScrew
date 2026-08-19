using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplateLocalStoreTests
{
    [Fact]
    public void GetDefaultTemplatePath_UsesPnSubfolder()
    {
        var root = CreateTempRoot();
        try
        {
            var store = CreateStore(root);
            var path = store.GetDefaultTemplatePath("PN-ABC");

            Assert.EndsWith(Path.Combine("PN-ABC", "PN-ABC.product-template.json"), path);
            Assert.StartsWith(root, path, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void EnsureProductFolder_CreatesDirectory()
    {
        var root = CreateTempRoot();
        try
        {
            var store = CreateStore(root);
            store.EnsureProductFolder("PN-X");
            Assert.True(Directory.Exists(store.GetProductFolder("PN-X")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void TryResolveLocalTemplate_FindsDefaultJson()
    {
        var root = CreateTempRoot();
        try
        {
            var store = CreateStore(root);
            store.EnsureProductFolder("PN-1");
            var path = store.GetDefaultTemplatePath("PN-1");
            File.WriteAllText(path, "{}");

            var resolved = store.TryResolveLocalTemplate("PN-1");
            Assert.Equal(path, resolved);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void TryResolveTemplatePath_ResolvesRelativeToRoot()
    {
        var root = CreateTempRoot();
        try
        {
            var store = CreateStore(root);
            var relDir = Path.Combine(root, "PN-2");
            Directory.CreateDirectory(relDir);
            var file = Path.Combine(relDir, "PN-2.product-template.json");
            File.WriteAllText(file, "{}");

            var resolved = store.TryResolveTemplatePath("PN-2/PN-2.product-template.json");
            Assert.Equal(file, resolved);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void SeedFromSamples_CopiesWhenTargetEmpty()
    {
        var root = CreateTempRoot();
        try
        {
            var samples = CreateSamplePn(root, "PNDEMO", "sample");
            var target = Path.Combine(root, "Templates");
            Directory.CreateDirectory(target);

            var store = CreateStore(target);
            store.SeedFromSamples(samples);

            var dest = Path.Combine(target, "PNDEMO", "PNDEMO.product-template.json");
            Assert.True(File.Exists(dest));
            Assert.Equal("sample", File.ReadAllText(dest));
            Assert.True(File.Exists(Path.Combine(target, "local-recipes.json")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void SeedFromSamples_OverwritesExistingPnWhenSampleNewer()
    {
        var root = CreateTempRoot();
        try
        {
            var samples = CreateSamplePn(root, "PNDEMO", "newer-sample");
            var sampleFile = Path.Combine(samples, "PNDEMO", "PNDEMO.product-template.json");
            File.SetLastWriteTimeUtc(sampleFile, DateTime.UtcNow.AddDays(-1));

            var target = Path.Combine(root, "Templates");
            var dest = Path.Combine(target, "PNDEMO", "PNDEMO.product-template.json");
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.WriteAllText(dest, "stale");
            File.SetLastWriteTimeUtc(dest, DateTime.UtcNow.AddDays(-3));

            CreateStore(target).SeedFromSamples(samples);

            Assert.Equal("newer-sample", File.ReadAllText(dest));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void SeedFromSamples_KeepsExistingPnWhenDestNewer()
    {
        var root = CreateTempRoot();
        try
        {
            var samples = CreateSamplePn(root, "PNDEMO", "sample");
            var sampleFile = Path.Combine(samples, "PNDEMO", "PNDEMO.product-template.json");
            File.SetLastWriteTimeUtc(sampleFile, DateTime.UtcNow.AddDays(-2));

            var target = Path.Combine(root, "Templates");
            var dest = Path.Combine(target, "PNDEMO", "PNDEMO.product-template.json");
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.WriteAllText(dest, "technician-edit");
            File.SetLastWriteTimeUtc(dest, DateTime.UtcNow.AddDays(-1));

            CreateStore(target).SeedFromSamples(samples);

            Assert.Equal("technician-edit", File.ReadAllText(dest));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void SeedFromSamples_AddsMissingPnAlongsideExisting()
    {
        var root = CreateTempRoot();
        try
        {
            var samples = CreateSamplePn(root, "PNDEMO", "demo");
            var target = Path.Combine(root, "Templates");
            var custom = Path.Combine(target, "PN-CUSTOM", "PN-CUSTOM.product-template.json");
            Directory.CreateDirectory(Path.GetDirectoryName(custom)!);
            File.WriteAllText(custom, "keep-me");

            CreateStore(target).SeedFromSamples(samples);

            Assert.Equal("keep-me", File.ReadAllText(custom));
            Assert.Equal("demo", File.ReadAllText(Path.Combine(target, "PNDEMO", "PNDEMO.product-template.json")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void SeedFromSamples_NoOpWhenSamplesIsTemplateDirectory()
    {
        var root = CreateTempRoot();
        try
        {
            var samples = CreateSamplePn(root, "PNDEMO", "same");
            var store = CreateStore(samples);
            store.SeedFromSamples(samples);

            Assert.Equal("same", File.ReadAllText(Path.Combine(samples, "PNDEMO", "PNDEMO.product-template.json")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string CreateSamplePn(string root, string partNumber, string templateContents)
    {
        var samples = Path.Combine(root, "Samples");
        var pnDir = Path.Combine(samples, partNumber);
        Directory.CreateDirectory(pnDir);
        File.WriteAllText(Path.Combine(samples, "local-recipes.json"), "{}");
        File.WriteAllText(Path.Combine(pnDir, $"{partNumber}.product-template.json"), templateContents);
        return samples;
    }

    private static ProductTemplateLocalStore CreateStore(string templateDir) =>
        new(Options.Create(new AutoScrewAppOptions { TemplateDirectory = templateDir }));

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "autoscrew-template-store", Guid.NewGuid().ToString("N"));
}
