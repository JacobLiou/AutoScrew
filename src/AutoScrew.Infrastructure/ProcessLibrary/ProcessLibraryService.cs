using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

public sealed class ProcessLibraryService : IProcessLibraryService
{
    private readonly ProcessLibraryStore _store;
    private readonly IControllerParameterPresetService _parameters;
    private readonly ILogger<ProcessLibraryService> _logger;

    public ProcessLibraryService(
        ProcessLibraryStore store,
        IControllerParameterPresetService parameters,
        ILogger<ProcessLibraryService> logger)
    {
        _store = store;
        _parameters = parameters;
        _logger = logger;
    }

    public string ProcessRootPath => _store.ResolveProcessRoot();

    public bool IsDeviceAvailable => _parameters.IsDeviceAvailable;

    public Task<IReadOnlyList<string>> ListProductPnsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_store.ListProductPns());
    }

    public Task<ProcessLibraryProductSummary?> GetProductAsync(
        string productPn,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_store.LoadProduct(productPn));
    }

    public ProcessCardParseResult ParseProcessCardText(string text) =>
        ProcessCardTxtParser.Parse(text);

    public ProcessCardParseResult ParseProcessCardFile(string filePath) =>
        ProcessCardTxtParser.ParseFile(filePath);

    public Task<ProcessCardParseResult> LoadProductSlotAsync(
        string productPn,
        int slotId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(productPn))
            throw new ArgumentException("产品 PN 不能为空。", nameof(productPn));

        var product = _store.LoadProduct(productPn)
            ?? throw new DirectoryNotFoundException($"未找到产品工艺库：{productPn}");

        var slot = product.Slots.FirstOrDefault(s => s.SlotId == slotId)
            ?? throw new InvalidOperationException($"产品 {productPn} 下没有槽位 {slotId:D2}。");

        var path = _store.ResolveSlotFilePath(productPn, slot);
        if (!File.Exists(path))
            throw new FileNotFoundException($"槽位 {slotId:D2} 文件不存在：{path}", path);

        return Task.FromResult(ProcessCardTxtParser.ParseFile(path));
    }

    public async Task<ProcessLibrarySlotInfo> UploadProcessCardAsync(
        string productPn,
        string sourceFilePath,
        CancellationToken cancellationToken = default)
    {
        var parsed = ProcessCardTxtParser.ParseFile(sourceFilePath);
        return await _store.SaveProcessCardAsync(productPn, sourceFilePath, parsed, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task RemoveSlotAsync(string productPn, int slotId, CancellationToken cancellationToken = default) =>
        _store.RemoveSlotAsync(productPn, slotId, cancellationToken);

    public async Task DeployTemplateToDeviceAsync(
        TighteningParameterTemplate template,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        await _parameters.WriteToDeviceAsync(template, cancellationToken).ConfigureAwait(false);
        await _parameters.SaveLocalPresetAsync(template, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ProcessLibraryDeployResult> DeployProductToDeviceAsync(
        string productPn,
        CancellationToken cancellationToken = default)
    {
        var product = _store.LoadProduct(productPn)
            ?? throw new DirectoryNotFoundException($"未找到产品工艺库：{productPn}");

        if (product.Slots.Count == 0)
            throw new InvalidOperationException($"产品 {productPn} 下没有工艺卡。");

        if (!_parameters.IsDeviceAvailable)
            throw new InvalidOperationException("控制器未连接，无法下发工艺参数。");

        var written = new List<int>();
        var failures = new List<ProcessLibraryDeployFailure>();

        foreach (var slot in product.Slots.OrderBy(s => s.SlotId))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var path = _store.ResolveSlotFilePath(productPn, slot);
                if (!File.Exists(path))
                    throw new FileNotFoundException($"槽位 {slot.SlotId:D2} 文件不存在。", path);

                var parsed = ProcessCardTxtParser.ParseFile(path);
                if (parsed.SlotId != slot.SlotId)
                {
                    _logger.LogWarning(
                        "Slot mismatch product={Product} manifest={ManifestSlot} file={FileSlot}",
                        productPn, slot.SlotId, parsed.SlotId);
                }

                await DeployTemplateToDeviceAsync(parsed.Template, cancellationToken).ConfigureAwait(false);
                written.Add(parsed.Template.ParameterId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Deploy slot {Slot} failed for {Product}", slot.SlotId, productPn);
                failures.Add(new ProcessLibraryDeployFailure(slot.SlotId, ex.Message));
                // 遇错停止，避免半套参数
                break;
            }
        }

        return new ProcessLibraryDeployResult(product.ProductPn, written, failures);
    }
}
