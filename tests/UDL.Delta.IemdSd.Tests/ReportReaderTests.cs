using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class ReportReaderTests
{
    [Fact]
    public void Parse_ReadsStatusAndTool()
    {
        var words = new int[253];
        words[0x13C - ModbusRegisterMap.CommandData] = 2;
        words[0x147 - ModbusRegisterMap.CommandData] = (int)DeviceTighteningStatus.Ok;
        words[0x146 - ModbusRegisterMap.CommandData] = 120;
        words[0x17D - ModbusRegisterMap.CommandData] = 500;
        words[0x17E - ModbusRegisterMap.CommandData] = 0;

        var report = ReportReader.Parse(42, words);
        Assert.Equal(42u, report.ReportId);
        Assert.Equal(2, report.Tool);
        Assert.Equal(DeviceTighteningStatus.Ok, report.Status);
        Assert.Equal(120, report.TotalAngle);
        Assert.Equal(0.5f, report.AppliedTorqueNm, 3);
    }
}
