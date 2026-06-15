using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Mes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class MesSettingsStoreTests
{
    [Fact]
    public async Task SaveAndLoadAsync_RoundTripsSettings()
    {
        var root = CreateTempRoot();
        try
        {
            var store = CreateStore(root);
            var original = new MesRuntimeSettings
            {
                UseMockMes = false,
                BaseUrl = "http://localhost:5080/",
                ApiKey = "test-key",
                TimeoutSeconds = 30,
            };

            await store.SaveAsync(original, CancellationToken.None);
            var loaded = await store.LoadAsync(CancellationToken.None);

            Assert.False(loaded.UseMockMes);
            Assert.Equal("http://localhost:5080/", loaded.BaseUrl);
            Assert.Equal("test-key", loaded.ApiKey);
            Assert.Equal(30, loaded.TimeoutSeconds);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task LoadAsync_MissingFile_SeedsFromAppSettings()
    {
        var root = CreateTempRoot();
        try
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AutoScrew:UseMockMes"] = "false",
                    ["AutoScrew:MesBaseUrl"] = "http://seed.example/",
                })
                .Build();
            var store = CreateStore(root, configuration);
            var loaded = await store.LoadAsync(CancellationToken.None);

            Assert.False(loaded.UseMockMes);
            Assert.Equal("http://seed.example/", loaded.BaseUrl);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static LocalJsonMesSettingsStore CreateStore(string dataDirectory, IConfiguration? configuration = null)
    {
        var options = Options.Create(new AutoScrewAppOptions { DataDirectory = dataDirectory });
        configuration ??= new ConfigurationBuilder().Build();
        return new LocalJsonMesSettingsStore(options, configuration);
    }

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "autoscrew-mes-settings-test", Guid.NewGuid().ToString("N"));
}
