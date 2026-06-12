using UDL.Delta.IemdSd.Modbus;

namespace UDL.Delta.IemdSd.Tests;

public class ModbusCommandLayoutTests
{
    [Theory]
    [InlineData(302, 0, 1, 42, 3, 0)]
    [InlineData(401, 0, 0, 0, 0, 0)]
    [InlineData(517, 0, 3, 0, 0, 0)]
    public void MailboxLayout_MatchesManual(int code, int w1, int w2, int w3, int w4, int w5)
    {
        var req = CommandMailbox.CreateRequest(code, w1, w2, w3, w4, w5);
        Assert.Equal(code, req[0]);
        Assert.Equal(w1, req[1]);
        Assert.Equal(w2, req[2]);
        Assert.Equal(w3, req[3]);
        Assert.Equal(w4, req[4]);
        Assert.Equal(w5, req[5]);
        Assert.Equal(0, req[6]);
    }

    [Fact]
    public void ReportId_SplitsLowHighWords()
    {
        var req = CommandMailbox.CreateRequest(750);
        CommandMailbox.SetReportId(req, 70000);
        Assert.Equal(70000 % 65536, req[2]);
        Assert.Equal(70000 / 65536, req[3]);
    }
}
