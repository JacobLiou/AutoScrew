using System.Globalization;
using System.Text;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>拧紧顺序步骤 → TXT 键值块（可再被 <see cref="SequenceTxtParser"/> 解析）。</summary>
public static class SequenceTxtWriter
{
    public static string Format(IReadOnlyList<SequenceExcelStepRow> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        if (steps.Count == 0)
            throw new InvalidDataException("没有可导出的顺序步骤。");

        var sb = new StringBuilder();
        sb.AppendLine("# 拧紧顺序 TXT。顺序 ID 在工艺库页填写。位置、备注不进设备。");
        sb.AppendLine("# 拧紧参数 = 已上传工艺卡的「螺钉PN-槽位」。");
        sb.AppendLine();

        foreach (var step in steps)
        {
            sb.AppendLine("步骤");
            sb.AppendLine(CultureInfo.InvariantCulture, $"拧紧顺序：{step.Order}");
            sb.AppendLine($"位置：{step.Location}");
            sb.AppendLine($"螺钉PN：{step.ScrewPn}");
            sb.AppendLine($"拧紧参数：{step.ParameterCode}");
            sb.AppendLine(CultureInfo.InvariantCulture, $"数量：{step.Quantity}");
            sb.AppendLine(CultureInfo.InvariantCulture, $"批头：{step.BitId}");
            if (!string.IsNullOrWhiteSpace(step.Remark))
                sb.AppendLine($"备注：{step.Remark}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static void WriteFile(string filePath, IReadOnlyList<SequenceExcelStepRow> steps)
    {
        File.WriteAllText(filePath, Format(steps), Encoding.UTF8);
    }
}
