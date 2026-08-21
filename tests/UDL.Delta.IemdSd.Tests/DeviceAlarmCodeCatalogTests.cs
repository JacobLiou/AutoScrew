using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class DeviceAlarmCodeCatalogTests
{
    [Fact]
    public void TryGetChineseDescription_RundownMaxTorque_Decimal()
    {
        var zh = DeviceAlarmCodeCatalog.TryGetChineseDescription(3224);
        Assert.Equal("旋入阶段:超出最大扭矩值", zh);
        Assert.Equal("NG3224", HistoryReportParser.FormatAlarmCode(3224));
    }

    [Fact]
    public void TryGetChineseDescription_RundownMaxTorque_HexRegister()
    {
        // Protocol D8 stores 0x3224 for NG3224
        const ushort reg = 0x3224;
        Assert.Equal("NG3224", HistoryReportParser.FormatAlarmCode(reg));
        Assert.Equal("旋入阶段:超出最大扭矩值", DeviceAlarmCodeCatalog.TryGetChineseDescription(reg));
    }

    [Fact]
    public void TryGetChineseDescription_Warning5081_HexRegister()
    {
        const ushort reg = 0x5081;
        Assert.Equal("WN5081", HistoryReportParser.FormatAlarmCode(reg));
        Assert.Equal("拧紧OK后禁止拧松", DeviceAlarmCodeCatalog.TryGetChineseDescription(reg));
    }

    [Fact]
    public void TryGetChineseDescription_Warning5007_HexAndDecimal()
    {
        Assert.Equal("WN5007", HistoryReportParser.FormatAlarmCode(5007));
        Assert.Equal("WN5007", HistoryReportParser.FormatAlarmCode(0x5007));
        Assert.Contains("电流", DeviceAlarmCodeCatalog.TryGetChineseDescription(0x5007)!);
        Assert.Contains("电流", DeviceAlarmCodeCatalog.TryGetChineseDescription(5007)!);
    }

    [Fact]
    public void TryGetChineseDescription_Overcurrent()
    {
        Assert.Equal("过电流", DeviceAlarmCodeCatalog.TryGetChineseDescription(1001));
        Assert.Equal("过电流", DeviceAlarmCodeCatalog.TryGetChineseDescription(0x1001));
        Assert.Equal("过电流", DeviceAlarmCodeCatalog.TryGetChineseDescription(2001));
        Assert.Equal("过电流", DeviceAlarmCodeCatalog.TryGetChineseDescription(0x2001));
    }

    [Fact]
    public void TryGetChineseDescription_Unknown_ReturnsNull()
    {
        Assert.Null(DeviceAlarmCodeCatalog.TryGetChineseDescription(0));
        Assert.Null(DeviceAlarmCodeCatalog.TryGetChineseDescription(999));
    }

    [Fact]
    public void FormatAlarmCode_HexLetterCodes()
    {
        Assert.Equal("WN500A", HistoryReportParser.FormatAlarmCode(0x500A));
        Assert.Equal("NG3A10", HistoryReportParser.FormatAlarmCode(0x3A10));
    }
}
