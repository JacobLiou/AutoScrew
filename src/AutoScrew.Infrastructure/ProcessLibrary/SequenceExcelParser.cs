using AutoScrew.Application.Abstractions;
using ClosedXML.Excel;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>拧紧顺序 Excel：第 1 行表头，其后为数据行。</summary>
public static class SequenceExcelParser
{
    private static readonly string[] RequiredHeaders =
    [
        "拧紧顺序",
        "位置",
        "螺钉PN",
        "拧紧参数",
        "数量",
        "批头",
    ];

    public static SequenceExcelParseResult ParseFile(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Parse(stream);
    }

    public static SequenceExcelParseResult Parse(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        var errors = new List<string>();
        var steps = new List<SequenceExcelStepRow>();

        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheets.FirstOrDefault()
                 ?? throw new InvalidDataException("Excel 中没有工作表。");

        var lastCol = Math.Max(ws.LastColumnUsed()?.ColumnNumber() ?? 0, 1);
        var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var c = 1; c <= lastCol; c++)
        {
            var name = NormalizeHeader(ws.Cell(1, c).GetString());
            if (string.IsNullOrEmpty(name))
                continue;
            if (!headerMap.ContainsKey(name))
                headerMap[name] = c;
        }

        foreach (var required in RequiredHeaders)
        {
            if (!headerMap.ContainsKey(required))
                errors.Add($"缺少表头列「{required}」。");
        }

        if (errors.Count > 0)
            return new SequenceExcelParseResult([], errors);

        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
        for (var r = 2; r <= lastRow; r++)
        {
            if (IsRowEmpty(ws, r, headerMap))
                continue;

            try
            {
                steps.Add(ParseDataRow(ws, r, headerMap));
            }
            catch (Exception ex)
            {
                errors.Add($"第 {r} 行：{ex.Message}");
            }
        }

        if (errors.Count == 0 && steps.Count == 0)
            errors.Add("未找到有效数据行。");

        if (errors.Count > 0)
            return new SequenceExcelParseResult([], errors);

        var ordered = steps.OrderBy(s => s.Order).ThenBy(s => s.ExcelRowNumber).ToList();
        return new SequenceExcelParseResult(ordered, []);
    }

    private static SequenceExcelStepRow ParseDataRow(
        IXLWorksheet ws,
        int row,
        IReadOnlyDictionary<string, int> headerMap)
    {
        string? remark = null;
        if (headerMap.TryGetValue("备注", out _)
            || headerMap.Keys.Any(k => k.StartsWith("备注", StringComparison.Ordinal)))
        {
            var remarkKey = headerMap.ContainsKey("备注")
                ? "备注"
                : headerMap.Keys.First(k => k.StartsWith("备注", StringComparison.Ordinal));
            var remarkText = Cell(ws, row, headerMap, remarkKey);
            if (!string.IsNullOrWhiteSpace(remarkText))
                remark = remarkText.Trim();
        }

        return SequenceStepRowFactory.Create(
            row,
            Cell(ws, row, headerMap, "拧紧顺序"),
            Cell(ws, row, headerMap, "位置"),
            Cell(ws, row, headerMap, "螺钉PN"),
            Cell(ws, row, headerMap, "拧紧参数"),
            Cell(ws, row, headerMap, "数量"),
            Cell(ws, row, headerMap, "批头"),
            remark);
    }

    private static bool IsRowEmpty(IXLWorksheet ws, int row, IReadOnlyDictionary<string, int> headerMap)
    {
        foreach (var col in headerMap.Values)
        {
            if (!string.IsNullOrWhiteSpace(ws.Cell(row, col).GetFormattedString()))
                return false;
        }

        return true;
    }

    private static string Cell(
        IXLWorksheet ws,
        int row,
        IReadOnlyDictionary<string, int> headerMap,
        string header)
    {
        var col = headerMap[header];
        return ws.Cell(row, col).GetFormattedString().Trim();
    }

    private static string NormalizeHeader(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return string.Empty;
        var s = raw.Trim().Replace('\u3000', ' ');
        // 表头如「备注（可以不用填写）」→ 备注
        var paren = s.IndexOf('（');
        if (paren > 0)
            s = s[..paren].Trim();
        paren = s.IndexOf('(');
        if (paren > 0)
            s = s[..paren].Trim();
        return s;
    }
}
