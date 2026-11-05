using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using AutoScrew.Infrastructure.Templates;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class RecipeProvisioningServiceTests
{
    [Fact]
    public async Task GetProvisionedRecipeAsync_WhenMesDownloadSucceeds_UsesMesSource()
    {
        var root = CreateTempRoot();
        try
        {
            var store = new ProductTemplateLocalStore(Options.Create(new AutoScrewAppOptions { TemplateDirectory = root }));
            var templatePath = store.GetDefaultTemplatePath("PN-MES");
            store.EnsureProductFolder("PN-MES");
            File.WriteAllText(templatePath, "{}");

            var service = CreateService(
                root,
                new StubMesClient(new RecipeBundle("PN-MES", "PN-MES/PN-MES.product-template.json", null, [], "http://mes/pkg")),
                useMockMes: false,
                downloadPath: templatePath);

            var result = await service.GetProvisionedRecipeAsync("SN1", "PN-MES");

            Assert.Equal(RecipeTemplateSource.Mes, result.TemplateSource);
            Assert.True(File.Exists(result.ResolvedTemplatePath));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task GetProvisionedRecipeAsync_WhenMesDownloadFails_FallsBackToLocal()
    {
        var root = CreateTempRoot();
        try
        {
            var store = new ProductTemplateLocalStore(Options.Create(new AutoScrewAppOptions { TemplateDirectory = root }));
            store.EnsureProductFolder("PN-LOC");
            var templatePath = store.GetDefaultTemplatePath("PN-LOC");
            File.WriteAllText(templatePath, "{}");

            var service = CreateService(
                root,
                new StubMesClient(new RecipeBundle("PN-LOC", "missing.json", null, [], "http://mes/fail")),
                useMockMes: false,
                downloadPath: null);

            var result = await service.GetProvisionedRecipeAsync("SN1", "PN-LOC");

            Assert.Equal(RecipeTemplateSource.Local, result.TemplateSource);
            Assert.Equal(templatePath, result.ResolvedTemplatePath);
            Assert.NotNull(result.InfoMessage);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task GetProvisionedRecipeAsync_WhenMesAndLocalMissing_Throws()
    {
        var root = CreateTempRoot();
        try
        {
            var service = CreateService(
                root,
                new ThrowingMesClient(),
                useMockMes: false,
                downloadPath: null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetProvisionedRecipeAsync("SN1", "PN-MISSING"));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task GetProvisionedRecipeAsync_WhenOnlyLanTemplate_MaterializesLocal()
    {
        var root = CreateTempRoot();
        try
        {
            var dataDir = Path.Combine(root, "data");
            var templateDir = Path.Combine(root, "templates");
            Directory.CreateDirectory(templateDir);
            var lanFolder = Path.Combine(dataDir, "process", "PN-LAN", "template");
            Directory.CreateDirectory(lanFolder);
            File.WriteAllText(
                Path.Combine(lanFolder, "PN-LAN.product-template.json"),
                """{"schemaVersion":2,"productId":"PN-LAN"}""");

            var app = Options.Create(new AutoScrewAppOptions
            {
                TemplateDirectory = templateDir,
                DataDirectory = dataDir,
            });
            var localStore = new ProductTemplateLocalStore(app);
            var mes = new StubMesSettings(useMockMes: true);
            var lan = new AutoScrew.Infrastructure.Lan.LanShareAccess(
                mes, app, NullLogger<AutoScrew.Infrastructure.Lan.LanShareAccess>.Instance);
            var processStore = new AutoScrew.Infrastructure.ProcessLibrary.ProcessLibraryStore(
                lan, app, NullLogger<AutoScrew.Infrastructure.ProcessLibrary.ProcessLibraryStore>.Instance);
            var lanShare = new ProductTemplateLanShare(
                processStore, localStore, NullLogger<ProductTemplateLanShare>.Instance);

            var service = new RecipeProvisioningService(
                new StubMesClient(new RecipeBundle("PN-LAN", null, null, [])),
                mes,
                localStore,
                lanShare,
                new StubPackageClient(null),
                new InMemorySyncRepository(),
                app,
                NullLogger<RecipeProvisioningService>.Instance);

            var result = await service.GetProvisionedRecipeAsync("SN1", "PN-LAN");

            Assert.Equal(RecipeTemplateSource.Local, result.TemplateSource);
            Assert.True(File.Exists(result.ResolvedTemplatePath));
            Assert.Contains("PN-LAN", result.ResolvedTemplatePath, StringComparison.OrdinalIgnoreCase);
            Assert.NotNull(localStore.TryResolveLocalTemplate("PN-LAN"));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static RecipeProvisioningService CreateService(
        string templateDir,
        IMesClient mesClient,
        bool useMockMes,
        string? downloadPath)
    {
        var store = new ProductTemplateLocalStore(Options.Create(new AutoScrewAppOptions { TemplateDirectory = templateDir }));
        var syncRepo = new InMemorySyncRepository();
        var packageClient = new StubPackageClient(downloadPath);
        var settings = new StubMesSettings(useMockMes);
        var lanShare = new StubLanShare();
        return new RecipeProvisioningService(
            mesClient,
            settings,
            store,
            lanShare,
            packageClient,
            syncRepo,
            Options.Create(new AutoScrewAppOptions { TemplateDirectory = templateDir }),
            NullLogger<RecipeProvisioningService>.Instance);
    }

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "autoscrew-recipe-prov", Guid.NewGuid().ToString("N"));

    private sealed class StubMesClient(RecipeBundle recipe) : IMesClient
    {
        public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SnValidationResult(true, recipe.PartNumber, null));

        public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(recipe with { PartNumber = partNumber });

        public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
            Task.FromResult(new MesUploadResult(true, null, payload.SerialNumber));
    }

    private sealed class ThrowingMesClient : IMesClient
    {
        public Task<SnValidationResult> ValidateSnAsync(string serialNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(new SnValidationResult(true, "PN-X", null));

        public Task<RecipeBundle> GetRecipeAsync(string serialNumber, string partNumber, CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("MES down");

        public Task<MesUploadResult> UploadResultAsync(LockJobResultPayload payload, CancellationToken cancellationToken = default) =>
            Task.FromResult(new MesUploadResult(true, null, payload.SerialNumber));
    }

    private sealed class StubPackageClient(string? pathToReturn) : IMesTemplatePackageClient
    {
        public Task<string> DownloadTemplatePackageAsync(string partNumber, string packageUrl, CancellationToken cancellationToken = default)
        {
            if (pathToReturn is null)
                throw new HttpRequestException("download failed");
            return Task.FromResult(pathToReturn);
        }
    }

    private sealed class StubMesSettings(bool useMockMes) : IMesSettingsService
    {
        public MesRuntimeSettings GetSnapshot() => new()
        {
            UseMockMes = useMockMes,
            MesMode = useMockMes ? MesProviderMode.Mock : MesProviderMode.LegacyHttp,
            BaseUrl = "http://mes.test/",
        };

        public Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(GetSnapshot());

        public Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void ApplySnapshot(MesRuntimeSettings settings)
        {
        }
    }

    private sealed class StubLanShare : IProductTemplateLanShare
    {
        public Task<ProductTemplateLanPublishResult> PublishAsync(
            string partNumber,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public IReadOnlyList<ProductTemplateLanEntry> ListLanTemplates() =>
            Array.Empty<ProductTemplateLanEntry>();

        public Task<string> PullToLocalAsync(string partNumber, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public string? TryResolveLanTemplate(string partNumber) => null;

        public string? TryMaterializeLanTemplateToLocal(string partNumber) => null;
    }

    private sealed class InMemorySyncRepository : IProductTemplateSyncRepository
    {
        private readonly Dictionary<string, ProductTemplateSyncRecord> _records = new(StringComparer.OrdinalIgnoreCase);

        public Task<ProductTemplateSyncRecord?> GetAsync(string partNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(_records.TryGetValue(partNumber, out var r) ? r : null);

        public Task<IReadOnlyList<ProductTemplateSyncRecord>> ListAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ProductTemplateSyncRecord>>(_records.Values.ToList());

        public Task UpsertAsync(ProductTemplateSyncRecord record, CancellationToken cancellationToken = default)
        {
            _records[record.PartNumber] = record;
            return Task.CompletedTask;
        }
    }
}
