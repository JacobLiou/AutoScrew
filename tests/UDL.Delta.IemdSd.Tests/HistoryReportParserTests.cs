using UDL.Delta.IemdSd.Internal;
using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace UDL.Delta.IemdSd.Tests;

public class HistoryReportParserTests
{
    [Fact]
    public void ParseError_ReadsTimestampAndCode()
    {
        var words = new[] { 2026, 8, 20, 14, 30, 26, 3224 };
        var entry = HistoryReportParser.ParseError(7, words);
        Assert.Equal(7u, entry.ReportId);
        Assert.Equal(new DateTime(2026, 8, 20, 14, 30, 26), entry.Timestamp);
        Assert.Equal((ushort)3224, entry.Code);
        Assert.Equal("NG3224", HistoryReportParser.FormatAlarmCode(entry.Code));
    }

    [Fact]
    public void ParseWarning_FormatsWnCode()
    {
        var words = new[] { 2026, 8, 20, 12, 59, 32, 5081 };
        var entry = HistoryReportParser.ParseWarning(3, words);
        Assert.Equal("WN5081", HistoryReportParser.FormatAlarmCode(entry.Code));
    }

    [Fact]
    public void ParseButton_ReadsBeforeAfterAndUser()
    {
        var words = new[] { 2026, 8, 20, 13, 2, 44, 4000, 0, 0, 1, 0, 1 };
        var entry = HistoryReportParser.ParseButton(18, words);
        Assert.Equal((ushort)4000, entry.ButtonId);
        Assert.Equal(0u, entry.ValueBefore);
        Assert.Equal(1u, entry.ValueAfter);
        Assert.Equal("User1", HistoryReportParser.FormatUserAccount(entry.UserId));
    }

    [Fact]
    public void ReportReader_Parse_IncludesTimestamp()
    {
        var words = new int[253];
        words[0x136 - ModbusRegisterMap.CommandData] = 2026;
        words[0x137 - ModbusRegisterMap.CommandData] = 8;
        words[0x138 - ModbusRegisterMap.CommandData] = 20;
        words[0x139 - ModbusRegisterMap.CommandData] = 14;
        words[0x13A - ModbusRegisterMap.CommandData] = 37;
        words[0x13B - ModbusRegisterMap.CommandData] = 15;
        words[0x13C - ModbusRegisterMap.CommandData] = 0;
        words[0x145 - ModbusRegisterMap.CommandData] = 2123;
        words[0x146 - ModbusRegisterMap.CommandData] = 2213;
        words[0x147 - ModbusRegisterMap.CommandData] = 1;
        words[0x143 - ModbusRegisterMap.CommandData] = 415;
        words[0x14E - ModbusRegisterMap.CommandData] = 3;

        var report = ReportReader.Parse(75, words);
        Assert.Equal(new DateTime(2026, 8, 20, 14, 37, 15), report.Timestamp);
        Assert.Equal(DeviceTighteningStatus.Ok, report.Status);
        Assert.Equal(2123, report.TighteningAngle);
        Assert.Equal(2213, report.TotalAngle);
        Assert.Equal((ushort)3, report.TorqueUnit);
        Assert.Equal("lbf.in", HistoryReportParser.FormatTorqueUnit(report.TorqueUnit));
    }
}
