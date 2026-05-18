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
    }
}
