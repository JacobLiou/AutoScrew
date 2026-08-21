using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Abstractions;

public sealed record ControllerParameterPresetSummary(
    int ParameterId,
    string Name,
    int ToolIndex,
    string? SourceProductPn = null,
    int? SourceSlotId = null);

public sealed record ControllerParameterBulkImportResult(
    IReadOnlyList<int> ImportedIds,
    IReadOnlyList<ControllerParameterImportFailure> Failures);

public sealed record ControllerParameterImportFailure(int ParameterId, string Message);

/// <summary>设备已配置参数摘要（含 Name，仅用于列表展示）。</summary>
public sealed record ControllerDeviceParameterEntry(int ParameterId, string Name);

public interface IControllerParameterPresetService
{
    bool IsDeviceAvailable { get; }

    Task<IReadOnlyList<ControllerParameterPresetSummary>> ListLocalPresetsAsync(CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> LoadLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default);

    Task SaveLocalPresetAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default);

    Task SaveLocalPresetWithOriginAsync(
        TighteningParameterTemplate template,
        string sourceProductPn,
        int sourceSlotId,
        CancellationToken cancellationToken = default);

    Task DeleteLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ImportFromFileAsync(string filePath, CancellationToken cancellationToken = default);

    Task ExportToFileAsync(TighteningParameterTemplate template, string filePath, CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ReadFromDeviceAsync(int parameterId, CancellationToken cancellationToken = default);

    /// <summary>#555 读控制器默认扭矩单位；离线时由调用方默认 kgf.cm。</summary>
    Task<DefaultTorqueUnit> ReadDefaultTorqueUnitAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> ListDeviceParameterIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 列设备已配置参数 ID，再并发读取 Name（并发度 2）。
    /// 单条读失败时该条 Name 为空，仍返回；不写入本地预设。
    /// </summary>
    Task<IReadOnlyList<ControllerDeviceParameterEntry>> ListDeviceParameterEntriesAsync(
        CancellationToken cancellationToken = default);

    Task<TighteningParameterTemplate> ImportFromDeviceAsync(int parameterId, CancellationToken cancellationToken = default);

    Task<ControllerParameterBulkImportResult> ImportAllFromDeviceAsync(CancellationToken cancellationToken = default);

    Task WriteToDeviceAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default);

    /// <summary>删除设备上的参数（Modbus #110）。</summary>
    Task DeleteFromDeviceAsync(int parameterId, CancellationToken cancellationToken = default);

    Task ActivateOnDeviceAsync(int parameterId, uint screwCount = 1, CancellationToken cancellationToken = default);
}
