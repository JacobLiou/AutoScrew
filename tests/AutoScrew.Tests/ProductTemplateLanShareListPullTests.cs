using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.ProcessLibrary;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplateLanShareListPullTests
{
    [Fact]
    public void ListLanTemplates_OnlyReturnsPnWithTemplateJson()
    {
        var root = CreateTempRoot();
        try
        {
            var dataDir = Path.Combine(root, "data");
            var templateDir = Path.Combine(root, "templates");
            Directory.CreateDirectory(dataDir);
            Directory.CreateDirectory(templateDir);

            // PN with template
            SeedLanTemplate(dataDir, "PN-A", """{"schemaVersion":2,"productId":"PN-A","surfaces":[{},{}]}""");
            // PN with only screws, no template
            var screwsOnly = Path.Combine(dataDir, "process", "PN-B", "screws");
            Directory.CreateDirectory(screwsOnly);
            File.WriteAllText(Path.Combine(screwsOnly, "00.txt"), "x");

            var share = CreateShare(templateDir, dataDir);
            var list = share.ListLanTemplates();

            Assert.Single(list);
            Assert.Equal("PN-A", list[0].PartNumber);
            Assert.Equal(2, list[0].SurfaceCount);
        }
        finally
        {
            TryDelete(root);
        }
    }

    [Fact]
    public async Task PullToLocalAsync_OverwritesExistingLocal()
    {
        var root = CreateTempRoot();
        try
        {
            var dataDir = Path.Combine(root, "data");
            var templateDir = Path.Combine(root, "templates");
            Directory.CreateDirectory(dataDir);
            Directory.CreateDirectory(templateDir);

            SeedLanTemplate(dataDir, "PN-PULL", """{"schemaVersion":2,"productId":"PN-PULL","marker":"from-lan"}""");
            var localFolder = Path.Combine(templateDir, "PN-PULL");
            Directory.CreateDirectory(Path.Combine(localFolder, "images"));
            File.WriteAllText(
                Path.Combine(localFolder, "PN-PULL.product-template.json"),
                """{"schemaVersion":2,"productId":"PN-PULL","marker":"local-old"}""");
            File.WriteAllText(Path.Combine(localFolder, "orphan-local.txt"), "gone");

            var share = CreateShare(templateDir, dataDir);
            var path = await share.PullToLocalAsync("PN-PULL");

            Assert.True(File.Exists(path));
            Assert.Contains("from-lan", await File.ReadAllTextAsync(path));
            Assert.False(File.Exists(Path.Combine(localFolder, "orphan-local.txt")));
        }
        finally
        {
            TryDelete(root);
        }
    }

    private static IProductTemplateLanShare CreateShare(string templateDir, string dataDir)
    {
        var app = Options.Create(new AutoScrewAppOptions
        {
            TemplateDirectory = templateDir,
            DataDirectory = dataDir,
        });
        var mes = new StubMesSettings();
        var lan = new LanShareAccess(mes, app, NullLogger<LanShareAccess>.Instance);
        var process = new ProcessLibraryStore(lan, app, NullLogger<ProcessLibraryStore>.Instance);
        var local = new ProductTemplateLocalStore(app);
        return new ProductTemplateLanShare(process, local, NullLogger<ProductTemplateLanShare>.Instance);
    }

    private static void SeedLanTemplate(string dataDir, string pn, string json)
    {
        var folder = Path.Combine(dataDir, "process", pn, "template");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, $"{pn}.product-template.json"), json);
    }

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "autoscrew-lan-list-pull-" + Guid.NewGuid().ToString("N"));

    private static void TryDelete(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
        catch
        {
        }
    }

    private sealed class StubMesSettings : IMesSettingsService
    {
        public MesRuntimeSettings GetSnapshot() =>
            new() { MesMode = MesProviderMode.ProductKey, UseMockMes = false };

        public Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(GetSnapshot());

        public Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void ApplySnapshot(MesRuntimeSettings settings)
        {
        }
    }
}
