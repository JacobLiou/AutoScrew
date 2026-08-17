using ClosedXML.Excel;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>拧紧顺序步骤 → Excel（表头对齐 <see cref="SequenceExcelParser"/>）。</summary>
public static class SequenceExcelWriter
{
    public static readonly string[] Headers =
    [
        "拧紧顺序",
        "位置",
        "螺钉PN",
        "拧紧参数",
        "数量",
        "批头",
        "备注",
    ];

    public static void Write(Stream stream, IReadOnlyList<SequenceExcelStepRow> steps)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(steps);
        if (steps.Count == 0)
            throw new InvalidDataException("没有可导出的顺序步骤。");

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Sequence");

        for (var c = 0; c < Headers.Length; c++)
            ws.Cell(1, c + 1).Value = Headers[c];

        for (var i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            var row = i + 2;
            ws.Cell(row, 1).Value = step.Order;
            ws.Cell(row, 2).Value = step.Location ?? string.Empty;
            ws.Cell(row, 3).Value = step.ScrewPn;
            ws.Cell(row, 4).Value = step.ParameterCode;
            ws.Cell(row, 5).Value = step.Quantity;
            ws.Cell(row, 6).Value = step.BitId;
            ws.Cell(row, 7).Value = step.Remark ?? string.Empty;
        }

        ws.Columns().AdjustToContents();
        workbook.SaveAs(stream);
    }

    public static void WriteFile(string filePath, IReadOnlyList<SequenceExcelStepRow> steps)
    {
        using var stream = File.Create(filePath);
        Write(stream, steps);
    }
}
