using System.Globalization;
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
        var orderText = Cell(ws, row, headerMap, "拧紧顺序");
        if (!int.TryParse(orderText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var order) || order < 1)
            throw new InvalidDataException($"拧紧顺序无效：{orderText}");

        var location = Cell(ws, row, headerMap, "位置");
        var screwPnCol = ProcessParameterCode.SanitizeAscii(Cell(ws, row, headerMap, "螺钉PN"));
        var parameterCode = Cell(ws, row, headerMap, "拧紧参数");
        var (screwFromCode, slotId) = ProcessParameterCode.Parse(parameterCode);

        if (!string.IsNullOrEmpty(screwPnCol)
            && !string.Equals(screwPnCol, screwFromCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                $"螺钉PN「{screwPnCol}」与拧紧参数前缀「{screwFromCode}」不一致。");
        }

        var qtyText = Cell(ws, row, headerMap, "数量");
        if (!int.TryParse(qtyText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity)
            || quantity is < 1 or > 999_999)
            throw new InvalidDataException($"数量无效（1–999999）：{qtyText}");

        var bitText = Cell(ws, row, headerMap, "批头");
        if (!int.TryParse(bitText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bitId)
            || bitId is < 0 or > 255)
            throw new InvalidDataException($"批头无效（0–255）：{bitText}");

        string? remark = null;
        if (headerMap.TryGetValue("备注", out var remarkCol)
            || headerMap.Keys.Any(k => k.StartsWith("备注", StringComparison.Ordinal)))
        {
            var remarkKey = headerMap.ContainsKey("备注")
                ? "备注"
                : headerMap.Keys.First(k => k.StartsWith("备注", StringComparison.Ordinal));
            var remarkText = Cell(ws, row, headerMap, remarkKey);
            if (!string.IsNullOrWhiteSpace(remarkText))
                remark = remarkText.Trim();
        }

        return new SequenceExcelStepRow(
            ExcelRowNumber: row,
            Order: order,
            Location: location,
            ScrewPn: string.IsNullOrEmpty(screwPnCol) ? screwFromCode : screwPnCol,
            ParameterCode: parameterCode.Trim(),
            ScrewPnFromCode: screwFromCode,
            SlotId: slotId,
            Quantity: quantity,
            BitId: bitId,
            Remark: remark);
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
