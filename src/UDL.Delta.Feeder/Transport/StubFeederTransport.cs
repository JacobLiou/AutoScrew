using System.Diagnostics;
using UDL.Delta.Feeder.Exceptions;
using UDL.Delta.Feeder.Protocol;

namespace UDL.Delta.Feeder.Transport;

internal sealed class StubFeederTransport : IFeederTransport
{
    private readonly FeederClientOptions _options;
    private FeederDeviceStatus _status = FeederDeviceStatus.Disconnected;

    public StubFeederTransport(FeederClientOptions options) => _options = options;

    public bool IsConnected => _status != FeederDeviceStatus.Disconnected;

    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _status = FeederDeviceStatus.Idle;
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _status = FeederDeviceStatus.Disconnected;
        return Task.CompletedTask;
    }

    public async Task<FeedResult> ExecuteFeedAsync(FeedRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsConnected)
            throw new FeederCommunicationException("Feeder is not connected.");

        _status = FeederDeviceStatus.Feeding;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return _options.SimulatedFailureMode switch
            {
                FeederSimulatedFailureMode.Timeout => await SimulateTimeoutAsync(stopwatch, cancellationToken),
                FeederSimulatedFailureMode.Empty => await SimulateFailureAsync(
                    stopwatch, "FEED_EMPTY", "Simulated feeder empty.", cancellationToken),
                FeederSimulatedFailureMode.Jam => await SimulateFailureAsync(
                    stopwatch, "FEED_JAM", "Simulated feeder jam.", cancellationToken),
                _ => await SimulateSuccessAsync(stopwatch, cancellationToken),
            };
        }
        finally
        {
            _status = FeederDeviceStatus.Idle;
        }
    }

    public Task<FeederDeviceStatus> ReadStatusAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_status);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task<FeedResult> SimulateSuccessAsync(Stopwatch stopwatch, CancellationToken cancellationToken)
    {
        var delay = Math.Max(0, _options.SimulatedFeedDelayMs);
        if (delay > 0)
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

        stopwatch.Stop();
        return new FeedResult
        {
            Success = true,
            DurationMs = (int)stopwatch.ElapsedMilliseconds,
        };
    }

    private async Task<FeedResult> SimulateFailureAsync(
        Stopwatch stopwatch,
        string errorCode,
        string message,
        CancellationToken cancellationToken)
    {
        var delay = Math.Max(0, _options.SimulatedFeedDelayMs);
        if (delay > 0)
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

        stopwatch.Stop();
        _status = FeederDeviceStatus.Fault;
        return new FeedResult
        {
            Success = false,
            DurationMs = (int)stopwatch.ElapsedMilliseconds,
            ErrorCode = errorCode,
            Message = message,
        };
    }

    private async Task<FeedResult> SimulateTimeoutAsync(Stopwatch stopwatch, CancellationToken cancellationToken)
    {
        var timeout = Math.Max(1, _options.FeedTimeoutMs);
        await Task.Delay(timeout + 50, cancellationToken).ConfigureAwait(false);
        stopwatch.Stop();
        _status = FeederDeviceStatus.Fault;
        return new FeedResult
        {
            Success = false,
            DurationMs = (int)stopwatch.ElapsedMilliseconds,
            ErrorCode = "FEED_TIMEOUT",
            Message = "Simulated feeder timeout.",
        };
    }
}
