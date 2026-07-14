using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public sealed class ModbusCommandErrorCatalogTests
{
    [Theory]
    [InlineData(150, 3, "未配置该参数 ID")]
    [InlineData(302, 1, "手动设定")]
    [InlineData(160, 1, "工具索引")]
    [InlineData(250, 3, "未配置该顺序")]
    [InlineData(303, 1, "手动设定")]
    public void Describe_MapsProductionChainErrors(int command, int code, string contains)
    {
        var text = ModbusCommandErrorCatalog.Describe(command, code);
        Assert.Contains(contains, text, StringComparison.Ordinal);
    }
}
