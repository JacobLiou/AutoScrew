using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Abstractions;

public sealed record ControllerSequencePresetSummary(
    int SequenceId,
    string Name,
    int StepCount,
    int BitId = 0,
    string? SourceProductPn = null,
    int? SourceSequenceId = null);

/// <summary>设备已配置顺序摘要（含 Name，仅用于列表展示）。</summary>
public sealed record ControllerDeviceSequenceEntry(int SequenceId, string Name);

public interface IControllerSequencePresetService
{
    bool IsDeviceAvailable { get; }

    Task<IReadOnlyList<ControllerSequencePresetSummary>> ListLocalPresetsAsync(CancellationToken cancellationToken = default);

    Task<TighteningSequencePackage> LoadLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task SaveLocalPresetAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default);

    Task SaveLocalPresetWithOriginAsync(
        TighteningSequencePackage package,
        string sourceProductPn,
        int sourceSequenceId,
        CancellationToken cancellationToken = default);

    Task DeleteLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task<TighteningSequencePackage> ImportFromFileAsync(string filePath, CancellationToken cancellationToken = default);

    Task ExportToFileAsync(TighteningSequencePackage package, string filePath, CancellationToken cancellationToken = default);

    Task<TighteningSequencePackage> ReadFromDeviceAsync(int sequenceId, CancellationToken cancellationToken = default);

    /// <summary>Lists configured sequence IDs from device (#260).</summary>
    Task<IReadOnlyList<int>> ListDeviceSequenceIdsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 列设备已配置顺序 ID，再并发读取 Name（并发度 2）。
    /// 单条读失败时该条 Name 为空，仍返回；不写入本地预设。
    /// </summary>
    Task<IReadOnlyList<ControllerDeviceSequenceEntry>> ListDeviceSequenceEntriesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Reads sequence from device (#250+) and saves to local store.</summary>
    Task<TighteningSequencePackage> ImportFromDeviceAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task WriteToDeviceAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default);

    /// <summary>删除设备上的拧紧顺序（Modbus #210）。</summary>
    Task DeleteFromDeviceAsync(int sequenceId, CancellationToken cancellationToken = default);

    Task ActivateOnDeviceAsync(int sequenceId, CancellationToken cancellationToken = default);
}
