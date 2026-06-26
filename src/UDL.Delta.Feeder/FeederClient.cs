using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UDL.Delta.Feeder.Exceptions;
using UDL.Delta.Feeder.Protocol;
using UDL.Delta.Feeder.Transport;

namespace UDL.Delta.Feeder;

public sealed class FeederClient : IFeederClient
{
    private readonly ILogger _logger;
    private readonly IFeederTransport _transport;

    public FeederClient(FeederClientOptions options, ILogger<FeederClient>? logger = null)
    {
        Options = options;
        _logger = logger ?? NullLogger<FeederClient>.Instance;
        _transport = CreateTransport(options);
    }

    public FeederClientOptions Options { get; }

    public bool IsConnected => _transport.IsConnected;

    public Task ConnectAsync(CancellationToken cancellationToken = default) =>
        _transport.ConnectAsync(cancellationToken);

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        _logger.LogDebug("Feeder initialized (transport={Transport}).", Options.Transport);
        return Task.CompletedTask;
    }

    public async Task<FeedResult> FeedAsync(FeedRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureConnected();

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(Options.FeedTimeoutMs);

        try
        {
            var result = await _transport.ExecuteFeedAsync(request, timeoutCts.Token).ConfigureAwait(false);
            if (!result.Success && result.ErrorCode is { } code)
            {
                _logger.LogWarning(
                    "Feed failed: {ErrorCode} partNo={PartNo} channel={Channel}",
                    code,
                    request.PartNo,
                    request.Channel);
            }

            return result;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new FeederCommunicationException("FEED_TIMEOUT", "Feeder operation timed out.");
        }
    }

    public Task<FeederDeviceStatus> GetStatusAsync(CancellationToken cancellationToken = default) =>
        _transport.ReadStatusAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        if (_transport.IsConnected)
            await _transport.DisconnectAsync(CancellationToken.None).ConfigureAwait(false);
        await _transport.DisposeAsync().ConfigureAwait(false);
    }

    private void EnsureConnected()
    {
        if (!IsConnected)
            throw new FeederCommunicationException("Feeder is not connected.");
    }

    private static IFeederTransport CreateTransport(FeederClientOptions options) =>
        options.Transport switch
        {
            FeederTransportType.Stub => new StubFeederTransport(options),
            _ => throw new FeederCommunicationException($"Unsupported feeder transport: {options.Transport}."),
        };
}
