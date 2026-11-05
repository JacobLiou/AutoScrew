using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.ProcessLibrary;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProcessLibraryProductCrudTests
{
    [Fact]
    public async Task CreateProduct_CreatesEmptyDirectoryAndManifest()
    {
        var dataDir = CreateTempDir();
        try
        {
            var store = CreateStore(dataDir);
            await store.CreateProductAsync("PN-EMPTY", CancellationToken.None);

            var productDir = Path.Combine(dataDir, "process", "PN-EMPTY");
            Assert.True(Directory.Exists(productDir));
            Assert.True(File.Exists(Path.Combine(productDir, "product.json")));

            var summary = store.LoadProduct("PN-EMPTY");
            Assert.NotNull(summary);
            Assert.Equal("PN-EMPTY", summary!.ProductPn);
            Assert.Empty(summary.Slots);
            Assert.Empty(summary.Sequences);
            Assert.Contains("PN-EMPTY", store.ListProductPns(), StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task CreateProduct_WhenAlreadyExists_Throws()
    {
        var dataDir = CreateTempDir();
        try
        {
            var store = CreateStore(dataDir);
            await store.CreateProductAsync("PN-DUP", CancellationToken.None);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                store.CreateProductAsync("PN-DUP", CancellationToken.None));
            Assert.Contains("已存在", ex.Message);
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task DeleteProduct_RemovesTemplateParamsAndSequences()
    {
        var dataDir = CreateTempDir();
        try
        {
            var pn = "PN-DEL";
            var productDir = Path.Combine(dataDir, "process", pn);
            Directory.CreateDirectory(Path.Combine(productDir, "screws"));
            Directory.CreateDirectory(Path.Combine(productDir, "sequences"));
            Directory.CreateDirectory(Path.Combine(productDir, "template"));
            File.WriteAllText(Path.Combine(productDir, "product.json"), "{\"productPn\":\"PN-DEL\",\"slots\":[],\"sequences\":[]}");
            File.WriteAllText(Path.Combine(productDir, "screws", "00.txt"), "x");
            File.WriteAllText(Path.Combine(productDir, "sequences", "01.json"), "{}");
            File.WriteAllText(Path.Combine(productDir, "template", "PN-DEL.product-template.json"), "{}");

            var store = CreateStore(dataDir);
            store.DeleteProduct(pn);

            Assert.False(Directory.Exists(productDir));
            Assert.DoesNotContain(pn, store.ListProductPns(), StringComparer.OrdinalIgnoreCase);
            Assert.False(Directory.Exists(Path.Combine(productDir, "template")));
            Assert.False(Directory.Exists(Path.Combine(productDir, "screws")));
            Assert.False(Directory.Exists(Path.Combine(productDir, "sequences")));
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public void DeleteProduct_WhenMissing_DoesNotThrow()
    {
        var dataDir = CreateTempDir();
        try
        {
            var store = CreateStore(dataDir);
            store.DeleteProduct("PN-GONE");
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    private static ProcessLibraryStore CreateStore(string dataDir)
    {
        var app = Options.Create(new AutoScrewAppOptions { DataDirectory = dataDir });
        var mes = new StubMesSettings();
        var lan = new LanShareAccess(mes, app, NullLogger<LanShareAccess>.Instance);
        return new ProcessLibraryStore(lan, app, NullLogger<ProcessLibraryStore>.Instance);
    }

    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "autoscrew-pn-crud-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void DeleteDir(string dir)
    {
        if (Directory.Exists(dir))
            Directory.Delete(dir, recursive: true);
    }

    private sealed class StubMesSettings : IMesSettingsService
    {
        public MesRuntimeSettings GetSnapshot() => new() { MesMode = MesProviderMode.Mock, UseMockMes = true };

        public Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(GetSnapshot());

        public Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void ApplySnapshot(MesRuntimeSettings settings)
        {
        }
    }
}
