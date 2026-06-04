using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Tests;

public class CommandMailboxTests
{
    [Fact]
    public void CreateRequest_SetsCommandInFirstWord()
    {
        var req = CommandMailbox.CreateRequest(750, word2: 1, word3: 2);
        Assert.Equal(750, req[0]);
        Assert.Equal(1, req[2]);
        Assert.Equal(2, req[3]);
        Assert.Equal(0, req[6]);
    }

    [Fact]
    public void SetReportId_SplitsLowHighWords()
    {
        var req = CommandMailbox.CreateRequest(751);
        CommandMailbox.SetReportId(req, 70000);
        Assert.Equal(70000 % 65536, req[2]);
        Assert.Equal(70000 / 65536, req[3]);
        Assert.Equal(0, req[6]);
    }

    [Fact]
    public void CreateRequest_ReadParameter_Layout()
    {
        var req = CommandMailbox.CreateRequest(150, word2: 1, word3: 42);
        Assert.Equal(150, req[0]);
        Assert.Equal(0, req[1]);
        Assert.Equal(1, req[2]);
        Assert.Equal(42, req[3]);
    }

    [Fact]
    public void CreateRequest_WriteParameter_Layout()
    {
        var req = CommandMailbox.CreateRequest(100, word2: 1, word3: 99);
        Assert.Equal(100, req[0]);
        Assert.Equal(0, req[1]);
        Assert.Equal(1, req[2]);
        Assert.Equal(99, req[3]);
    }
}
