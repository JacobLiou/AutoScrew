using UDL.Delta.ToolDock;
using UDL.Delta.ToolDock.Exceptions;
using UDL.Delta.ToolDock.Protocol;
using UDL.Delta.ToolDock.Transport;

namespace UDL.Delta.ToolDock.Tests;

public sealed class ToolDockClientTests
{
    [Fact]
    public async Task GetStateAsync_AfterConnect_ReturnsInitialState()
    {
        var options = new ToolDockClientOptions { InitialState = ToolDockState.Placed };
        await using var client = new ToolDockClient(options);
        await client.ConnectAsync();

        var state = await client.GetStateAsync();

        Assert.Equal(ToolDockState.Placed, state);
    }

    [Fact]
    public async Task GetStateAsync_WhenNotConnected_Throws()
    {
        await using var client = new ToolDockClient(new ToolDockClientOptions());

        var ex = await Assert.ThrowsAsync<ToolDockCommunicationException>(() => client.GetStateAsync());

        Assert.Contains("not connected", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SetState_UpdatesReadState()
    {
        var client = new ToolDockClient(new ToolDockClientOptions { InitialState = ToolDockState.Placed });
        await client.ConnectAsync();
        var stub = (StubToolDockTransport)((ToolDockClient)client).Transport;

        stub.SetState(ToolDockState.PickedUp);

        var state = await client.GetStateAsync();
        await client.DisposeAsync();

        Assert.Equal(ToolDockState.PickedUp, state);
    }

    [Fact]
    public async Task WatchStateChangesAsync_EmitsDebouncedChange()
    {
        var options = new ToolDockClientOptions
        {
            InitialState = ToolDockState.Placed,
            PollIntervalMs = 20,
            DebounceMs = 30,
        };
        var client = new ToolDockClient(options);
        await client.ConnectAsync();
        var stub = (StubToolDockTransport)((ToolDockClient)client).Transport;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var watchTask = CollectChangesAsync(client, cts.Token);

        await Task.Delay(30);
        stub.SetState(ToolDockState.PickedUp);

        var changes = await watchTask;

        Assert.Contains(changes, c =>
            c.Previous == ToolDockState.Placed && c.Current == ToolDockState.PickedUp);
        await client.DisposeAsync();
    }

    private static async Task<List<ToolDockStateChange>> CollectChangesAsync(
        IToolDockClient client,
        CancellationToken cancellationToken)
    {
        var changes = new List<ToolDockStateChange>();
        try
        {
            await foreach (var change in client.WatchStateChangesAsync(cancellationToken))
                changes.Add(change);
        }
        catch (OperationCanceledException)
        {
        }

        return changes;
    }
}
