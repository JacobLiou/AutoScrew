using UDL.Delta.ToolDock.Protocol;

namespace UDL.Delta.ToolDock;

public interface IToolDockClient : IAsyncDisposable
{
    ToolDockClientOptions Options { get; }

    bool IsConnected { get; }

    Task ConnectAsync(CancellationToken cancellationToken = default);

    Task<ToolDockState> GetStateAsync(CancellationToken cancellationToken = default);

    IAsyncEnumerable<ToolDockStateChange> WatchStateChangesAsync(CancellationToken cancellationToken = default);
}
