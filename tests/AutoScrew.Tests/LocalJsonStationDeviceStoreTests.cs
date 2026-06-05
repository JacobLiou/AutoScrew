using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Hardware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public class LocalJsonStationDeviceStoreTests
{
    [Fact]
    public async Task LoadAsync_LegacyThreeSlotJson_MigratesActiveSlotAndRewrites()
    {
        var root = CreateTempRoot();
        try
        {
            var stationId = "STATION-01";
            var path = Path.Combine(root, "stations", stationId, "devices.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path,
                """
                {
                  "StationId": "STATION-01",
                  "ActiveDeviceSlot": 1,
                  "Devices": [
                    { "SlotIndex": 0, "DisplayName": "Device 1", "Enabled": true, "Host": "10.0.0.1", "Port": 502 },
                    { "SlotIndex": 1, "DisplayName": "Device 2", "Enabled": true, "Host": "10.0.0.2", "Port": 503 },
                    { "SlotIndex": 2, "DisplayName": "Device 3", "Enabled": false, "Host": "10.0.0.3", "Port": 504 }
                  ]
                }
                """);

            var store = CreateStore(root);
            var config = await store.LoadAsync(stationId, CancellationToken.None);

            Assert.Equal("Device 2", config.Device.DisplayName);
            Assert.Equal("10.0.0.2", config.Device.Host);
            Assert.Equal(503, config.Device.Port);

            var rewritten = await File.ReadAllTextAsync(path);
            Assert.Contains("\"Device\"", rewritten);
            Assert.DoesNotContain("\"Devices\"", rewritten);
            Assert.DoesNotContain("ActiveDeviceSlot", rewritten);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task SaveAndLoadAsync_NewFormat_RoundTripsSingleDevice()
    {
        var root = CreateTempRoot();
        try
        {
            var store = CreateStore(root);
            var stationId = "STATION-02";
            var original = new AutoScrew.Application.Abstractions.StationDeviceConfiguration
            {
                StationId = stationId,
                Device = new AutoScrew.Application.Abstractions.StationDeviceEndpoint
                {
                    DisplayName = "Line Controller",
                    Enabled = true,
                    Host = "192.168.0.10",
                    Port = 1502,
                },
            };

            await store.SaveAsync(original, CancellationToken.None);
            var loaded = await store.LoadAsync(stationId, CancellationToken.None);

            Assert.Equal("Line Controller", loaded.Device.DisplayName);
            Assert.Equal("192.168.0.10", loaded.Device.Host);
            Assert.Equal(1502, loaded.Device.Port);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task LoadAsync_MissingFile_CreatesSeedWithSingleDevice()
    {
        var root = CreateTempRoot();
        try
        {
            var store = CreateStore(root);
            var config = await store.LoadAsync("STATION-NEW", CancellationToken.None);

            Assert.Equal("STATION-NEW", config.StationId);
            Assert.Equal("IEMD-SD", config.Device.DisplayName);
            Assert.False(config.Device.Enabled);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static LocalJsonStationDeviceStore CreateStore(string dataDirectory)
    {
        var options = Options.Create(new AutoScrewAppOptions { DataDirectory = dataDirectory });
        IConfiguration configuration = new ConfigurationBuilder().Build();
        return new LocalJsonStationDeviceStore(options, configuration);
    }

    private static string CreateTempRoot() =>
        Path.Combine(Path.GetTempPath(), "autoscrew-device-store-test", Guid.NewGuid().ToString("N"));
}
