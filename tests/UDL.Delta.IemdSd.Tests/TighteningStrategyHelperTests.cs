using UDL.Delta.IemdSd.Protocol;

namespace UDL.Delta.IemdSd.Tests;

public class TighteningStrategyHelperTests
{
    [Theory]
    [InlineData(TighteningStrategy.Standard, 0)]
    [InlineData(TighteningStrategy.Enhanced, 0)]
    [InlineData(TighteningStrategy.PrePosition, 0)]
    [InlineData(TighteningStrategy.SelfDefined, 1)]
    public void ToMailboxWord4_MatchesProtocol(TighteningStrategy strategy, int expected) =>
        Assert.Equal(expected, TighteningStrategyHelper.ToMailboxWord4(strategy));

    [Fact]
    public void ApplyStrategyMask_Enhanced_ClearsAllButTightenSlot()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 100;
        stages[1].TargetAngleDeg = 90;
        stages[3].SpeedRpm = 200;
        stages[3].TargetTorqueMilliNm = 1500;

        TighteningStrategyHelper.ApplyStrategyMask(stages, TighteningStrategy.Enhanced);

        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[0]));
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[1]));
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[2]));
        Assert.True(TighteningStrategyHelper.IsStageConfigured(stages[3]));
        Assert.Equal(200, stages[3].SpeedRpm);
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[4]));
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[5]));
    }

    [Fact]
    public void ApplyStrategyMask_PrePosition_KeepsStartAndRundown()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].TargetAngleDeg = 30;
        stages[1].SpeedRpm = 500;
        stages[2].TargetTorqueMilliNm = 100;
        stages[3].TargetTorqueMilliNm = 200;

        TighteningStrategyHelper.ApplyStrategyMask(stages, TighteningStrategy.PrePosition);

        Assert.True(TighteningStrategyHelper.IsStageConfigured(stages[0]));
        Assert.True(TighteningStrategyHelper.IsStageConfigured(stages[1]));
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[2]));
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[3]));
    }

    [Fact]
    public void ApplyStrategyMask_Standard_ClearsSlots4And5()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        for (var i = 0; i < 6; i++)
            stages[i].SpeedRpm = 100 + i;

        TighteningStrategyHelper.ApplyStrategyMask(stages, TighteningStrategy.Standard);

        Assert.True(TighteningStrategyHelper.IsStageConfigured(stages[0]));
        Assert.True(TighteningStrategyHelper.IsStageConfigured(stages[3]));
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[4]));
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[5]));
    }

    [Fact]
    public void ApplyStrategyMask_SelfDefined_KeepsConfigured_ClearsEmpty()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 300;
        stages[0].TargetAngleDeg = 1080;
        stages[2].ControlMode = TighteningControlMode.Torque; // 空段误标扭矩@0
        stages[5].SpeedRpm = 300;
        TighteningStrategyHelper.ApplyStrategyMask(stages, TighteningStrategy.SelfDefined);

        Assert.Equal(300, stages[0].SpeedRpm);
        Assert.Equal(1080, stages[0].TargetAngleDeg);
        Assert.Equal(300, stages[5].SpeedRpm);
        Assert.False(TighteningStrategyHelper.IsStageConfigured(stages[2]));
        Assert.Equal(TighteningControlMode.Angle, stages[2].ControlMode);
        Assert.Equal(0, stages[2].TargetTorqueMilliNm);
    }

    [Fact]
    public void InferFromStages_Enhanced_AbsoluteSlot3Only()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[3].SpeedRpm = 100;
        stages[3].ControlMode = TighteningControlMode.Torque;
        Assert.Equal(TighteningStrategy.Enhanced, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_StartOnly_IsSelfDefined_NotEnhanced()
    {
        // #150 绝对布局：仅槽 0 有转速 = 自创子集，不得按 SDSoft UI 重映射判为加强
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 300;
        stages[0].ControlMode = TighteningControlMode.Angle;
        stages[0].TargetAngleDeg = 1080;
        Assert.Equal(TighteningStrategy.SelfDefined, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_StartTorqueOnly_IsSelfDefined_NotEnhanced()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 100;
        stages[0].ControlMode = TighteningControlMode.Torque;
        Assert.Equal(TighteningStrategy.SelfDefined, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_PrePosition()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 80;
        stages[0].ControlMode = TighteningControlMode.Angle;
        stages[1].SpeedRpm = 200;
        stages[1].ControlMode = TighteningControlMode.Torque;
        Assert.Equal(TighteningStrategy.PrePosition, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_Standard_ControlModePattern()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 100;
        stages[0].ControlMode = TighteningControlMode.Angle;
        stages[1].SpeedRpm = 200;
        stages[1].ControlMode = TighteningControlMode.Torque;
        stages[2].SpeedRpm = 150;
        stages[2].ControlMode = TighteningControlMode.Torque;
        stages[3].SpeedRpm = 80;
        stages[3].ControlMode = TighteningControlMode.Torque;
        Assert.Equal(TighteningStrategy.Standard, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_SelfDefined_WhenSlot5Configured()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 100;
        stages[5].SpeedRpm = 50;
        Assert.Equal(TighteningStrategy.SelfDefined, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_SelfDefined_WhenDirectionChanges()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 100;
        stages[0].ControlMode = TighteningControlMode.Angle;
        stages[0].Direction = TighteningDirection.Clockwise;
        stages[1].SpeedRpm = 200;
        stages[1].ControlMode = TighteningControlMode.Torque;
        stages[1].Direction = TighteningDirection.CounterClockwise;
        stages[2].SpeedRpm = 150;
        stages[2].ControlMode = TighteningControlMode.Torque;
        stages[3].SpeedRpm = 80;
        stages[3].ControlMode = TighteningControlMode.Torque;
        Assert.Equal(TighteningStrategy.SelfDefined, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_SelfDefined_WhenCompTorqueEnabled()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 100;
        stages[0].ControlMode = TighteningControlMode.Angle;
        stages[1].SpeedRpm = 200;
        stages[1].ControlMode = TighteningControlMode.Torque;
        stages[2].SpeedRpm = 150;
        stages[2].ControlMode = TighteningControlMode.Torque;
        stages[3].SpeedRpm = 80;
        stages[3].ControlMode = TighteningControlMode.Torque;
        stages[1].CompTorqueEnabled = true;
        Assert.Equal(TighteningStrategy.SelfDefined, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromCore_SelfDefined_WhenLinkedCompensationParamIdSet()
    {
        var core = new TighteningParameterCore
        {
            LinkedCompensationParamId = 2,
            Stages = TighteningParameterCore.CreateDefaultStages(),
        };
        core.Stages[0].SpeedRpm = 100;
        core.Stages[0].ControlMode = TighteningControlMode.Angle;
        core.Stages[1].SpeedRpm = 200;
        core.Stages[1].ControlMode = TighteningControlMode.Torque;
        core.Stages[2].SpeedRpm = 150;
        core.Stages[2].ControlMode = TighteningControlMode.Torque;
        core.Stages[3].SpeedRpm = 80;
        core.Stages[3].ControlMode = TighteningControlMode.Torque;

        Assert.Equal(TighteningStrategy.SelfDefined, TighteningStrategyHelper.InferFromCore(core));
    }

    [Fact]
    public void InferFromStages_SelfDefined_WhenPatternDoesNotMatchTemplates()
    {
        // 四段都有转速，但控制模式不像标准模板 → 自创（不再误判标准）
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].SpeedRpm = 100;
        stages[0].ControlMode = TighteningControlMode.Torque;
        stages[1].SpeedRpm = 200;
        stages[1].ControlMode = TighteningControlMode.Torque;
        stages[2].SpeedRpm = 150;
        stages[2].ControlMode = TighteningControlMode.Angle;
        stages[3].SpeedRpm = 80;
        stages[3].ControlMode = TighteningControlMode.Angle;
        Assert.Equal(TighteningStrategy.SelfDefined, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_Empty_DefaultsToStandard()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        Assert.Equal(TighteningStrategy.Standard, TighteningStrategyHelper.InferFromStages(stages));
    }
}
