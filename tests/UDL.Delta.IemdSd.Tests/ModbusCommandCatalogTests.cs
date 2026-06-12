using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class ModbusCommandCatalogTests
{
    [Fact]
    public void Catalog_ContainsExpectedProductionCodes()
    {
        Assert.True(ModbusCommandCatalog.TryGet(100, out var writeParam));
        Assert.True(ModbusCommandCatalog.TryGet(150, out var readParam));
        Assert.True(ModbusCommandCatalog.TryGet(302, out var switchParam));
        Assert.True(ModbusCommandCatalog.TryGet(517, out var export));
        Assert.True(ModbusCommandCatalog.TryGet(750, out var report));
        Assert.True(ModbusCommandCatalog.TryGet(751, out var curve));

        Assert.Equal("Write the parameter", writeParam.Name);
        Assert.Equal(ModbusCommandPayloadKind.WriteThenMailbox, writeParam.PayloadKind);
        Assert.Equal(349u, writeParam.TypicalDataWordCount);
        Assert.Equal("Find and read curves", curve.Name);
        Assert.Equal(PerScrewExportMode.BinHmiDisk, (PerScrewExportMode)3);
        Assert.NotNull(export);
    }

    [Fact]
    public void Catalog_HasNoDuplicateCodes()
    {
        var codes = ModbusCommandCatalog.All.Keys.ToList();
        Assert.Equal(codes.Count, codes.Distinct().Count());
        Assert.True(codes.Count >= 140);
    }

    [Fact]
    public void Enum_MatchesCatalogForCoreCommands()
    {
        Assert.Equal(100, (int)ModbusFunctionCode.Write_parameter);
        Assert.Equal(751, (int)ModbusFunctionCode.Find_read_curves);
        Assert.True(ModbusCommandCatalog.TryGet((int)ModbusFunctionCode.Write_barcode_string, out _));
    }
}
