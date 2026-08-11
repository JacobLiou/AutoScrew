using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Infrastructure.ProcessLibrary;

public sealed class ProcessLibraryService : IProcessLibraryService
{
    private readonly ProcessLibraryStore _store;
    private readonly IControllerParameterPresetService _parameters;
    private readonly IControllerSequencePresetService _sequences;
    private readonly ILogger<ProcessLibraryService> _logger;

    public ProcessLibraryService(
        ProcessLibraryStore store,
        IControllerParameterPresetService parameters,
        IControllerSequencePresetService sequences,
        ILogger<ProcessLibraryService> logger)
    {
        _store = store;
        _parameters = parameters;
        _sequences = sequences;
        _logger = logger;
    }

    public string ProcessRootPath => _store.ResolveProcessRoot();

    public bool IsDeviceAvailable => _parameters.IsDeviceAvailable || _sequences.IsDeviceAvailable;

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
                break;
            }
        }

        return new ProcessLibraryDeployResult(product.ProductPn, written, failures);
    }

    public async Task<ProcessLibrarySequenceInfo> UploadSequenceAsync(
        string productPn,
        string sourceFilePath,
        CancellationToken cancellationToken = default)
    {
        var package = await _sequences.ImportFromFileAsync(sourceFilePath, cancellationToken).ConfigureAwait(false);
        return await _store.SaveSequenceAsync(productPn, package, cancellationToken).ConfigureAwait(false);
    }

    public SequenceExcelParseResult ParseSequenceExcelFile(string filePath) =>
        SequenceExcelParser.ParseFile(filePath);

    public async Task<ProcessLibrarySequenceInfo> UploadSequenceExcelAsync(
        string productPn,
        string sourceFilePath,
        int sequenceId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productPn))
            throw new ArgumentException("产品 PN 不能为空。", nameof(productPn));
        if (sequenceId is < 1 or > 500)
            throw new ArgumentOutOfRangeException(nameof(sequenceId), sequenceId, "顺序 ID 须为 1–500。");
        if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
            throw new FileNotFoundException("顺序 Excel 文件不存在。", sourceFilePath);

        cancellationToken.ThrowIfCancellationRequested();

        var parsed = SequenceExcelParser.ParseFile(sourceFilePath);
        if (!parsed.IsSuccess)
            throw new InvalidDataException(string.Join(Environment.NewLine, parsed.Errors));

        var product = _store.LoadProduct(productPn)
            ?? throw new DirectoryNotFoundException(
                $"未找到产品工艺库：{productPn}。请先上传拧紧参数工艺卡。");

        if (product.Slots.Count == 0)
            throw new InvalidOperationException($"产品 {productPn} 下没有工艺卡，无法关联拧紧顺序。");

        var slotLookup = product.Slots
            .GroupBy(s => s.SlotId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var missing = new List<string>();
        var steps = new List<TighteningSequenceStepCore>();

        foreach (var row in parsed.Steps)
        {
            if (!slotLookup.TryGetValue(row.SlotId, out var candidates))
            {
                missing.Add($"第 {row.ExcelRowNumber} 行：槽位 {row.SlotId:D2}（{row.ParameterCode}）不在工艺库中。");
                continue;
            }

            var match = candidates.FirstOrDefault(s =>
                string.Equals(s.ScrewPn, row.ScrewPnFromCode, StringComparison.OrdinalIgnoreCase));
            if (match is null)
            {
                missing.Add(
                    $"第 {row.ExcelRowNumber} 行：槽位 {row.SlotId:D2} 存在，但螺钉 PN「{row.ScrewPnFromCode}」与工艺库不匹配。");
                continue;
            }

            steps.Add(new TighteningSequenceStepCore
            {
                ToolId = 0,
                ParameterId = match.SlotId,
                Quantity = row.Quantity,
                BitId = row.BitId,
            });
        }

        if (missing.Count > 0)
            throw new InvalidDataException(string.Join(Environment.NewLine, missing));

        var displayName = Path.GetFileNameWithoutExtension(sourceFilePath);
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = $"{productPn.Trim()}-顺序{sequenceId}";

        var package = new TighteningSequencePackage
        {
            SequenceId = sequenceId,
            Core = new TighteningSequenceCore
            {
                Name = displayName.Trim(),
                NavigatorMode = TighteningSequenceNavigatorMode.General,
                PositioningArmEnabled = false,
                Steps = steps,
            },
        };
        package.ApplyCoreToRaw();

        _logger.LogInformation(
            "Sequence Excel imported product={Product} sequenceId={SequenceId} steps={Steps}",
            productPn, sequenceId, steps.Count);

        return await _store.SaveSequenceAsync(productPn, package, cancellationToken).ConfigureAwait(false);
    }

    public Task<TighteningSequencePackage> LoadProductSequenceAsync(
        string productPn,
        int sequenceId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(productPn))
            throw new ArgumentException("产品 PN 不能为空。", nameof(productPn));

        var product = _store.LoadProduct(productPn)
            ?? throw new DirectoryNotFoundException($"未找到产品工艺库：{productPn}");

        var seq = product.Sequences.FirstOrDefault(s => s.SequenceId == sequenceId)
            ?? throw new InvalidOperationException($"产品 {productPn} 下没有顺序 {sequenceId:D2}。");

        var path = _store.ResolveSequenceFilePath(productPn, seq);
        if (!File.Exists(path))
            throw new FileNotFoundException($"顺序 {sequenceId:D2} 文件不存在：{path}", path);

        return Task.FromResult(_store.LoadSequencePackage(path));
    }

    public Task RemoveSequenceAsync(string productPn, int sequenceId, CancellationToken cancellationToken = default) =>
        _store.RemoveSequenceAsync(productPn, sequenceId, cancellationToken);

    public async Task DeploySequenceToDeviceAsync(
        TighteningSequencePackage package,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(package);
        await _sequences.WriteToDeviceAsync(package, cancellationToken).ConfigureAwait(false);
        await _sequences.SaveLocalPresetAsync(package, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ProcessLibrarySequenceDeployResult> DeployProductSequencesToDeviceAsync(
        string productPn,
        CancellationToken cancellationToken = default)
    {
        var product = _store.LoadProduct(productPn)
            ?? throw new DirectoryNotFoundException($"未找到产品工艺库：{productPn}");

        if (product.Sequences.Count == 0)
            throw new InvalidOperationException($"产品 {productPn} 下没有拧紧顺序。");

        if (!_sequences.IsDeviceAvailable)
            throw new InvalidOperationException("控制器未连接，无法下发拧紧顺序。");

        var written = new List<int>();
        var failures = new List<ProcessLibrarySequenceDeployFailure>();

        foreach (var seq in product.Sequences.OrderBy(s => s.SequenceId))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var path = _store.ResolveSequenceFilePath(productPn, seq);
                if (!File.Exists(path))
                    throw new FileNotFoundException($"顺序 {seq.SequenceId:D2} 文件不存在。", path);

                var package = _store.LoadSequencePackage(path);
                await DeploySequenceToDeviceAsync(package, cancellationToken).ConfigureAwait(false);
                written.Add(package.SequenceId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Deploy sequence {SequenceId} failed for {Product}", seq.SequenceId, productPn);
                failures.Add(new ProcessLibrarySequenceDeployFailure(seq.SequenceId, ex.Message));
                break;
            }
        }

        return new ProcessLibrarySequenceDeployResult(product.ProductPn, written, failures);
    }
}
