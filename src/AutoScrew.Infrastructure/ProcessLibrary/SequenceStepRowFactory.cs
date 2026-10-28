using System.Globalization;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure.ProcessLibrary;

/// <summary>拧紧顺序一步的字段校验。Excel 与 TXT 共用，报错文案保持一致。</summary>
public static class SequenceStepRowFactory
{
    public static SequenceExcelStepRow Create(
        int sourceRow,
        string? orderText,
        string? location,
        string? screwPn,
        string? parameterCode,
        string? quantityText,
        string? bitText,
        string? remark)
    {
        if (!int.TryParse(orderText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var order) || order < 1)
            throw new InvalidDataException($"拧紧顺序无效：{orderText}");

        var screwPnCol = ProcessParameterCode.SanitizeAscii(screwPn ?? string.Empty);
        var code = parameterCode?.Trim() ?? string.Empty;
        var (screwFromCode, slotId) = ProcessParameterCode.Parse(code);

        if (!string.IsNullOrEmpty(screwPnCol)
            && !string.Equals(screwPnCol, screwFromCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                $"螺钉PN「{screwPnCol}」与拧紧参数前缀「{screwFromCode}」不一致。");
        }

        if (!int.TryParse(quantityText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity)
            || quantity is < 1 or > 999_999)
            throw new InvalidDataException($"数量无效（1–999999）：{quantityText}");

        if (!int.TryParse(bitText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bitId)
            || bitId is < 0 or > 255)
            throw new InvalidDataException($"批头无效（0–255）：{bitText}");

        string? remarkOut = string.IsNullOrWhiteSpace(remark) ? null : remark.Trim();

        return new SequenceExcelStepRow(
            ExcelRowNumber: sourceRow,
            Order: order,
            Location: location?.Trim() ?? string.Empty,
            ScrewPn: string.IsNullOrEmpty(screwPnCol) ? screwFromCode : screwPnCol,
            ParameterCode: code,
            ScrewPnFromCode: screwFromCode,
            SlotId: slotId,
            Quantity: quantity,
            BitId: bitId,
            Remark: remarkOut);
    }
}
