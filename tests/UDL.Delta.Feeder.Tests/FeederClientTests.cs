using UDL.Delta.Feeder;
using UDL.Delta.Feeder.Exceptions;
using UDL.Delta.Feeder.Protocol;

namespace UDL.Delta.Feeder.Tests;

public sealed class FeederClientTests
{
    [Fact]
    public async Task FeedAsync_WhenConnected_ReturnsSuccess()
    {
        var options = new FeederClientOptions
        {
            SimulatedFeedDelayMs = 10,
        };
        await using var client = new FeederClient(options);
        await client.ConnectAsync();

        var result = await client.FeedAsync(new FeedRequest { PartNo = "PN-001", Channel = 1 });

        Assert.True(result.Success);
        Assert.Null(result.ErrorCode);
        Assert.True(result.DurationMs >= 10);
    }

    [Fact]
    public async Task FeedAsync_WhenNotConnected_Throws()
    {
        await using var client = new FeederClient(new FeederClientOptions());

        var ex = await Assert.ThrowsAsync<FeederCommunicationException>(
            () => client.FeedAsync(new FeedRequest()));

        Assert.Contains("not connected", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(FeederSimulatedFailureMode.Empty, "FEED_EMPTY")]
    [InlineData(FeederSimulatedFailureMode.Jam, "FEED_JAM")]
    public async Task FeedAsync_WhenSimulatedFailure_ReturnsErrorCode(
        FeederSimulatedFailureMode mode,
        string expectedCode)
    {
        var options = new FeederClientOptions
        {
            SimulatedFeedDelayMs = 5,
            SimulatedFailureMode = mode,
        };
        await using var client = new FeederClient(options);
        await client.ConnectAsync();

        var result = await client.FeedAsync(new FeedRequest { Channel = 2 });

        Assert.False(result.Success);
        Assert.Equal(expectedCode, result.ErrorCode);
    }

    [Fact]
    public async Task FeedAsync_WhenSimulatedTimeout_ThrowsFeedTimeout()
    {
        var options = new FeederClientOptions
        {
            FeedTimeoutMs = 50,
            SimulatedFailureMode = FeederSimulatedFailureMode.Timeout,
        };
        await using var client = new FeederClient(options);
        await client.ConnectAsync();

        var ex = await Assert.ThrowsAsync<FeederCommunicationException>(
            () => client.FeedAsync(new FeedRequest()));

        Assert.Equal("FEED_TIMEOUT", ex.ErrorCode);
    }

    [Fact]
    public async Task GetStatusAsync_AfterConnect_ReturnsIdle()
    {
        await using var client = new FeederClient(new FeederClientOptions());
        await client.ConnectAsync();

        var status = await client.GetStatusAsync();

        Assert.Equal(FeederDeviceStatus.Idle, status);
    }
}
