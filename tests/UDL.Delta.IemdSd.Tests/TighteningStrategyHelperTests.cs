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
    public void ApplyStrategyMask_SelfDefined_DoesNotClear()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[5].SpeedRpm = 300;
        TighteningStrategyHelper.ApplyStrategyMask(stages, TighteningStrategy.SelfDefined);
        Assert.Equal(300, stages[5].SpeedRpm);
    }

    [Fact]
    public void InferFromStages_Enhanced()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[3].SpeedRpm = 100;
        Assert.Equal(TighteningStrategy.Enhanced, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_PrePosition()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].TargetAngleDeg = 10;
        stages[1].SpeedRpm = 200;
        Assert.Equal(TighteningStrategy.PrePosition, TighteningStrategyHelper.InferFromStages(stages));
    }

    [Fact]
    public void InferFromStages_Standard()
    {
        var stages = TighteningParameterCore.CreateDefaultStages();
        stages[0].TargetAngleDeg = 10;
        stages[1].SpeedRpm = 200;
        stages[2].TargetTorqueMilliNm = 500;
        stages[3].TargetTorqueMilliNm = 1000;
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
}
