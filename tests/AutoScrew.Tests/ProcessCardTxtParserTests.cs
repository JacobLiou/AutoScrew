using AutoScrew.Infrastructure.ProcessLibrary;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProcessCardTxtParserTests
{
    [Fact]
    public void Parse_LegacySampleCard_MapsSlotScrewAndStages()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1830330479_00.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var result = ProcessCardTxtParser.ParseFile(path);

        Assert.Equal(0, result.SlotId);
        Assert.Equal("1830330479", result.ScrewPn);
        Assert.Equal(0, result.Template.ParameterId);
        Assert.Equal("1830330479", result.Template.Core.Name);

        Assert.Equal(4160, result.Template.Core.MaxAngleDeg);
        Assert.Equal(3800, result.Template.Core.MinAngleDeg);
        Assert.Equal(100, result.Template.Core.MaxTighteningTimeTenthSec);
        Assert.True(result.Template.Core.FinalCurrentJudgeEnabled);
        Assert.False(result.Template.Core.LastStageServoOn);

        var s0 = result.Template.Core.Stages[0];
        Assert.Equal(TighteningControlMode.Angle, s0.ControlMode);
        Assert.Equal(360, s0.TargetAngleDeg);
        Assert.Equal(80, s0.SpeedRpm);
        Assert.Equal(0, s0.MaxTorqueMilliNm); // 扭矩判断 OFF

        var s1 = result.Template.Core.Stages[1];
        Assert.Equal(TighteningControlMode.Angle, s1.ControlMode);
        Assert.Equal(3600, s1.TargetAngleDeg);
        Assert.Equal(600, s1.SpeedRpm);

        var s2 = result.Template.Core.Stages[2];
        Assert.Equal(TighteningControlMode.Torque, s2.ControlMode);
        Assert.True(s2.TargetTorqueMilliNm > 0);
        Assert.True(s2.MaxAngleDeg > 0); // 角度判断 ON

        var s3 = result.Template.Core.Stages[3];
        Assert.Equal(TighteningControlMode.Torque, s3.ControlMode);
        Assert.True(s3.TargetTorqueMilliNm > 0);
        Assert.True(s3.MaxTorqueMilliNm > 0); // 扭矩判断 ON

        Assert.Equal(240, result.Template.Core.Loosen.Stage1AngleDeg);
        Assert.Equal(60, result.Template.Core.Loosen.Stage1SpeedRpm);
        Assert.Equal(3800, result.Template.Core.Loosen.Stage2AngleDeg);
        Assert.Equal(500, result.Template.Core.Loosen.Stage2SpeedRpm);
        Assert.False(result.Template.Core.Loosen.ProductionLogEnabled);
        Assert.True(result.Template.Core.Loosen.DetectTorqueMilliNm > 0);
    }

    [Fact]
    public void Parse_FinalReviewedCard_MapsPnDashSlotAndJudges()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1830330479_00_v2.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var result = ProcessCardTxtParser.ParseFile(path);

        Assert.Equal(0, result.SlotId);
        Assert.Equal("1830330479", result.ScrewPn);
        Assert.Equal(0, result.Template.ParameterId);
        Assert.Equal("1830330479", result.Template.Core.Name);

        Assert.Equal(4160, result.Template.Core.MaxAngleDeg);
        Assert.Equal(3800, result.Template.Core.MinAngleDeg);
        Assert.False(result.Template.Core.FinalCurrentJudgeEnabled);

        var s0 = result.Template.Core.Stages[0];
        Assert.Equal(TighteningControlMode.Angle, s0.ControlMode);
        Assert.Equal(360, s0.TargetAngleDeg);
        Assert.Equal(80, s0.SpeedRpm);
        Assert.Equal(0, s0.MaxTorqueMilliNm); // 扭矩判断 OFF

        var s1 = result.Template.Core.Stages[1];
        Assert.Equal(TighteningControlMode.Angle, s1.ControlMode);
        Assert.Equal(3600, s1.TargetAngleDeg);
        Assert.Equal(600, s1.SpeedRpm);
        Assert.True(s1.MaxTorqueMilliNm > 0); // 扭矩判断 ON

        var s2 = result.Template.Core.Stages[2];
        Assert.Equal(TighteningControlMode.Torque, s2.ControlMode);
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(2.6, DefaultTorqueUnit.LbfIn),
            s2.TargetTorqueMilliNm);
        Assert.Equal(100, s2.MaxAngleDeg); // 角度判断 ON

        var s3 = result.Template.Core.Stages[3];
        Assert.Equal(TighteningControlMode.Torque, s3.ControlMode);
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(3.0, DefaultTorqueUnit.LbfIn),
            s3.TargetTorqueMilliNm);
        // 无「扭矩判断」行，但有最大/最小扭矩 → 仍写入
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(3.15, DefaultTorqueUnit.LbfIn),
            s3.MaxTorqueMilliNm);
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(2.85, DefaultTorqueUnit.LbfIn),
            s3.MinTorqueMilliNm);
        Assert.Equal(10, s3.MaxAngleDeg); // 角度判断 ON

        Assert.Equal(240, result.Template.Core.Loosen.Stage1AngleDeg);
        Assert.Equal(60, result.Template.Core.Loosen.Stage1SpeedRpm);
        Assert.Equal(3800, result.Template.Core.Loosen.Stage2AngleDeg);
        Assert.Equal(500, result.Template.Core.Loosen.Stage2SpeedRpm);
        Assert.True(result.Template.Core.Loosen.ProductionLogEnabled);
        Assert.True(result.Template.Core.Loosen.DetectTorqueMilliNm > 0);
    }
}
