using UDL.Delta.ToolDock.Protocol;

namespace UDL.Delta.ToolDock.Transport;

internal interface IToolDockTransport : IAsyncDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task DisconnectAsync(CancellationToken cancellationToken);

    Task<ToolDockState> ReadStateAsync(CancellationToken cancellationToken);
}
