using System.Net;
using System.Text;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Services;
using AutoScrew.Infrastructure.Mes;
using AutoScrew.Infrastructure.Persistence;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProductTemplateMesSyncServiceTests
{
    [Fact]
    public async Task SyncWithMesAsync_UploadsPending_AndDownloadsRemoteUpdate()
    {
        var root = CreateTempRoot();
        var handler = new RoutingHttpHandler();
        var services = BuildServices(root, handler);
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

        SeedRemoteTemplate(root, "PN-2", "remote-v1");
        var remoteHash = ProductTemplateSyncOperations.ComputePackageHash(Path.Combine(root, "remote", "PN-2"));
        handler.SetCatalog(
        [
            new CatalogEntry("PN-2", remoteHash, DateTimeOffset.UtcNow, "api/templates/PN-2/package"),
        ]);
        handler.SetPackageProvider("PN-2", () => CreateZipFromFolder(Path.Combine(root, "remote", "PN-2")));

        var sync = provider.GetRequiredService<IProductTemplateMesSyncService>();
        var result = await sync.SyncWithMesAsync();

        Assert.Equal(1, result.UploadedCount);
        Assert.Equal(1, result.DownloadedCount);
        Assert.Contains("PN-2", result.DownloadedPartNumbers);

        var pn1 = await syncRepo.GetAsync("PN-1");
        Assert.Equal(ProductTemplateSyncState.Synced, pn1!.SyncState);

        var pn2Path = localStore.TryResolveLocalTemplate("PN-2");
        Assert.NotNull(pn2Path);
        Assert.Contains("remote-v1", await File.ReadAllTextAsync(pn2Path!));

        var pn2 = await syncRepo.GetAsync("PN-2");
        Assert.Equal(ProductTemplateSyncState.DownloadedFromMes, pn2!.SyncState);

        TryDeleteDirectory(root);
    }

    [Fact]
    public async Task SyncWithMesAsync_SkipsDownload_WhenRemoteHashMatchesLocal()
    {
        var root = CreateTempRoot();
        var handler = new RoutingHttpHandler();
        var services = BuildServices(root, handler);
        await using var provider = services.BuildServiceProvider();

        SeedLocalTemplate(root, "PN-1", "same-content");
        var localStore = provider.GetRequiredService<IProductTemplateLocalStore>();
        var syncRepo = provider.GetRequiredService<IProductTemplateSyncRepository>();
        await ProductTemplateSyncOperations.UpsertFromFileAsync(
            syncRepo,
            localStore,
            "PN-1",
            ProductTemplateSyncState.Synced,
            null,
            null,
            DateTimeOffset.UtcNow,
            "rev-1",
            CancellationToken.None);

        var localHash = ProductTemplateSyncOperations.ComputePackageHash(localStore.GetProductFolder("PN-1"));
        handler.SetCatalog(
        [
            new CatalogEntry("PN-1", localHash, DateTimeOffset.UtcNow.AddHours(1), "api/templates/PN-1/package"),
        ]);
        handler.SetPackageProvider("PN-1", () => CreateZipFromFolder(localStore.GetProductFolder("PN-1")));

        var sync = provider.GetRequiredService<IProductTemplateMesSyncService>();
        var result = await sync.SyncWithMesAsync();

        Assert.Equal(0, result.UploadedCount);
        Assert.Equal(0, result.DownloadedCount);
        Assert.True(result.SkippedCount >= 1);

        TryDeleteDirectory(root);
    }

    [Fact]
    public async Task UpsertFromFileAsync_MarksPendingUpload_WhenOnlyImageChanges()
    {
        var root = CreateTempRoot();
        var handler = new RoutingHttpHandler();
        var services = BuildServices(root, handler);
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

    private static ServiceCollection BuildServices(string templateRoot, RoutingHttpHandler handler)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpClientFactory>(new StubHttpClientFactory(handler));
        services.AddSingleton(new MesRuntimeSettings
        {
            UseMockMes = true,
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
        services.Configure<AutoScrew.Application.Configuration.AutoScrewAppOptions>(o =>
        {
            o.TemplateDirectory = templateRoot;
        });

        services.AddSingleton<ProductTemplateLocalStore>();
        services.AddSingleton<IProductTemplateLocalStore>(sp => sp.GetRequiredService<ProductTemplateLocalStore>());
        services.AddSingleton<IProductTemplateSyncRepository, EfProductTemplateSyncRepository>();
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

    private static void SeedRemoteTemplate(string root, string pn, string marker)
    {
        var folder = Path.Combine(root, "remote", pn);
        Directory.CreateDirectory(Path.Combine(folder, "images"));
        File.WriteAllText(
            Path.Combine(folder, $"{pn}.product-template.json"),
            $$"""{"schemaVersion":2,"productId":"{{pn}}","marker":"{{marker}}"}""");
        File.WriteAllBytes(Path.Combine(folder, "images", "top.png"), [4, 5, 6]);
    }

    private static byte[] CreateZipFromFolder(string folder)
    {
        using var ms = new MemoryStream();
        using (var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(folder, file).Replace('\\', '/');
                var entry = archive.CreateEntry(relative, System.IO.Compression.CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(file);
                fileStream.CopyTo(entryStream);
            }
        }

        return ms.ToArray();
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

    private sealed record CatalogEntry(string PartNumber, string ContentHash, DateTimeOffset ModifiedUtc, string PackageUrl);

    private sealed class RoutingHttpHandler : HttpMessageHandler
    {
        private IReadOnlyList<CatalogEntry> _catalog = Array.Empty<CatalogEntry>();
        private readonly Dictionary<string, Func<byte[]>> _packages = new(StringComparer.OrdinalIgnoreCase);

        public void SetCatalog(IReadOnlyList<CatalogEntry> catalog) => _catalog = catalog;

        public void SetPackageProvider(string pn, Func<byte[]> provider) => _packages[pn] = provider;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.AbsolutePath;

            if (request.Method == HttpMethod.Get && path.Equals("/api/templates", StringComparison.OrdinalIgnoreCase))
            {
                var json = "[" + string.Join(",", _catalog.Select(x =>
                    $$"""{"partNumber":"{{x.PartNumber}}","contentHash":"{{x.ContentHash}}","modifiedUtc":"{{x.ModifiedUtc:O}}","packageUrl":"{{x.PackageUrl}}"}""")) + "]";
                return Task.FromResult(Json(HttpStatusCode.OK, json));
            }

            if (path.StartsWith("/api/templates/", StringComparison.OrdinalIgnoreCase)
                && path.EndsWith("/package", StringComparison.OrdinalIgnoreCase))
            {
                var pn = path["/api/templates/".Length..^"/package".Length];
                if (request.Method == HttpMethod.Get)
                {
                    if (!_packages.TryGetValue(pn, out var provider))
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));

                    var bytes = provider();
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(bytes),
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
                    return Task.FromResult(response);
                }

                if (request.Method == HttpMethod.Post)
                {
                    return request.Content!.ReadAsByteArrayAsync(cancellationToken).ContinueWith(t =>
                    {
                        var body = t.Result;
                        var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(body));
                        var json = $$"""{"accepted":true,"revision":"test-rev","contentHash":"{{hash}}"}""";
                        return Json(HttpStatusCode.OK, json);
                    }, cancellationToken);
                }
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
