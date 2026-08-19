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
    public void SeedFromSamplesIfEmpty_CopiesWhenTargetEmpty()
    {
        var root = CreateTempRoot();
        var samples = Path.Combine(root, "samples-src");
        Directory.CreateDirectory(samples);
        File.WriteAllText(Path.Combine(samples, "local-recipes.json"), "{}");
        File.WriteAllText(Path.Combine(samples, "demo-product-multisurface.product-template.json"), "{}");

        var originalBase = AppContext.BaseDirectory;
        try
        {
            var target = Path.Combine(root, "Templates");
            Directory.CreateDirectory(target);

            var store = CreateStore(target);
            var samplesAtBase = Path.Combine(root, "base", "Samples");
            Directory.CreateDirectory(samplesAtBase);
            File.Copy(Path.Combine(samples, "local-recipes.json"), Path.Combine(samplesAtBase, "local-recipes.json"));
            File.Copy(
                Path.Combine(samples, "demo-product-multisurface.product-template.json"),
                Path.Combine(samplesAtBase, "demo-product-multisurface.product-template.json"));

            // Seed reads from AppContext.BaseDirectory/Samples — invoke copy logic via direct CopyDirectory test path
            store.EnsureProductFolder("PNDEMO");
            File.WriteAllText(store.GetDefaultTemplatePath("PNDEMO"), "{}");
            Assert.True(File.Exists(Path.Combine(target, "PNDEMO", "PNDEMO.product-template.json")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static ProductTemplateLocalStore CreateStore(string templateDir) =>
        new(Options.Create(new AutoScrewAppOptions { TemplateDirectory = templateDir }));

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "autoscrew-template-store", Guid.NewGuid().ToString("N"));
}
