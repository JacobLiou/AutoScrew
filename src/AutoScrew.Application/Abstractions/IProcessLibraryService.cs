using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Application.Abstractions;

public sealed record ProcessLibrarySlotInfo(
    int SlotId,
    string ScrewPn,
    string FileName,
    string DisplayName,
    int DeviceParameterId,
    bool WasUpdate = false);

public sealed record ProcessLibrarySequenceInfo(
    int SequenceId,
    string FileName,
    string DisplayName);

public sealed record ProcessLibraryProductSummary(
    string ProductPn,
    DateTimeOffset? UpdatedUtc,
    IReadOnlyList<ProcessLibrarySlotInfo> Slots,
    IReadOnlyList<ProcessLibrarySequenceInfo> Sequences);

public sealed record ProcessCardParseResult(
    TighteningParameterTemplate Template,
    string ScrewPn,
    int SlotId);

public sealed record ProcessLibraryDeployResult(
    string ProductPn,
    IReadOnlyList<int> WrittenSlotIds,
    IReadOnlyList<ProcessLibraryDeployFailure> Failures);

public sealed record ProcessLibraryDeployFailure(int SlotId, string Message);

public sealed record ProcessLibrarySequenceDeployResult(
    string ProductPn,
    IReadOnlyList<int> WrittenSequenceIds,
    IReadOnlyList<ProcessLibrarySequenceDeployFailure> Failures);

public sealed record ProcessLibrarySequenceDeployFailure(int SequenceId, string Message);

/// <summary>按产品 PN 管理工艺卡（参数 TXT / 顺序 JSON）并下发到设备。</summary>
public interface IProcessLibraryService
{
    /// <summary>当前工艺库根路径（局域网优先，否则本机 DataDirectory/process）。</summary>
    string ProcessRootPath { get; }

    bool IsDeviceAvailable { get; }

    Task<IReadOnlyList<string>> ListProductPnsAsync(CancellationToken cancellationToken = default);

    Task<ProcessLibraryProductSummary?> GetProductAsync(string productPn, CancellationToken cancellationToken = default);

    /// <summary>解析工艺卡 TXT（不落盘）。</summary>
    ProcessCardParseResult ParseProcessCardText(string text);

    ProcessCardParseResult ParseProcessCardFile(string filePath);

    /// <summary>按产品 PN + 槽位读取并解析工艺卡。</summary>
    Task<ProcessCardParseResult> LoadProductSlotAsync(
        string productPn,
        int slotId,
        CancellationToken cancellationToken = default);

    /// <summary>上传 TXT 到产品目录并更新 product.json。</summary>
    Task<ProcessLibrarySlotInfo> UploadProcessCardAsync(
        string productPn,
        string sourceFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>删除产品下某一槽位工艺卡。</summary>
    Task RemoveSlotAsync(string productPn, int slotId, CancellationToken cancellationToken = default);

    /// <summary>按产品 PN 将全部参数槽写入设备，并回写本机参数预设。</summary>
    Task<ProcessLibraryDeployResult> DeployProductToDeviceAsync(
        string productPn,
        CancellationToken cancellationToken = default);

    /// <summary>将单张工艺卡解析后写入设备与本机预设。</summary>
    Task DeployTemplateToDeviceAsync(TighteningParameterTemplate template, CancellationToken cancellationToken = default);

    /// <summary>上传顺序 JSON 到产品目录并更新 product.json。</summary>
    Task<ProcessLibrarySequenceInfo> UploadSequenceAsync(
        string productPn,
        string sourceFilePath,
        CancellationToken cancellationToken = default);

    /// <summary>解析拧紧顺序 Excel（第 1 行表头；不落盘、不校验工艺库槽位）。</summary>
    SequenceExcelParseResult ParseSequenceExcelFile(string filePath);

    /// <summary>
    /// 解析 Excel → 按「螺钉PN-槽位」映射 ParameterId，校验产品工艺库已有对应槽位后写入 sequences/{id}.json。
    /// </summary>
    Task<ProcessLibrarySequenceInfo> UploadSequenceExcelAsync(
        string productPn,
        string sourceFilePath,
        int sequenceId,
        CancellationToken cancellationToken = default);

    /// <summary>按产品 PN + 顺序 ID 读取顺序包。</summary>
    Task<TighteningSequencePackage> LoadProductSequenceAsync(
        string productPn,
        int sequenceId,
        CancellationToken cancellationToken = default);

    /// <summary>删除产品下某一顺序。</summary>
    Task RemoveSequenceAsync(string productPn, int sequenceId, CancellationToken cancellationToken = default);

    /// <summary>按产品 PN 将全部顺序覆盖写入设备，并回写本机顺序预设。</summary>
    Task<ProcessLibrarySequenceDeployResult> DeployProductSequencesToDeviceAsync(
        string productPn,
        CancellationToken cancellationToken = default);

    /// <summary>将单条顺序写入设备与本机预设。</summary>
    Task DeploySequenceToDeviceAsync(TighteningSequencePackage package, CancellationToken cancellationToken = default);
}
