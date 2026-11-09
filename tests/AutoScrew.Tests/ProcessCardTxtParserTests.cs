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
        Assert.Equal(1, result.Template.ParameterId);
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
    public void Parse_LastStageServoInTightenConditionSection_MapsCorrectly()
    {
        var text = """
            参数：TEST-00
            阶段有效：1 阶段有效
            基本设定
            拧紧条件
            旋转方向：顺时针
            最大总角度（°）：100
            最小总角度（°）：0
            最大拧紧时间（秒）：1
            拧紧启动延时（×0.01）：0
            末段伺服保持：ON
            关联补偿参数ID：7
            拧松条件
            最大拧松时间（秒）：1
            进阶设定
            最终电流判定：OFF
            1.启动
            拧紧角度（°）：90
            速度（转/分钟）：80
            扭矩判断：OFF
            """;

        var result = ProcessCardTxtParser.Parse(text);
        Assert.True(result.Template.Core.LastStageServoOn);
        Assert.Equal(7, result.Template.Core.LinkedCompensationParamId);
    }

    [Fact]
    public void Parse_FinalReviewedCard_MapsPnDashSlotAndJudges()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1830330479_00_v2.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var result = ProcessCardTxtParser.ParseFile(path);

        Assert.Equal(0, result.SlotId);
        Assert.Equal("1830330479", result.ScrewPn);
        Assert.Equal(1, result.Template.ParameterId);
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

    [Fact]
    public void Parse_EmptyParamIdLine_UsesPnDashSlotForDeviceId()
    {
        var text = """
            参数ID：
            参数：1830330479-01
            阶段有效：1 阶段有效
            旋转方向：顺时针
            最大总角度（°）：100
            最小总角度（°）：0
            最大拧紧时间（秒）：1
            1.启动
            拧紧角度（°）：90
            速度（转/分钟）：80
            扭矩判断：OFF
            """;

        var result = ProcessCardTxtParser.Parse(text);
        Assert.Equal(1, result.SlotId);
        Assert.Equal(2, result.Template.ParameterId);
        Assert.Equal("1830330479", result.ScrewPn);
    }

    [Fact]
    public void Parse_FilenameFallback_WhenParamMissing()
    {
        var dir = Path.Combine(Path.GetTempPath(), "autoscrew-card-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var path = Path.Combine(dir, "1830330479_03.txt");
            File.WriteAllText(path, """
                阶段有效：1 阶段有效
                旋转方向：顺时针
                最大总角度（°）：100
                最小总角度（°）：0
                最大拧紧时间（秒）：1
                1.启动
                拧紧角度（°）：90
                速度（转/分钟）：80
                扭矩判断：OFF
                """);

            var result = ProcessCardTxtParser.ParseFile(path);
            Assert.Equal(3, result.SlotId);
            Assert.Equal(4, result.Template.ParameterId);
            Assert.Equal("1830330479", result.ScrewPn);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void Parse_LegacyCard_DefaultsStrategyToStandard()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1830330479_00.txt");
        var result = ProcessCardTxtParser.ParseFile(path);
        Assert.Equal(TighteningStrategy.Standard, result.Template.Core.Strategy);
    }

    [Fact]
    public void Parse_AllTemplateParams_MapsSelfDefinedAndAdvanced()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "AllTemplateParams.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var result = ProcessCardTxtParser.ParseFile(path);

        Assert.Equal("ALLTPL", result.ScrewPn);
        Assert.Equal(0, result.SlotId);
        Assert.Equal(TighteningStrategy.SelfDefined, result.Template.Core.Strategy);

        Assert.Equal(TighteningControlMode.Angle, result.Template.Core.Stages[0].ControlMode);
        Assert.Equal(360, result.Template.Core.Stages[0].TargetAngleDeg);
        Assert.Equal(50, result.Template.Core.Stages[0].AccelTimeMs);
        Assert.Equal(10, result.Template.Core.Stages[0].TorqueRateAngleIntervalTenthDeg);

        Assert.Equal(TighteningControlMode.TorqueRate, result.Template.Core.Stages[4].ControlMode);
        Assert.Equal(TighteningControlMode.ClampAngle, result.Template.Core.Stages[5].ControlMode);
        Assert.True(result.Template.Core.Stages[5].SpeedRpm > 0);

        Assert.Equal(100, result.Template.Core.Loosen.Stage1AccelMs);
        Assert.Equal(100, result.Template.Core.Loosen.Stage2AccelMs);
        Assert.True(result.Template.Core.Stages[3].Segment1TorqueMilliNm > 0);
        Assert.Equal(30, result.Template.Core.Stages[3].FinalSpeedRpm);
    }

    [Fact]
    public void Parse_EnhancedStrategy_FillsSlot3FromStage4Header()
    {
        var text = """
            参数：ENH-00
            策略：加强
            阶段有效：1 阶段有效
            旋转方向：顺时针
            最大总角度（°）：100
            最大拧紧时间（秒）：5
            4.拧紧
            控制模式：扭矩
            扭矩（lbf.in）：3.0
            速度（转/分钟）：200
            扭矩判断：ON
            最大扭矩（lbf.in）：3.5
            最小扭矩（lbf.in）：2.5
            """;

        var result = ProcessCardTxtParser.Parse(text);
        Assert.Equal(TighteningStrategy.Enhanced, result.Template.Core.Strategy);
        Assert.Equal(0, result.Template.Core.Stages[0].SpeedRpm);
        Assert.Equal(200, result.Template.Core.Stages[3].SpeedRpm);
        Assert.Equal(TighteningControlMode.Torque, result.Template.Core.Stages[3].ControlMode);
    }

    [Theory]
    [InlineData("标准", TighteningStrategy.Standard)]
    [InlineData("加强", TighteningStrategy.Enhanced)]
    [InlineData("预定位", TighteningStrategy.PrePosition)]
    [InlineData("自创", TighteningStrategy.SelfDefined)]
    public void ParseStrategy_MapsChineseLabels(string label, TighteningStrategy expected) =>
        Assert.Equal(expected, ProcessCardTxtParser.ParseStrategy(label));

    [Fact]
    public void LooksLikeVer02_FalseForLegacyKvCard()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1830330479_00_v2.txt");
        var text = File.ReadAllText(path);
        Assert.False(ProcessCardVer02Parser.LooksLikeVer02(text));
    }

    [Fact]
    public void Parse_Ver02Fixture_MapsPnSlotStagesAndLoosen()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1835154375_00_ver02.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var result = ProcessCardTxtParser.ParseFile(path);

        Assert.Equal("1835154375", result.ScrewPn);
        Assert.Equal(0, result.SlotId);
        Assert.Equal(1, result.Template.ParameterId);
        Assert.Equal(32767, result.Template.Core.MaxAngleDeg);
        Assert.Equal(3240, result.Template.Core.MinAngleDeg);
        Assert.Equal(120, result.Template.Core.TighteningStartDelayCentiSec);
        Assert.Equal(123, result.Template.Core.LoosenStartDelayCentiSec);
        Assert.Equal(21, result.Template.Core.FeederResultDelayTenthSec);
        Assert.True(result.Template.Core.FinalCurrentJudgeEnabled);

        var s0 = result.Template.Core.Stages[0];
        Assert.Equal(TighteningControlMode.Angle, s0.ControlMode);
        Assert.Equal(90, s0.TargetAngleDeg);
        Assert.Equal(80, s0.SpeedRpm);
        Assert.Equal(55, s0.AccelTimeMs);
        Assert.Equal(55, s0.DecelTimeMs);
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(2123, DefaultTorqueUnit.LbfIn),
            s0.MaxTorqueMilliNm);

        var s1 = result.Template.Core.Stages[1];
        Assert.Equal(TighteningControlMode.Angle, s1.ControlMode);
        Assert.Equal(1080, s1.TargetAngleDeg);
        Assert.Equal(110, s1.SpeedRpm);
        Assert.Equal(9600, s1.MaxAngleDeg);
        Assert.Equal(3240, s1.MinAngleDeg);
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(450, DefaultTorqueUnit.LbfIn),
            s1.MaxTorqueMilliNm);

        var s2 = result.Template.Core.Stages[2];
        Assert.Equal(110, s2.SpeedRpm);
        Assert.Equal(9600, s2.MaxAngleDeg);

        var s3 = result.Template.Core.Stages[3];
        Assert.Equal(TighteningControlMode.Torque, s3.ControlMode);
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(2123, DefaultTorqueUnit.LbfIn),
            s3.TargetTorqueMilliNm);
        Assert.Equal(
            TorqueUnitConverter.DisplayToMilliNm(520, DefaultTorqueUnit.LbfIn),
            s3.Segment1TorqueMilliNm);
        Assert.Equal(0, s3.TargetAngleDeg); // 第二行角度 0 不覆盖；扭矩模式目标角为 0

        Assert.Equal(7200, result.Template.Core.Loosen.Stage1AngleDeg);
        Assert.Equal(300, result.Template.Core.Loosen.Stage1SpeedRpm);
        Assert.Equal(0, result.Template.Core.Loosen.Stage2AngleDeg);
        Assert.Equal(0, result.Template.Core.Loosen.Stage2SpeedRpm);
    }

    [Fact]
    public void Parse_Ver02_1109BlankTemplate_UsesFilenameSlot()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "1835154375_1109_00.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var result = ProcessCardTxtParser.ParseFile(path);
        Assert.Equal("1835154375", result.ScrewPn);
        Assert.Equal(0, result.SlotId);
        Assert.Equal(32767, result.Template.Core.MaxAngleDeg);
        Assert.Equal(90, result.Template.Core.Stages[0].TargetAngleDeg);
        Assert.Equal(7200, result.Template.Core.Loosen.Stage1AngleDeg);
    }

    [Fact]
    public void Parse_Ver02_StrategyPipeLine_MapsEnhanced()
    {
        var text = """
            PN【1835154375】
            拧紧参数后缀【00】
            策略｜【加强】                       标准/加强/预定位/自创/
            　｜　　｜最大总角度｜【100】   Max Angle Tightening
            拧紧设定｜拧紧｜扭矩｜【2.5】   Target Torque
            拧紧设定｜拧紧｜速度｜【200】   Target Speed
            """;

        var result = ProcessCardTxtParser.Parse(text);
        Assert.Equal(TighteningStrategy.Enhanced, result.Template.Core.Strategy);
    }

    [Fact]
    public void Parse_Ver02_FinalCurrentJudge_FromTailNote()
    {
        var text = """
            PN【1835154375】
            拧紧参数后缀【00】
            　｜　　｜进阶设定｜【1】   最终电流判定 | F2 | 1=open, 0=close
            　｜　　｜最大总角度｜【100】   Max Angle Tightening
            拧紧设定｜启动｜速度｜【80】   Target Speed
            拧紧设定｜启动｜角度｜【90】   Target Angle
            """;

        var result = ProcessCardTxtParser.Parse(text);
        Assert.True(result.Template.Core.FinalCurrentJudgeEnabled);
    }

    [Fact]
    public void Parse_Ver02_EmptySuffixWithoutFilename_Throws()
    {
        var text = """
            PN【1835154375】
            拧紧参数后缀【     】
            　｜　　｜最大总角度｜【100】   Max Angle Tightening
            拧紧设定｜启动｜速度｜【80】   Target Speed
            拧紧设定｜启动｜角度｜【90】   Target Angle
            """;

        var ex = Assert.Throws<InvalidDataException>(() => ProcessCardTxtParser.Parse(text));
        Assert.Contains("拧紧参数后缀", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Parse_Ver02_EmptySuffix_UsesFilenameSlot()
    {
        var dir = Path.Combine(Path.GetTempPath(), "autoscrew-ver02-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var path = Path.Combine(dir, "1835154375_10.txt");
            File.WriteAllText(path, """
                PN【1835154375】
                拧紧参数后缀【     】
                　｜　　｜最大总角度｜【100】   Max Angle Tightening
                拧紧设定｜启动｜速度｜【80】   Target Speed
                拧紧设定｜启动｜角度｜【90】   Target Angle
                """);

            var result = ProcessCardTxtParser.ParseFile(path);
            Assert.Equal(10, result.SlotId);
            Assert.Equal(11, result.Template.ParameterId);
            Assert.Equal("1835154375", result.ScrewPn);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}
