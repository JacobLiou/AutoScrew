using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UDL.Delta.ToolDock.Exceptions;
using UDL.Delta.ToolDock.Protocol;
using UDL.Delta.ToolDock.Transport;

namespace UDL.Delta.ToolDock;

public sealed class ToolDockClient : IToolDockClient
{
    private readonly ILogger _logger;
    private readonly IToolDockTransport _transport;

    public ToolDockClient(ToolDockClientOptions options, ILogger<ToolDockClient>? logger = null)
    {
        Options = options;
        _logger = logger ?? NullLogger<ToolDockClient>.Instance;
        _transport = CreateTransport(options);
    }

    public ToolDockClientOptions Options { get; }

    public bool IsConnected => _transport.IsConnected;

    public Task ConnectAsync(CancellationToken cancellationToken = default) =>
        _transport.ConnectAsync(cancellationToken);

    public Task<ToolDockState> GetStateAsync(CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        return _transport.ReadStateAsync(cancellationToken);
    }

    public async IAsyncEnumerable<ToolDockStateChange> WatchStateChangesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        EnsureConnected();

        var pollInterval = Math.Max(10, Options.PollIntervalMs);
        var debounceMs = Math.Max(0, Options.DebounceMs);
        var stableState = await _transport.ReadStateAsync(cancellationToken).ConfigureAwait(false);
        var candidateState = stableState;
        DateTimeOffset? candidateSince = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(pollInterval, cancellationToken).ConfigureAwait(false);
            var current = await _transport.ReadStateAsync(cancellationToken).ConfigureAwait(false);

            if (current == stableState)
            {
                candidateState = stableState;
                candidateSince = null;
                continue;
            }

            if (current != candidateState)
            {
                candidateState = current;
                candidateSince = DateTimeOffset.UtcNow;
                continue;
            }

            if (candidateSince is null)
                continue;

            var elapsed = DateTimeOffset.UtcNow - candidateSince.Value;
            if (elapsed.TotalMilliseconds < debounceMs)
                continue;

            var previous = stableState;
            stableState = candidateState;
            candidateSince = null;

            _logger.LogDebug("Tool dock state changed: {Previous} -> {Current}", previous, stableState);
            yield return new ToolDockStateChange
            {
                Previous = previous,
                Current = stableState,
                Timestamp = DateTimeOffset.UtcNow,
            };
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transport.IsConnected)
            await _transport.DisconnectAsync(CancellationToken.None).ConfigureAwait(false);
        await _transport.DisposeAsync().ConfigureAwait(false);
    }

    internal IToolDockTransport Transport => _transport;

    private void EnsureConnected()
    {
        if (!IsConnected)
            throw new ToolDockCommunicationException("Tool dock is not connected.");
    }

    private static IToolDockTransport CreateTransport(ToolDockClientOptions options) =>
        options.Transport switch
        {
            ToolDockTransportType.Stub => new StubToolDockTransport(options),
            _ => throw new ToolDockCommunicationException($"Unsupported tool dock transport: {options.Transport}."),
        };
}
