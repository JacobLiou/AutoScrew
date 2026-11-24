using AutoScrew.Infrastructure.Hardware;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ControllerParameterPresetWritableTests
{
    [Fact]
    public void EnsureWritableStages_TorqueModeZeroTarget_Throws()
    {
        var template = new TighteningParameterTemplate
        {
            ParameterId = 1,
            Core = new TighteningParameterCore
            {
                Stages = TighteningParameterCore.CreateDefaultStages(),
            },
        };
        template.Core.Stages[0].ControlMode = TighteningControlMode.Torque;
        template.Core.Stages[0].SpeedRpm = 100;
        template.Core.Stages[0].TargetTorqueMilliNm = 0;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ControllerParameterPresetService.EnsureWritableStages(template));
        Assert.Contains("目标扭矩为 0", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void EnsureWritableStages_TimeoutOverLimit_Throws()
    {
        var template = new TighteningParameterTemplate
        {
            ParameterId = 1,
            Core = new TighteningParameterCore
            {
                Stages = TighteningParameterCore.CreateDefaultStages(),
                MaxTighteningTimeTenthSec = 40000,
            },
        };
        template.Core.Stages[0].SpeedRpm = 80;
        template.Core.Stages[0].TargetAngleDeg = 90;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ControllerParameterPresetService.EnsureWritableStages(template));
        Assert.Contains("32767", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void EnsureWritableStages_AngleOnlyStart_Ok()
    {
        var template = new TighteningParameterTemplate
        {
            ParameterId = 1,
            Core = new TighteningParameterCore
            {
                Strategy = TighteningStrategy.SelfDefined,
                Stages = TighteningParameterCore.CreateDefaultStages(),
                MaxTighteningTimeTenthSec = 10,
                MaxLoosenTimeTenthSec = 0,
            },
        };
        template.Core.Stages[0].ControlMode = TighteningControlMode.Angle;
        template.Core.Stages[0].SpeedRpm = 300;
        template.Core.Stages[0].TargetAngleDeg = 1080;

        ControllerParameterPresetService.EnsureWritableStages(template);
    }
}
