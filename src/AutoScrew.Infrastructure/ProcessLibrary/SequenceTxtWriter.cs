using System.Globalization;
using System.Text;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>拧紧顺序步骤 → 业务 Ver01 表格式 TXT（可再被 <see cref="SequenceTxtParser"/> 解析）。</summary>
public static class SequenceTxtWriter
{
    public static string Format(
        IReadOnlyList<SequenceExcelStepRow> steps,
        string? productPn = null,
        string? author = null,
        DateTimeOffset? exportedAt = null,
        string? changeContent = null)
    {
        ArgumentNullException.ThrowIfNull(steps);
        if (steps.Count == 0)
            throw new InvalidDataException("没有可导出的顺序步骤。");

        var pn = ProcessParameterCode.SanitizeAscii(productPn ?? string.Empty);
        var who = string.IsNullOrWhiteSpace(author) ? "未知" : author.Trim();
        var content = string.IsNullOrWhiteSpace(changeContent)
            ? TemplateExportHeader.DefaultChangeContent
            : changeContent.Trim();
        var sb = new StringBuilder();
        sb.AppendLine($"产品PN【{pn}】");
        sb.AppendLine("版本号【00】");
        sb.AppendLine($"做成【{who}】");
        sb.AppendLine($"变更内容【{content}】");
        sb.AppendLine($"变更时间：{TemplateExportHeader.FormatChangeTime(exportedAt)}");
        sb.AppendLine();
        sb.AppendLine("拧紧参数后缀：20=紧固程序；10=预紧程序；00=正常锁附程序");
        sb.AppendLine();
        sb.AppendLine("顺序｜位置｜螺钉PN｜【拧紧参数】数量｜批头｜吸头｜备注｜结果");

        foreach (var step in steps.OrderBy(s => s.Order).ThenBy(s => s.ExcelRowNumber))
        {
            SequenceRemarkMeta.TrySplit(step.Remark, out var body, out var nozzle, out var result);
            sb.Append(CultureInfo.InvariantCulture, $"{step.Order}");
            sb.Append('｜');
            sb.Append(step.Location);
            sb.Append('｜');
            sb.Append(step.ScrewPn);
            sb.Append('｜');
            sb.Append(CultureInfo.InvariantCulture, $"【{step.ParameterCode}】{step.Quantity}");
            sb.Append('｜');
            sb.Append(CultureInfo.InvariantCulture, $"{step.BitId}");
            sb.Append('｜');
            sb.Append(nozzle);
            sb.Append('｜');
            sb.Append(body);
            sb.Append('｜');
            sb.Append(result);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static void WriteFile(
        string filePath,
        IReadOnlyList<SequenceExcelStepRow> steps,
        string? productPn = null,
        string? author = null,
        DateTimeOffset? exportedAt = null,
        string? changeContent = null)
    {
        File.WriteAllText(filePath, Format(steps, productPn, author, exportedAt, changeContent), Encoding.UTF8);
    }
}
