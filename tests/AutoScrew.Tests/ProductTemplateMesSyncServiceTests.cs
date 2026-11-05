using System.Net;
using System.Text;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.Mes;
using AutoScrew.Infrastructure.Persistence;
using AutoScrew.Infrastructure.ProcessLibrary;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplateMesSyncServiceTests
{
    [Fact]
    public async Task SyncWithMesAsync_Mock_DoesNotTransfer()
    {
        var root = CreateTempRoot();
        var handler = new RoutingHttpHandler();
        var services = BuildServices(root, handler, MesProviderMode.Mock);
        await using var provider = services.BuildServiceProvider();

        SeedLocalTemplate(root, "PN-1", "v1");
        handler.PostCount = 0;

        var sync = provider.GetRequiredService<IProductTemplateMesSyncService>();
        var result = await sync.SyncWithMesAsync("PN-1");

        Assert.Equal(0, result.UploadedCount);
        Assert.Equal(0, result.DownloadedCount);
        Assert.Empty(result.Errors);
        Assert.Contains("Mock", result.SummaryMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, handler.PostCount);

        TryDeleteDirectory(root);
    }

    [Fact]
    public async Task SyncWithMesAsync_LegacyHttp_UploadsCurrentPn()
    {
        var root = CreateTempRoot();
        var handler = new RoutingHttpHandler();
        var services = BuildServices(root, handler, MesProviderMode.LegacyHttp);
        await using var provider = services.BuildServiceProvider();

        SeedLocalTemplate(root, "PN-1", "v1");
        var localStore = provider.GetRequiredService<IProductTemplateLocalStore>();
        var syncRepo = provider.GetRequiredService<IProductTemplateSyncRepository>();

        await ProductTemplateSyncOperations.UpsertFromFileAsync(
            syncRepo,
            localStore,
            "PN-1",
            ProductTemplateSyncState.PendingUpload,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        var sync = provider.GetRequiredService<IProductTemplateMesSyncService>();
        var result = await sync.SyncWithMesAsync("PN-1");

        Assert.Equal(1, result.UploadedCount);
        Assert.Equal(0, result.DownloadedCount);
        Assert.Empty(result.Errors);
        Assert.Equal(1, handler.PostCount);

        var pn1 = await syncRepo.GetAsync("PN-1");
        Assert.Equal(ProductTemplateSyncState.Synced, pn1!.SyncState);

        TryDeleteDirectory(root);
    }

    [Fact]
    public async Task SyncWithMesAsync_ProductKey_PublishesToProcessTemplateFolder()
    {
        var root = CreateTempRoot();
        var dataDir = Path.Combine(root, "data");
        var templateRoot = Path.Combine(root, "templates");
        Directory.CreateDirectory(dataDir);
        Directory.CreateDirectory(templateRoot);

        var handler = new RoutingHttpHandler();
        var services = BuildServices(templateRoot, handler, MesProviderMode.ProductKey, dataDir);
        await using var provider = services.BuildServiceProvider();

        SeedLocalTemplate(templateRoot, "PN-LAN", "lan-v1");

        var sync = provider.GetRequiredService<IProductTemplateMesSyncService>();
        var result = await sync.SyncWithMesAsync("PN-LAN");

        Assert.Equal(1, result.UploadedCount);
        Assert.Empty(result.Errors);
        Assert.Contains("created", result.SummaryMessage, StringComparison.OrdinalIgnoreCase);

        var published = Path.Combine(dataDir, "process", "PN-LAN", "template", "PN-LAN.product-template.json");
        Assert.True(File.Exists(published));
        Assert.Contains("lan-v1", await File.ReadAllTextAsync(published));

        TryDeleteDirectory(root);
    }

    [Fact]
    public async Task SyncWithMesAsync_ProductKey_UpdatesLan_WhenLocalChanged()
    {
        var root = CreateTempRoot();
        var dataDir = Path.Combine(root, "data");
        var templateRoot = Path.Combine(root, "templates");
        Directory.CreateDirectory(dataDir);
        Directory.CreateDirectory(templateRoot);

        var handler = new RoutingHttpHandler();
        var services = BuildServices(templateRoot, handler, MesProviderMode.ProductKey, dataDir);
        await using var provider = services.BuildServiceProvider();

        SeedLocalTemplate(templateRoot, "PN-UPD", "v1");
        var sync = provider.GetRequiredService<IProductTemplateMesSyncService>();
        await sync.SyncWithMesAsync("PN-UPD");

        // LAN leftover orphan that should be removed on update
        var lanFolder = Path.Combine(dataDir, "process", "PN-UPD", "template");
        File.WriteAllText(Path.Combine(lanFolder, "orphan.txt"), "stale");

        SeedLocalTemplate(templateRoot, "PN-UPD", "v2-local-newer");
        var result = await sync.SyncWithMesAsync("PN-UPD");

        Assert.Equal(1, result.UploadedCount);
        Assert.Equal(0, result.SkippedCount);
        Assert.Contains("updated", result.SummaryMessage, StringComparison.OrdinalIgnoreCase);

        var published = Path.Combine(lanFolder, "PN-UPD.product-template.json");
        Assert.Contains("v2-local-newer", await File.ReadAllTextAsync(published));
        Assert.False(File.Exists(Path.Combine(lanFolder, "orphan.txt")));

        TryDeleteDirectory(root);
    }

    [Fact]
    public async Task SyncWithMesAsync_ProductKey_Skips_WhenLanAlreadySame()
    {
        var root = CreateTempRoot();
        var dataDir = Path.Combine(root, "data");
        var templateRoot = Path.Combine(root, "templates");
        Directory.CreateDirectory(dataDir);
        Directory.CreateDirectory(templateRoot);

        var handler = new RoutingHttpHandler();
        var services = BuildServices(templateRoot, handler, MesProviderMode.ProductKey, dataDir);
        await using var provider = services.BuildServiceProvider();

        SeedLocalTemplate(templateRoot, "PN-SAME", "same");
        var sync = provider.GetRequiredService<IProductTemplateMesSyncService>();
        await sync.SyncWithMesAsync("PN-SAME");

        var again = await sync.SyncWithMesAsync("PN-SAME");
        Assert.Equal(0, again.UploadedCount);
        Assert.Equal(1, again.SkippedCount);
        Assert.Contains("unchanged", again.SummaryMessage, StringComparison.OrdinalIgnoreCase);

        TryDeleteDirectory(root);
    }

    [Fact]
    public async Task UpsertFromFileAsync_MarksPendingUpload_WhenOnlyImageChanges()
    {
        var root = CreateTempRoot();
        var handler = new RoutingHttpHandler();
        var services = BuildServices(root, handler, MesProviderMode.LegacyHttp);
        await using var provider = services.BuildServiceProvider();

        SeedLocalTemplate(root, "PN-IMG", "same-json");
        var localStore = provider.GetRequiredService<IProductTemplateLocalStore>();
        var syncRepo = provider.GetRequiredService<IProductTemplateSyncRepository>();

        await ProductTemplateSyncOperations.UpsertFromFileAsync(
            syncRepo,
            localStore,
            "PN-IMG",
            ProductTemplateSyncState.Synced,
            null,
            null,
            DateTimeOffset.UtcNow,
            "rev-1",
            CancellationToken.None);

        var before = await syncRepo.GetAsync("PN-IMG");
        var imagePath = Path.Combine(localStore.GetProductFolder("PN-IMG"), "images", "top.png");
        File.WriteAllBytes(imagePath, [9, 9, 9]);

        await ProductTemplateSyncOperations.UpsertFromFileAsync(
            syncRepo,
            localStore,
            "PN-IMG",
            ProductTemplateSyncState.PendingUpload,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        var after = await syncRepo.GetAsync("PN-IMG");
        Assert.Equal(ProductTemplateSyncState.PendingUpload, after!.SyncState);
        Assert.NotEqual(before!.LocalFileHash, after.LocalFileHash);

        TryDeleteDirectory(root);
    }

    private static ServiceCollection BuildServices(
        string templateRoot,
        RoutingHttpHandler handler,
        string mesMode,
        string? dataDirectory = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpClientFactory>(new StubHttpClientFactory(handler));
        services.AddSingleton(new MesRuntimeSettings
        {
            UseMockMes = mesMode == MesProviderMode.Mock,
            MesMode = mesMode,
            BaseUrl = "http://localhost:5080/",
        });
        services.AddSingleton<IMesSettingsService>(sp =>
        {
            var settings = sp.GetRequiredService<MesRuntimeSettings>();
            return new FixedMesSettingsService(settings);
        });

        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        using (var db = new AppDbContext(options))
        {
            db.Database.EnsureCreated();
        }

        services.AddSingleton<IDbContextFactory<AppDbContext>>(new TestDbContextFactory(options));
        services.Configure<AutoScrewAppOptions>(o =>
        {
            o.TemplateDirectory = templateRoot;
            o.DataDirectory = dataDirectory ?? Path.Combine(templateRoot, "_data");
        });

        services.AddSingleton<ProductTemplateLocalStore>();
        services.AddSingleton<IProductTemplateLocalStore>(sp => sp.GetRequiredService<ProductTemplateLocalStore>());
        services.AddSingleton<IProductTemplateSyncRepository, EfProductTemplateSyncRepository>();
        services.AddSingleton<LanShareAccess>();
        services.AddSingleton<ProcessLibraryStore>();
        services.AddSingleton<IProductTemplateLanShare, ProductTemplateLanShare>();
        services.AddSingleton<IMesTemplateUploadService, MesTemplateUploadService>();
        services.AddSingleton<IMesTemplateCatalogClient, MesTemplateCatalogClient>();
        services.AddSingleton<IMesTemplatePackageClient, MesTemplatePackageClient>();
        services.AddSingleton<IProductTemplateMesSyncService, ProductTemplateMesSyncService>();
        return services;
    }

    private static void SeedLocalTemplate(string root, string pn, string marker)
    {
        var folder = Path.Combine(root, pn);
        Directory.CreateDirectory(Path.Combine(folder, "images"));
        File.WriteAllText(
            Path.Combine(folder, $"{pn}.product-template.json"),
            $$"""{"schemaVersion":2,"productId":"{{pn}}","marker":"{{marker}}"}""");
        File.WriteAllBytes(Path.Combine(folder, "images", "top.png"), [1, 2, 3]);
    }

    private static string CreateTempRoot() =>
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
        }
    }

    private sealed class RoutingHttpHandler : HttpMessageHandler
    {
        public int PostCount { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.AbsolutePath;

            if (path.StartsWith("/api/templates/", StringComparison.OrdinalIgnoreCase)
                && path.EndsWith("/package", StringComparison.OrdinalIgnoreCase)
                && request.Method == HttpMethod.Post)
            {
                PostCount++;
                return request.Content!.ReadAsByteArrayAsync(cancellationToken).ContinueWith(t =>
                {
                    var body = t.Result;
                    var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(body));
                    var json = $$"""{"accepted":true,"revision":"test-rev","contentHash":"{{hash}}"}""";
                    return Json(HttpStatusCode.OK, json);
                }, cancellationToken);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        private static HttpResponseMessage Json(HttpStatusCode code, string json) =>
            new(code) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
    }

    private sealed class StubHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    private sealed class FixedMesSettingsService(MesRuntimeSettings settings) : IMesSettingsService
    {
        public MesRuntimeSettings GetSnapshot() => settings.Clone();

        public Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(settings.Clone());

        public Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void ApplySnapshot(MesRuntimeSettings settings)
        {
        }
    }

    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CreateDbContext());
    }
}
