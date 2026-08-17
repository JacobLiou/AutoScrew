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
}
