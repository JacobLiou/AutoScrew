using AutoScrew.Hmi.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using UDL.Delta.IemdSd;
using Xunit;

namespace AutoScrew.Tests;

public sealed class CommunicationLoggingTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "AutoScrewCommTests", Guid.NewGuid().ToString("N"));

    [Theory]
    [InlineData("Information")]
    [InlineData("Debug")]
    public void Configure_RoutesCommunicationSeparatelyAndUsesDailyNames(string minimumLevel)
    {
        using (var logger = CreateLogger(minimumLevel))
        {
            logger.ForContext("SourceContext", "Business").Information("business-only");
            logger.ForContext("SourceContext", "Business").Debug("business-debug");
            logger.ForContext("SourceContext", IemdSdClient.CommunicationLogCategory).Information("comm-only");
            logger.ForContext("SourceContext", IemdSdClient.CommunicationLogCategory).Debug("comm-debug-hidden");
        }

        var main = File.ReadAllText(Assert.Single(Directory.GetFiles(_directory, "autoscrew-*.log")));
        var commPath = Assert.Single(Directory.GetFiles(Path.Combine(_directory, "Logs", "Comm"), "comm-*.log"));
        var comm = File.ReadAllText(commPath);
        Assert.Equal($"comm-{DateTime.Now:yyyyMMdd}.log", Path.GetFileName(commPath));
        Assert.Contains("business-only", main);
        Assert.Equal(minimumLevel == "Debug", main.Contains("business-debug"));
        Assert.DoesNotContain("comm-only", main);
        Assert.Contains("comm-only", comm);
        Assert.DoesNotContain("business-only", comm);
        Assert.DoesNotContain("comm-debug-hidden", comm);
    }

    [Fact]
    public void Configure_RemovesOlderDailyFilesKeepingFourteen()
    {
        var commDirectory = Path.Combine(_directory, "Logs", "Comm");
        Directory.CreateDirectory(commDirectory);
        for (var daysAgo = 1; daysAgo <= 18; daysAgo++)
            File.WriteAllText(Path.Combine(commDirectory, $"comm-{DateTime.Now.AddDays(-daysAgo):yyyyMMdd}.log"), "old");

        using (var logger = CreateLogger("Information"))
            logger.ForContext("SourceContext", IemdSdClient.CommunicationLogCategory).Information("today");

        var files = Directory.GetFiles(commDirectory, "comm-*.log");
        Assert.Equal(14, files.Length);
        Assert.True(File.Exists(Path.Combine(commDirectory, $"comm-{DateTime.Now:yyyyMMdd}.log")));
        Assert.False(File.Exists(Path.Combine(commDirectory, $"comm-{DateTime.Now.AddDays(-14):yyyyMMdd}.log")));
    }

    private Serilog.Core.Logger CreateLogger(string minimumLevel)
    {
        Directory.CreateDirectory(_directory);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Serilog:Using:0"] = "Serilog.Sinks.File",
            ["Serilog:MinimumLevel:Default"] = minimumLevel,
            ["Serilog:WriteTo:0:Name"] = "File",
            ["Serilog:WriteTo:0:Args:path"] = Path.Combine(_directory, "autoscrew-.log"),
            ["Serilog:WriteTo:0:Args:rollingInterval"] = "Day",
            ["Serilog:CommunicationFile:Path"] = "Logs/Comm/comm-.log",
            ["Serilog:CommunicationFile:RetainedFileCountLimit"] = "14"
        }).Build();
        var settings = new LoggerConfiguration();
        CommunicationLogging.Configure(settings, configuration, _directory);
        return settings.CreateLogger();
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }
}