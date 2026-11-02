using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using UDL.Delta.IemdSd;

namespace AutoScrew.Hmi.Services;

internal static class CommunicationLogging
{
    public static void Configure(LoggerConfiguration logger, IConfiguration configuration, string baseDirectory)
    {
        var levels = new ConfigurationBuilder().AddInMemoryCollection(configuration.AsEnumerable()
            .Where(entry => entry.Key.StartsWith("Serilog:MinimumLevel", StringComparison.OrdinalIgnoreCase)
                || entry.Key.StartsWith("Serilog:LevelSwitches", StringComparison.OrdinalIgnoreCase)))
            .Build();
        var settings = configuration.GetSection("Serilog:CommunicationFile");
        var path = Path.GetFullPath(settings["Path"] ?? "Logs/Comm/comm-.log", baseDirectory);
        var retainedFiles = settings.GetValue("RetainedFileCountLimit", 14);
        if (retainedFiles < 1)
            throw new InvalidOperationException("Serilog:CommunicationFile:RetainedFileCountLimit must be positive.");

        logger.ReadFrom.Configuration(levels)
            .MinimumLevel.Override(IemdSdClient.CommunicationLogCategory, settings.GetValue("MinimumLevel", LogEventLevel.Information))
            .WriteTo.Logger(main => main
                .Filter.ByExcluding(IsCommunication)
                .ReadFrom.Configuration(configuration))
            .WriteTo.Logger(comm => comm
                .MinimumLevel.Verbose()
                .Filter.ByIncludingOnly(IsCommunication)
                .WriteTo.File(path,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: retainedFiles,
                    fileSizeLimitBytes: null,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));
    }

    private static bool IsCommunication(LogEvent entry) =>
        entry.Properties.TryGetValue("SourceContext", out var source)
        && source is ScalarValue { Value: string category }
        && category == IemdSdClient.CommunicationLogCategory;
}