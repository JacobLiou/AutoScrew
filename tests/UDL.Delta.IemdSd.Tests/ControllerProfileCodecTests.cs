using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class ControllerProfileCodecTests
{
    [Fact]
    public void Sd3_43_Encode_TorqueMode_UsesPlainEnumAndNoMirror()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        var core = new TighteningParameterCore
        {
            Name = "P43",
            MaxLoosenAngleDeg = 90,
            Stages = TighteningParameterCore.CreateDefaultStages(),
        };
        core.Stages[0].ControlMode = TighteningControlMode.Torque;
        core.Stages[0].Direction = TighteningDirection.Clockwise;
        core.Stages[0].TwoStageModeEnabled = true;
        core.Stages[0].TargetTorqueMilliNm = 1500;

        TighteningParameterCodec.ApplyCoreToRaw(raw, core, IemdSdControllerProfile.Sd3_43);

        var baseIndex = TighteningParameterRegisterMap.StageStarts[0];
        Assert.Equal(1, raw[baseIndex]); // FA = torque enum
        Assert.Equal(0, raw[baseIndex + 1]); // FB = clockwise
        Assert.Equal(0, raw[baseIndex + 46] & (1 << 13)); // no +46 mirror
        Assert.Equal(90, raw[TighteningParameterRegisterMap.MaxLoosenAngle]); // 0xEC
    }

    [Fact]
    public void Sd3_101_Encode_TorqueMode_KeepsPackedBits()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        var core = new TighteningParameterCore
        {
            Name = "P101",
            MaxLoosenAngleDeg = 90,
            Stages = TighteningParameterCore.CreateDefaultStages(),
        };
        core.Stages[0].ControlMode = TighteningControlMode.Torque;
        core.Stages[0].Direction = TighteningDirection.Clockwise;
        core.Stages[0].TwoStageModeEnabled = true;

        TighteningParameterCodec.ApplyCoreToRaw(raw, core, IemdSdControllerProfile.Sd3_101);

        var baseIndex = TighteningParameterRegisterMap.StageStarts[0];
        var word0 = (ushort)raw[baseIndex];
        Assert.Equal(0x10, word0 & 0x30);
        Assert.True((word0 & (1 << 13)) != 0);
        Assert.True((raw[baseIndex + 46] & (1 << 13)) != 0);
        Assert.Equal(0, raw[TighteningParameterRegisterMap.MaxLoosenAngle]); // reserved on 10.1
    }

    [Theory]
    [InlineData(0x0001, 0x0000)]
    [InlineData(0x0010, 0x0000)]
    public void Decode_PlainAndPacked_Torque(ushort word0, ushort word1)
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        var baseIndex = TighteningParameterRegisterMap.StageStarts[0];
        raw[baseIndex] = word0;
        raw[baseIndex + 1] = word1;
        raw[baseIndex + TighteningParameterRegisterMap.StageTargetTorqueMilliNm] = 2000;

        var core = TighteningParameterCodec.ExtractCoreFromRaw(raw, IemdSdControllerProfile.Sd3_43);
        Assert.Equal(TighteningControlMode.Torque, core.Stages[0].ControlMode);
    }

    [Fact]
    public void Extract_Sd3_101_IgnoresReservedEc()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        raw[TighteningParameterRegisterMap.MaxLoosenAngle] = 123;
        var core = TighteningParameterCodec.ExtractCoreFromRaw(raw, IemdSdControllerProfile.Sd3_101);
        Assert.Equal(0, core.MaxLoosenAngleDeg);
    }

    [Fact]
    public void Extract_Sd3_43_ReadsEcAsMaxLoosenAngle()
    {
        var raw = TighteningParameterTemplate.CreateEmptyRawBlock();
        raw[TighteningParameterRegisterMap.MaxLoosenAngle] = 123;
        var core = TighteningParameterCodec.ExtractCoreFromRaw(raw, IemdSdControllerProfile.Sd3_43);
        Assert.Equal(123, core.MaxLoosenAngleDeg);
    }

    [Fact]
    public void ToMailboxWord4_Sd3_43_AlwaysZero()
    {
        Assert.Equal(0, TighteningStrategyHelper.ToMailboxWord4(TighteningStrategy.SelfDefined, IemdSdControllerProfile.Sd3_43));
        Assert.Equal(0, TighteningStrategyHelper.ToMailboxWord4(TighteningStrategy.Standard, IemdSdControllerProfile.Sd3_43));
    }

    [Fact]
    public void ToMailboxWord4_Sd3_101_SelfDefinedIsOne()
    {
        Assert.Equal(1, TighteningStrategyHelper.ToMailboxWord4(TighteningStrategy.SelfDefined, IemdSdControllerProfile.Sd3_101));
        Assert.Equal(0, TighteningStrategyHelper.ToMailboxWord4(TighteningStrategy.Standard, IemdSdControllerProfile.Sd3_101));
    }
}
