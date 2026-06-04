using UDL.Delta.IemdSd.Modbus;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class TighteningParameterCodecTests
{
    [Fact]
    public void NameRoundTrip_PreservesAscii()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        var core = new TighteningParameterCore { Name = "TestParam-01" };
        TighteningParameterCodec.ApplyCoreToRaw(raw, core);
        Assert.Equal("TestParam-01", TighteningParameterCodec.ReadName(raw));
    }

    [Fact]
    public void StageRoundTrip_PreservesModeledFields()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        raw[36] = 0xAAAA;
        raw[37] = 0xBBBB;

        var core = new TighteningParameterCore
        {
            Name = "StageTest",
            Stages = TighteningParameterCore.CreateDefaultStages(),
        };
        core.Stages[0].ControlMode = TighteningControlMode.Torque;
        core.Stages[0].TargetTorqueMilliNm = 2500;
        core.Stages[2].SpeedRpm = 1200;

        TighteningParameterCodec.ApplyCoreToRaw(raw, core);
        Assert.Equal(0xAAAA, raw[36]);
        Assert.Equal(0xBBBB, raw[37]);

        var decoded = TighteningParameterCodec.ExtractCoreFromRaw(raw);
        Assert.Equal(TighteningControlMode.Torque, decoded.Stages[0].ControlMode);
        Assert.Equal(2500, decoded.Stages[0].TargetTorqueMilliNm);
        Assert.Equal(1200, decoded.Stages[2].SpeedRpm);
    }

    [Fact]
    public void TemplateSync_RoundTripsCore()
    {
        var template = new TighteningParameterTemplate
        {
            ParameterId = 7,
            ToolIndex = 0,
            Core = new TighteningParameterCore
            {
                Name = "PN-A",
                MaxAngleDeg = 3600,
                Loosen = new TighteningLoosenCore { Stage1AngleDeg = 90, DetectTorqueMilliNm = 100 },
            },
        };
        template.Core.Stages[5].TargetAngleDeg = 180;

        template.ApplyCoreToRaw();
        var clone = new TighteningParameterTemplate
        {
            RawBlock = (int[])template.RawBlock.Clone(),
        };
        clone.SyncCoreFromRaw();

        Assert.Equal("PN-A", clone.Core.Name);
        Assert.Equal(3600, clone.Core.MaxAngleDeg);
        Assert.Equal(90, clone.Core.Loosen.Stage1AngleDeg);
        Assert.Equal(180, clone.Core.Stages[5].TargetAngleDeg);
    }

    [Fact]
    public void ParameterBlockWordCount_MatchesManual()
    {
        Assert.Equal(349, ModbusRegisterMap.ParameterBlockWordCount);
        Assert.Equal(0x22E, ModbusRegisterMap.ParameterBlockEnd);
    }
}
