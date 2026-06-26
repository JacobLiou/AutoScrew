using UDL.Delta.Feeder.Protocol;

namespace UDL.Delta.Feeder;

public interface IFeederClient : IAsyncDisposable
{
    FeederClientOptions Options { get; }

    bool IsConnected { get; }

    Task ConnectAsync(CancellationToken cancellationToken = default);

    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<FeedResult> FeedAsync(FeedRequest request, CancellationToken cancellationToken = default);

    Task<FeederDeviceStatus> GetStatusAsync(CancellationToken cancellationToken = default);
}
