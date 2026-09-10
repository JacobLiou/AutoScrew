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
    public void NamePacking_UsesLowByteFirst_MatchingDeviceHmi()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        TighteningParameterCodec.WriteName(raw, "388");
        // LE within register: word0 = ('8' << 8) | '3' = 0x3833
        Assert.Equal(0x3833, raw[0]);
        Assert.Equal(0x0038, raw[1]);
        Assert.Equal("388", TighteningParameterCodec.ReadName(raw));
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
    public void StageFlags_ErgoStopAndTwoStage_RoundTripPackedBits()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        var core = new TighteningParameterCore
        {
            Name = "Flags",
            Stages = TighteningParameterCore.CreateDefaultStages(),
        };
        var stage = core.Stages[3];
        stage.ControlMode = TighteningControlMode.Torque;
        stage.Direction = TighteningDirection.CounterClockwise;
        stage.TwoStageModeEnabled = true;
        stage.ErgoStopEnabled = true;
        stage.Segment1TorqueMilliNm = 1800;
        stage.FinalSpeedRpm = 100;

        TighteningParameterCodec.ApplyCoreToRaw(raw, core);

        var baseIndex = TighteningParameterRegisterMap.StageStarts[3];
        var word0 = (ushort)raw[baseIndex];
        Assert.Equal(0x10, word0 & 0x30); // torque target bits
        Assert.True((word0 & (1 << 13)) != 0); // two-stage
        Assert.True((word0 & (1 << 6)) != 0); // CCW
        Assert.True((raw[baseIndex + 46] & (1 << 13)) != 0); // enable mirror
        Assert.True((((ushort)raw[baseIndex + 1]) & 0x4) != 0); // ergo in low bits of word1

        var decoded = TighteningParameterCodec.ExtractCoreFromRaw(raw).Stages[3];
        Assert.Equal(TighteningControlMode.Torque, decoded.ControlMode);
        Assert.Equal(TighteningDirection.CounterClockwise, decoded.Direction);
        Assert.True(decoded.TwoStageModeEnabled);
        Assert.True(decoded.ErgoStopEnabled);
        Assert.Equal(1800, decoded.Segment1TorqueMilliNm);
        Assert.Equal(100, decoded.FinalSpeedRpm);
    }

    [Fact]
    public void StageFlags_LegacyPlainControlWord_StillReads()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        var baseIndex = TighteningParameterRegisterMap.StageStarts[0];
        raw[baseIndex] = (int)TighteningControlMode.Torque; // legacy plain enum
        raw[baseIndex + 1] = (int)TighteningDirection.Clockwise;
        raw[baseIndex + TighteningParameterRegisterMap.StageSpeedRpm] = 500;

        var decoded = TighteningParameterCodec.ExtractCoreFromRaw(raw).Stages[0];
        Assert.Equal(TighteningControlMode.Torque, decoded.ControlMode);
        Assert.Equal(TighteningDirection.Clockwise, decoded.Direction);
        Assert.Equal(500, decoded.SpeedRpm);
        Assert.False(decoded.ErgoStopEnabled);
        Assert.False(decoded.TwoStageModeEnabled);
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
