using UDL.Delta.ToolDock.Protocol;

namespace UDL.Delta.ToolDock.Transport;

internal sealed class StubToolDockTransport : IToolDockTransport
{
    private readonly ToolDockClientOptions _options;
    private ToolDockState _state;

    public StubToolDockTransport(ToolDockClientOptions options)
    {
        _options = options;
        _state = options.InitialState;
        IsConnected = false;
    }

    public bool IsConnected { get; private set; }

    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IsConnected = true;
        return Task.CompletedTask;
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IsConnected = false;
        return Task.CompletedTask;
    }

    public Task<ToolDockState> ReadStateAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!IsConnected)
            return Task.FromResult(ToolDockState.Unknown);

        return Task.FromResult(_state);
    }

    internal void SetState(ToolDockState state) => _state = state;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
