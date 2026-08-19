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
        parsed.Template.ParameterId = ProcessParameterCode.ToDeviceParameterId(parsed.SlotId);
        if (string.IsNullOrWhiteSpace(parsed.Template.Core.Name))
            parsed.Template.Core.Name = parsed.ScrewPn;

        var slot = await _store.SaveProcessCardAsync(productPn, sourceFilePath, parsed, cancellationToken)
            .ConfigureAwait(false);

        await ImportSlotsToLocalAsync(productPn, [parsed.SlotId], cancellationToken).ConfigureAwait(false);
        _logger.LogInformation(
            "Process card synced to local parameter preset product={Product} slot={Slot} wasUpdate={WasUpdate}",
            productPn,
            parsed.SlotId,
            slot.WasUpdate);

        return slot;
    }

    public Task RemoveSlotAsync(string productPn, int slotId, CancellationToken cancellationToken = default) =>
        _store.RemoveSlotAsync(productPn, slotId, cancellationToken);

    public async Task<ProcessLibraryLocalImportResult> ImportSlotsToLocalAsync(
        string productPn,
        IReadOnlyList<int>? slotIds,
        CancellationToken cancellationToken = default)
    {
        var pn = RequireProductPn(productPn);
        var product = _store.LoadProduct(pn)
            ?? throw new DirectoryNotFoundException($"未找到产品工艺库：{pn}");

        var requested = slotIds is { Count: > 0 }
            ? slotIds.Distinct().ToList()
            : product.Slots.Select(s => s.SlotId).ToList();
        if (requested.Count == 0)
            throw new InvalidOperationException($"产品 {pn} 下没有工艺卡。");

        var origins = ParameterOrigins(await _parameters.ListLocalPresetsAsync(cancellationToken).ConfigureAwait(false));
        var items = new List<ProcessLibraryLocalImportItem>();

        foreach (var slotId in requested.OrderBy(id => id))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var parsed = await LoadProductSlotAsync(pn, slotId, cancellationToken).ConfigureAwait(false);
            var preferredId = ProcessParameterCode.ToDeviceParameterId(slotId);
            var existed = HasOrigin(origins, pn, slotId);
            int localId;
            try
            {
                localId = ProcessLibraryLocalIdAllocator.Resolve(origins, pn, slotId, preferredId);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    $"无法为产品 {pn} 槽位 {slotId:D2} 分配本机参数 ID：{ex.Message}",
                    ex);
            }

            parsed.Template.ParameterId = localId;
            if (string.IsNullOrWhiteSpace(parsed.Template.Core.Name))
                parsed.Template.Core.Name = parsed.ScrewPn;

            await _parameters
                .SaveLocalPresetWithOriginAsync(parsed.Template, pn, slotId, cancellationToken)
                .ConfigureAwait(false);

            UpsertOrigin(origins, new LocalPresetOrigin(localId, pn, slotId));
            items.Add(new ProcessLibraryLocalImportItem(slotId, preferredId, localId, !existed));
            _logger.LogInformation(
                "Imported process slot to local preset product={Product} slot={Slot} preferred={Preferred} local={Local} wasNew={WasNew}",
                pn, slotId, preferredId, localId, !existed);
        }

        return new ProcessLibraryLocalImportResult(pn, items);
    }

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

                parsed.Template.ParameterId = ProcessParameterCode.ToDeviceParameterId(slot.SlotId);
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
                ParameterId = match.DeviceParameterId > 0
                    ? match.DeviceParameterId
                    : ProcessParameterCode.ToDeviceParameterId(match.SlotId),
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

    public async Task<ProcessLibraryLocalImportResult> ImportSequencesToLocalAsync(
        string productPn,
        IReadOnlyList<int>? sequenceIds,
        CancellationToken cancellationToken = default)
    {
        var pn = RequireProductPn(productPn);
        var product = _store.LoadProduct(pn)
            ?? throw new DirectoryNotFoundException($"未找到产品工艺库：{pn}");

        var requested = sequenceIds is { Count: > 0 }
            ? sequenceIds.Distinct().ToList()
            : product.Sequences.Select(s => s.SequenceId).ToList();
        if (requested.Count == 0)
            throw new InvalidOperationException($"产品 {pn} 下没有拧紧顺序。");

        var packages = new List<(int LibrarySequenceId, TighteningSequencePackage Package)>();
        var referencedSlots = new HashSet<int>();
        foreach (var sequenceId in requested.OrderBy(id => id))
        {
            var pkg = await LoadProductSequenceAsync(pn, sequenceId, cancellationToken).ConfigureAwait(false);
            packages.Add((sequenceId, pkg));
            foreach (var step in pkg.Core.Steps)
            {
                if (step.ParameterId is >= ProcessParameterCode.MinDeviceParameterId
                    and <= ProcessParameterCode.MaxDeviceParameterId)
                    referencedSlots.Add(ProcessParameterCode.ToSlotIndex(step.ParameterId));
            }
        }

        if (referencedSlots.Count > 0)
        {
            var available = product.Slots.Select(s => s.SlotId).ToHashSet();
            var missing = referencedSlots.Where(s => !available.Contains(s)).ToList();
            if (missing.Count > 0)
            {
                throw new InvalidOperationException(
                    $"产品 {pn} 顺序引用了工艺库中不存在的槽位：{string.Join(", ", missing.Select(s => s.ToString("D2")))}。");
            }

            await ImportSlotsToLocalAsync(pn, referencedSlots.ToList(), cancellationToken).ConfigureAwait(false);
        }

        var paramOrigins = ParameterOrigins(
            await _parameters.ListLocalPresetsAsync(cancellationToken).ConfigureAwait(false));
        var seqOrigins = SequenceOrigins(
            await _sequences.ListLocalPresetsAsync(cancellationToken).ConfigureAwait(false));
        var items = new List<ProcessLibraryLocalImportItem>();

        foreach (var (librarySequenceId, package) in packages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var step in package.Core.Steps)
                step.ParameterId = RemapStepParameterId(paramOrigins, pn, step.ParameterId);

            var preferredId = librarySequenceId is >= ProcessLibraryLocalIdAllocator.MinId
                and <= ProcessLibraryLocalIdAllocator.MaxId
                ? librarySequenceId
                : ProcessLibraryLocalIdAllocator.MinId;
            var existed = HasOrigin(seqOrigins, pn, librarySequenceId);
            int localId;
            try
            {
                localId = ProcessLibraryLocalIdAllocator.Resolve(
                    seqOrigins, pn, librarySequenceId, preferredId);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    $"无法为产品 {pn} 顺序 {librarySequenceId:D2} 分配本机顺序 ID：{ex.Message}",
                    ex);
            }

            package.SequenceId = localId;
            package.ApplyCoreToRaw();
            await _sequences
                .SaveLocalPresetWithOriginAsync(package, pn, librarySequenceId, cancellationToken)
                .ConfigureAwait(false);

            UpsertOrigin(seqOrigins, new LocalPresetOrigin(localId, pn, librarySequenceId));
            items.Add(new ProcessLibraryLocalImportItem(librarySequenceId, preferredId, localId, !existed));
            _logger.LogInformation(
                "Imported process sequence to local preset product={Product} librarySeq={Library} preferred={Preferred} local={Local} wasNew={WasNew}",
                pn, librarySequenceId, preferredId, localId, !existed);
        }

        return new ProcessLibraryLocalImportResult(pn, items);
    }

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

    private static string RequireProductPn(string productPn)
    {
        if (string.IsNullOrWhiteSpace(productPn))
            throw new ArgumentException("产品 PN 不能为空。", nameof(productPn));
        return productPn.Trim();
    }

    private static List<LocalPresetOrigin> ParameterOrigins(
        IReadOnlyList<ControllerParameterPresetSummary> items) =>
        items.Select(p => new LocalPresetOrigin(p.ParameterId, p.SourceProductPn, p.SourceSlotId)).ToList();

    private static List<LocalPresetOrigin> SequenceOrigins(
        IReadOnlyList<ControllerSequencePresetSummary> items) =>
        items.Select(s => new LocalPresetOrigin(s.SequenceId, s.SourceProductPn, s.SourceSequenceId)).ToList();

    private static bool HasOrigin(IReadOnlyList<LocalPresetOrigin> origins, string productPn, int sourceIdentity) =>
        origins.Any(o =>
            o.SourceIdentity == sourceIdentity
            && string.Equals(o.SourceProductPn, productPn, StringComparison.OrdinalIgnoreCase));

    private static void UpsertOrigin(List<LocalPresetOrigin> origins, LocalPresetOrigin next)
    {
        origins.RemoveAll(o =>
            o.Id == next.Id
            || (o.SourceIdentity == next.SourceIdentity
                && string.Equals(o.SourceProductPn, next.SourceProductPn, StringComparison.OrdinalIgnoreCase)));
        origins.Add(next);
    }

    private static int RemapStepParameterId(
        IReadOnlyList<LocalPresetOrigin> paramOrigins,
        string productPn,
        int stepParameterId)
    {
        if (stepParameterId is < ProcessParameterCode.MinDeviceParameterId
            or > ProcessParameterCode.MaxDeviceParameterId)
            return stepParameterId;

        foreach (var origin in paramOrigins)
        {
            if (origin.Id == stepParameterId
                && string.Equals(origin.SourceProductPn, productPn, StringComparison.OrdinalIgnoreCase))
                return origin.Id;
        }

        var slotId = ProcessParameterCode.ToSlotIndex(stepParameterId);
        foreach (var origin in paramOrigins)
        {
            if (origin.SourceIdentity == slotId
                && string.Equals(origin.SourceProductPn, productPn, StringComparison.OrdinalIgnoreCase))
                return origin.Id;
        }

        throw new InvalidOperationException(
            $"产品 {productPn} 顺序步骤参数 ID {stepParameterId} 无法映射到本机预设（槽位 {slotId:D2}）。");
    }
}
