using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Activity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoScrew.Tests;

public sealed class OperationActivityLogServiceTests
{
    [Fact]
    public void ResolveOperationActivityDirectory_UsesDataDirectoryWhenUnset()
    {
        var opts = new AutoScrewAppOptions
        {
            DataDirectory = @"C:\data\autoscrew",
            OperationActivityDirectory = "",
        };

        var path = OperationActivityLogService.ResolveOperationActivityDirectory(opts);
        Assert.Equal(Path.Combine(@"C:\data\autoscrew", "activity"), path);
    }

    [Fact]
    public void Append_keeps_at_most_max_in_memory_newest_first()
    {
        using var fixture = new ActivityLogFixture(maxInMemory: 200);
        for (var i = 0; i < 250; i++)
            fixture.Service.Append($"log-{i}", "SN001");

        Assert.Equal(200, fixture.Service.Entries.Count);
        Assert.Equal("log-249", fixture.Service.Entries[0].Message);
        Assert.Equal("log-50", fixture.Service.Entries[^1].Message);
    }

    [Fact]
    public async Task Append_persists_all_lines_to_jsonl()
    {
        using var fixture = new ActivityLogFixture(maxInMemory: 200);
        for (var i = 0; i < 250; i++)
            fixture.Service.Append($"log-{i}", "SN001");

        await WaitForPersistedLinesAsync(fixture, 250).ConfigureAwait(false);

        var lines = ReadAllLinesShared(fixture.JsonlPath);
        Assert.Equal(250, lines.Length);
        Assert.Contains(lines, line => line.Contains("\"Message\":\"log-0\"", StringComparison.Ordinal));
        Assert.Contains(lines, line => line.Contains("\"SerialNumber\":\"SN001\"", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ClearRecent_clears_memory_but_not_disk()
    {
        using var fixture = new ActivityLogFixture(maxInMemory: 200);
        fixture.Service.Append("line-1", "SN002");
        fixture.Service.Append("line-2", "SN002");

        await WaitForPersistedLinesAsync(fixture, 2).ConfigureAwait(false);

        fixture.Service.ClearRecent();

        Assert.Empty(fixture.Service.Entries);
        Assert.Equal(2, ReadAllLinesShared(fixture.JsonlPath).Length);
    }

    private static async Task WaitForPersistedLinesAsync(ActivityLogFixture fixture, int expectedLines)
    {
        for (var i = 0; i < 50; i++)
        {
            if (File.Exists(fixture.JsonlPath) && ReadAllLinesShared(fixture.JsonlPath).Length >= expectedLines)
                return;

            await Task.Delay(50).ConfigureAwait(false);
        }

        var actual = File.Exists(fixture.JsonlPath) ? ReadAllLinesShared(fixture.JsonlPath).Length : 0;
        Assert.Equal(expectedLines, actual);
    }

    private static string[] ReadAllLinesShared(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs);
        var text = sr.ReadToEnd();
        return text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    private sealed class ActivityLogFixture : IDisposable
    {
        public ActivityLogFixture(int maxInMemory)
        {
            DataDirectory = Path.Combine(Path.GetTempPath(), "autoscrew-activity-test", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(DataDirectory);

            Service = new OperationActivityLogService(
                Options.Create(new AutoScrewAppOptions
                {
                    DataDirectory = DataDirectory,
                    OperationActivityLogMaxInMemory = maxInMemory,
                    StationId = "T-TEST",
                }),
                NullLogger<OperationActivityLogService>.Instance);

            var activityDir = OperationActivityLogService.ResolveOperationActivityDirectory(
                new AutoScrewAppOptions { DataDirectory = DataDirectory });
            JsonlPath = Path.Combine(activityDir, $"operation-activity-{DateTime.Now:yyyy-MM-dd}.jsonl");
        }

        public string DataDirectory { get; }

        public string JsonlPath { get; }

        public OperationActivityLogService Service { get; }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(DataDirectory))
                    Directory.Delete(DataDirectory, recursive: true);
            }
            catch
            {
                // best effort temp cleanup
            }
        }
    }
}
