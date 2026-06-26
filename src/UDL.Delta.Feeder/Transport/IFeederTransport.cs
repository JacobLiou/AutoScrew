using UDL.Delta.Feeder.Protocol;

namespace UDL.Delta.Feeder.Transport;

internal interface IFeederTransport : IAsyncDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task DisconnectAsync(CancellationToken cancellationToken);

    Task<FeedResult> ExecuteFeedAsync(FeedRequest request, CancellationToken cancellationToken);

    Task<FeederDeviceStatus> ReadStatusAsync(CancellationToken cancellationToken);
}
