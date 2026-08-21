using AutoScrew.Infrastructure.ProcessLibrary;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProcessCardTxtWriterTests
{
    [Fact]
    public void Format_RoundTrip_FixturePreservesIdentityAndKeyValues()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1830330479_00.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var original = ProcessCardTxtParser.ParseFile(path);
        var text = ProcessCardTxtWriter.Format(original.Template, original.ScrewPn, original.SlotId);
        var again = ProcessCardTxtParser.Parse(text, "1830330479_00.txt");

        Assert.Equal(original.ScrewPn, again.ScrewPn);
        Assert.Equal(original.SlotId, again.SlotId);
        Assert.Equal(original.Template.ParameterId, again.Template.ParameterId);
        Assert.Equal(original.Template.Core.MaxAngleDeg, again.Template.Core.MaxAngleDeg);
        Assert.Equal(original.Template.Core.MinAngleDeg, again.Template.Core.MinAngleDeg);
        Assert.Equal(original.Template.Core.MaxTighteningTimeTenthSec, again.Template.Core.MaxTighteningTimeTenthSec);
        Assert.Equal(original.Template.Core.FinalCurrentJudgeEnabled, again.Template.Core.FinalCurrentJudgeEnabled);

        Assert.Equal(original.Template.Core.Stages[0].TargetAngleDeg, again.Template.Core.Stages[0].TargetAngleDeg);
        Assert.Equal(original.Template.Core.Stages[0].SpeedRpm, again.Template.Core.Stages[0].SpeedRpm);
        Assert.Equal(original.Template.Core.Stages[1].TargetAngleDeg, again.Template.Core.Stages[1].TargetAngleDeg);
        Assert.Equal(original.Template.Core.Stages[2].ControlMode, again.Template.Core.Stages[2].ControlMode);
        Assert.Equal(original.Template.Core.Stages[2].TargetTorqueMilliNm, again.Template.Core.Stages[2].TargetTorqueMilliNm);
        Assert.Equal(original.Template.Core.Stages[3].TargetTorqueMilliNm, again.Template.Core.Stages[3].TargetTorqueMilliNm);
        Assert.Equal(original.Template.Core.Stages[3].MaxTorqueMilliNm, again.Template.Core.Stages[3].MaxTorqueMilliNm);

        Assert.Equal(original.Template.Core.Loosen.Stage1AngleDeg, again.Template.Core.Loosen.Stage1AngleDeg);
        Assert.Equal(original.Template.Core.Loosen.Stage2AngleDeg, again.Template.Core.Loosen.Stage2AngleDeg);
        Assert.Equal(original.Template.Core.Loosen.DetectTorqueMilliNm, again.Template.Core.Loosen.DetectTorqueMilliNm);
    }

    [Fact]
    public void Format_UsesProvidedSlot_NotEditorParameterId()
    {
        var template = new TighteningParameterTemplate
        {
            ParameterId = 9,
            Core = new TighteningParameterCore
            {
                Name = "OLD",
                Stages = TighteningParameterCore.CreateDefaultStages(),
                Loosen = new TighteningLoosenCore(),
            },
        };
        template.Core.Stages[0].ControlMode = TighteningControlMode.Angle;
        template.Core.Stages[0].TargetAngleDeg = 90;
        template.Core.Stages[0].SpeedRpm = 50;

        var text = ProcessCardTxtWriter.Format(template, "1830330999", slotId: 2);
        var parsed = ProcessCardTxtParser.Parse(text);

        Assert.Equal("1830330999", parsed.ScrewPn);
        Assert.Equal(2, parsed.SlotId);
        Assert.Equal(3, parsed.Template.ParameterId);
    }

    [Fact]
    public void Format_RejectsEmptyScrewPn()
    {
        var template = new TighteningParameterTemplate
        {
            ParameterId = 1,
            Core = new TighteningParameterCore { Stages = TighteningParameterCore.CreateDefaultStages() },
        };
        Assert.Throws<InvalidDataException>(() => ProcessCardTxtWriter.Format(template, "!!!", 0));
    }

    [Fact]
    public void Format_WritesLastStageServoUnderTightenCondition_NotUnderAdvanced()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1830330479_00.txt");
        var original = ProcessCardTxtParser.ParseFile(path);
        original.Template.Core.LastStageServoOn = true;
        original.Template.Core.LinkedCompensationParamId = 42;

        var text = ProcessCardTxtWriter.Format(original.Template, original.ScrewPn, original.SlotId);

        var tightenIdx = text.IndexOf("拧紧条件", StringComparison.Ordinal);
        var loosenIdx = text.IndexOf("拧松条件", StringComparison.Ordinal);
        var advancedIdx = text.IndexOf("进阶设定", StringComparison.Ordinal);
        var servoIdx = text.IndexOf("末段伺服保持", StringComparison.Ordinal);
        var linkedIdx = text.IndexOf("关联补偿参数ID", StringComparison.Ordinal);

        Assert.True(tightenIdx >= 0);
        Assert.True(servoIdx > tightenIdx && servoIdx < loosenIdx);
        Assert.True(linkedIdx > tightenIdx && linkedIdx < loosenIdx);
        var advancedSection = text[advancedIdx..];
        Assert.DoesNotContain("末段伺服保持", advancedSection);
        Assert.DoesNotContain("关联补偿参数ID", advancedSection);
    }

    [Fact]
    public void Format_WritesStrategyAndRoundTripsSelfDefinedAdvanced()
    {
        var core = new TighteningParameterCore
        {
            Name = "RT",
            Strategy = TighteningStrategy.SelfDefined,
            MaxAngleDeg = 100,
            MaxTighteningTimeTenthSec = 50,
            Stages = TighteningParameterCore.CreateDefaultStages(),
            Loosen = new TighteningLoosenCore
            {
                Stage1AngleDeg = 10,
                Stage1SpeedRpm = 20,
                Stage1AccelMs = 33,
                Stage2AccelMs = 44,
            },
        };
        core.Stages[0].ControlMode = TighteningControlMode.Angle;
        core.Stages[0].TargetAngleDeg = 90;
        core.Stages[0].SpeedRpm = 80;
        core.Stages[0].AccelTimeMs = 12;
        core.Stages[0].PauseTimeMs = 5;
        core.Stages[4].ControlMode = TighteningControlMode.Torque;
        core.Stages[4].TargetTorqueMilliNm = TorqueUnitConverter.DisplayToMilliNm(1.5, DefaultTorqueUnit.LbfIn);
        core.Stages[4].SpeedRpm = 150;
        core.Stages[5].ControlMode = TighteningControlMode.Angle;
        core.Stages[5].TargetAngleDeg = 30;
        core.Stages[5].SpeedRpm = 40;

        var template = new TighteningParameterTemplate { ParameterId = 1, Core = core };
        var text = ProcessCardTxtWriter.Format(template, "RT", 0);
        Assert.Contains("策略：自创", text, StringComparison.Ordinal);
        Assert.Contains("5.阶段5", text, StringComparison.Ordinal);
        Assert.Contains("6.阶段6", text, StringComparison.Ordinal);
        Assert.Contains("控制模式：", text, StringComparison.Ordinal);

        var again = ProcessCardTxtParser.Parse(text);
        Assert.Equal(TighteningStrategy.SelfDefined, again.Template.Core.Strategy);
        Assert.Equal(12, again.Template.Core.Stages[0].AccelTimeMs);
        Assert.Equal(5, again.Template.Core.Stages[0].PauseTimeMs);
        Assert.Equal(150, again.Template.Core.Stages[4].SpeedRpm);
        Assert.Equal(30, again.Template.Core.Stages[5].TargetAngleDeg);
        Assert.Equal(33, again.Template.Core.Loosen.Stage1AccelMs);
        Assert.Equal(44, again.Template.Core.Loosen.Stage2AccelMs);
    }

    [Fact]
    public void Format_Enhanced_WritesOnlyTightenStage()
    {
        var core = new TighteningParameterCore
        {
            Strategy = TighteningStrategy.Enhanced,
            MaxTighteningTimeTenthSec = 10,
            Stages = TighteningParameterCore.CreateDefaultStages(),
            Loosen = new TighteningLoosenCore(),
        };
        core.Stages[3].ControlMode = TighteningControlMode.Torque;
        core.Stages[3].TargetTorqueMilliNm = 500;
        core.Stages[3].SpeedRpm = 200;

        var text = ProcessCardTxtWriter.Format(
            new TighteningParameterTemplate { ParameterId = 1, Core = core },
            "ENH",
            0);
        Assert.Contains("策略：加强", text, StringComparison.Ordinal);
        Assert.Contains("4.拧紧", text, StringComparison.Ordinal);
        Assert.DoesNotContain("1.启动", text, StringComparison.Ordinal);
    }
}
