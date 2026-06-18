using System.Collections.ObjectModel;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Activity;

public sealed class OperationActivityLogService : IOperationActivityLogService
{
    private readonly ObservableCollection<OperationActivityLogEntry> _entries = new();
    private readonly JsonlOperationActivityStore _store;
    private readonly IOptions<AutoScrewAppOptions> _options;

    public OperationActivityLogService(
        IOptions<AutoScrewAppOptions> options,
        ILogger<OperationActivityLogService> logger)
    {
        _store = new JsonlOperationActivityStore(logger);
        _options = options;
        Entries = new ReadOnlyObservableCollection<OperationActivityLogEntry>(_entries);
    }

    public ReadOnlyObservableCollection<OperationActivityLogEntry> Entries { get; }

    public void Append(string message, string? serialNumber = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        var entry = new OperationActivityLogEntry(DateTimeOffset.Now, message.Trim(), serialNumber);
        _entries.Insert(0, entry);

        var max = Math.Clamp(_options.Value.OperationActivityLogMaxInMemory, 1, 10_000);
        while (_entries.Count > max)
            _entries.RemoveAt(_entries.Count - 1);

        var opts = _options.Value;
        var line = new OperationActivityLogLine(
            entry.Timestamp,
            opts.StationId,
            entry.SerialNumber,
            entry.Message);

        var directory = ResolveOperationActivityDirectory(opts);
        _ = Task.Run(() => _store.TryAppend(directory, line));
    }

    public void ClearRecent() => _entries.Clear();

    internal static string ResolveOperationActivityDirectory(AutoScrewAppOptions opts)
    {
        if (!string.IsNullOrWhiteSpace(opts.OperationActivityDirectory))
            return opts.OperationActivityDirectory;

        var dataRoot = string.IsNullOrWhiteSpace(opts.DataDirectory)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
            : opts.DataDirectory;

        return Path.Combine(dataRoot, "activity");
    }
}
