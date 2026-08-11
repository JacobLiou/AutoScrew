namespace AutoScrew.Application.Abstractions;

public sealed record SequenceExcelStepRow(
    int ExcelRowNumber,
    int Order,
    string Location,
    string ScrewPn,
    string ParameterCode,
    string ScrewPnFromCode,
    int SlotId,
    int Quantity,
    int BitId,
    string? Remark);

public sealed record SequenceExcelParseResult(
    IReadOnlyList<SequenceExcelStepRow> Steps,
    IReadOnlyList<string> Errors)
{
    public bool IsSuccess => Errors.Count == 0 && Steps.Count > 0;
}
