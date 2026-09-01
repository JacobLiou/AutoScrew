using System.Collections.Immutable;
using System.Text.Json;
using System.Threading;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Templates;
using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Application.Services;

/// <summary>
/// Orchestrates operator job session: SN → recipe → per-surface/per-screw pick/tighten/evaluate → MES/outbox.
/// </summary>
public sealed class OperatorSessionController
{
    private readonly IMesClient _mesClient;
    private readonly IRecipeProvisioningService _recipeProvisioning;
    private readonly IControllerTraceService _controllerTrace;
    private readonly ITemplateLayoutLoader _templateLoader;
    private readonly ILockStationHardware _hardware;
    private readonly ICurveArchive _curveArchive;
    private readonly ILockSessionRepository _checkpointStore;
    private readonly IOutboundMesQueue _outbox;
    private readonly ICurrentUser _currentUser;
    private readonly IOptions<AutoScrewAppOptions> _options;
    private readonly IOptions<SimulationOptions> _simulation;
    private readonly IUserAuditService _audit;
    private readonly IHostIdentity _hostIdentity;
    private readonly IProcessChangeoverService? _changeover;
    private readonly ILogger<OperatorSessionController> _logger;

    private JobSessionPhase _phase = JobSessionPhase.Idle;
    private string? _serialNumber;
    private string? _partNumber;
    private ImmutableArray<ScrewPosition> _positions = ImmutableArray<ScrewPosition>.Empty;
    private ImmutableArray<StationScrewState> _states = ImmutableArray<StationScrewState>.Empty;
    private ImmutableArray<SegmentedTorqueProgram> _programs = ImmutableArray<SegmentedTorqueProgram>.Empty;
    private ImmutableArray<ScrewRecipeDto> _recipeScrews = ImmutableArray<ScrewRecipeDto>.Empty;
    private readonly List<ScrewCycleRecord> _screwRecords = new();
    private readonly List<OrderedSurfaceRuntime> _surfaces = new();
    private int _activeSurfaceOrdinal;
    private int _currentIndex;
    private string? _resolvedImagePath;
    private double _boardWidth;
    private double _boardHeight;
    private bool _isRework;
    private string? _lastErrorMessage;
    private string? _lastErrorCode;
    private ushort? _lastDeviceErrorCode;
    private int _lastFailedScrewLocalIndex;
    private int _cycleInProgress;
    private CancellationTokenSource? _cycleCts;
    private CancellationTokenSource? _loadCts;
    private int _templateSurfaceCount = 1;
    private string? _activeSurfaceId;
    private string? _activeSurfaceName;
    private DateTimeOffset _sessionStartedAt;

    public OperatorSessionController(
        IMesClient mesClient,
        IRecipeProvisioningService recipeProvisioning,
        IControllerTraceService controllerTrace,
        ITemplateLayoutLoader templateLoader,
        ILockStationHardware hardware,
        ICurveArchive curveArchive,
        ILockSessionRepository checkpointStore,
        IOutboundMesQueue outbox,
        ICurrentUser currentUser,
        IOptions<AutoScrewAppOptions> options,
        IOptions<SimulationOptions> simulation,
        IUserAuditService audit,
        IHostIdentity hostIdentity,
        ILogger<OperatorSessionController> logger,
        IProcessChangeoverService? changeover = null)
    {
        _mesClient = mesClient;
        _recipeProvisioning = recipeProvisioning;
        _controllerTrace = controllerTrace;
        _templateLoader = templateLoader;
        _hardware = hardware;
        _curveArchive = curveArchive;
        _checkpointStore = checkpointStore;
        _outbox = outbox;
        _currentUser = currentUser;
        _options = options;
        _simulation = simulation;
        _audit = audit;
        _hostIdentity = hostIdentity;
        _logger = logger;
        _changeover = changeover;
    }

    public event EventHandler? Changed;

    public event EventHandler? TighteningProgress;

    public event EventHandler<ScrewCycleProgressEventArgs>? ScrewCycleProgress;

    public JobSessionPhase Phase => _phase;

    public string? SerialNumber => _serialNumber;

    public string? PartNumber => _partNumber;

    public string? LastErrorMessage => _lastErrorMessage;

    public string? LastErrorCode => _lastErrorCode;

    public ushort? LastDeviceErrorCode => _lastDeviceErrorCode;

    public int LastFailedScrewLocalIndex => _lastFailedScrewLocalIndex;

    public bool IsCycleInProgress => _cycleInProgress > 0;

    public int TemplateSurfaceCount => _templateSurfaceCount;

    public int ActiveSurfaceOrdinal => _activeSurfaceOrdinal;

    public string? ActiveSurfaceId => _activeSurfaceId;

    public string? ActiveSurfaceName => _activeSurfaceName;

    public int CurrentScrewIndex => _currentIndex;

    public int CurrentScrewLocalIndex =>
        _currentIndex >= 0 && _currentIndex < _positions.Length ? _positions[_currentIndex].Index : 0;

    public bool IsRework => _isRework;

    public IReadOnlyList<ScrewPosition> Positions => _positions;

    public IReadOnlyList<StationScrewState> ScrewStates => _states;

    public IReadOnlyList<OperatorSurfaceSnapshot> SurfaceSnapshots =>
        _surfaces.Select(s => new OperatorSurfaceSnapshot(
            s.SurfaceId,
            s.Name,
            s.Order,
            s.ProgressState,
            s.Positions.Select(p => p.Index).ToList(),
            s.States)).ToList();

    public string? ResolvedProductImagePath => _resolvedImagePath;

    public double BoardWidth => _boardWidth;

    public double BoardHeight => _boardHeight;

    public IReadOnlyList<TorqueAngleSample> LastTighteningSamples { get; private set; } = Array.Empty<TorqueAngleSample>();

    public sealed record CheckpointRestoreOffer(
        string SerialNumber,
        string PartNumber,
        JobSessionPhase Phase,
        int CompletedScrewCount,
        int TotalScrewCount);

    public bool IsActiveJobPhase =>
        _phase is JobSessionPhase.Running or JobSessionPhase.AwaitFlip or JobSessionPhase.NgLocked;

    public async Task<CheckpointRestoreOffer?> GetCheckpointRestoreOfferAsync(CancellationToken cancellationToken = default)
    {
        var data = await _checkpointStore.LoadLatestRestorableAsync(cancellationToken).ConfigureAwait(false);
        return data is null ? null : ToRestoreOffer(data);
    }

    public async Task<CheckpointRestoreOffer?> TryGetRestorableMemoryAsync(
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return null;

        var status = await _checkpointStore.GetJobMemoryStatusAsync(serialNumber.Trim(), cancellationToken)
            .ConfigureAwait(false);
        if (status is null or SnJobMemoryStatus.Completed)
            return null;

        var data = await _checkpointStore.LoadJobMemoryAsync(serialNumber.Trim(), cancellationToken)
            .ConfigureAwait(false);
        if (data is null || !IsRestorablePhase(data.Phase))
            return null;

        return ToRestoreOffer(data);
    }

    /// <summary>按 SN 恢复作业记忆（可在 Accept 后的 LoadingRecipe 调用）。</summary>
    public async Task<bool> RestoreJobMemoryAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
            return false;

        var sn = serialNumber.Trim();
        var status = await _checkpointStore.GetJobMemoryStatusAsync(sn, cancellationToken).ConfigureAwait(false);
        if (status is null or SnJobMemoryStatus.Completed)
            return false;

        var data = await _checkpointStore.LoadJobMemoryAsync(sn, cancellationToken).ConfigureAwait(false);
        if (data is null || !IsRestorablePhase(data.Phase))
            return false;

        try
        {
            _serialNumber = data.SerialNumber;
            _partNumber = data.PartNumber;
            _phase = data.Phase;
            _activeSurfaceOrdinal = data.ActiveSurfaceOrdinal;
            _currentIndex = data.CurrentScrewIndex;
            _sessionStartedAt = data.UpdatedAt;

            await ReloadTemplateForRestoreAsync(cancellationToken).ConfigureAwait(false);
            MergeCheckpointSurfaceStates(data);

            if (_phase == JobSessionPhase.Running && _currentIndex < 0)
                _currentIndex = NextPendingIndex(0);

            AuditOperation("Operation.RestoreCheckpoint", $"sn={_serialNumber};phase={_phase}");
            await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
            NotifyChanged();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore job memory for SN={SerialNumber}", data.SerialNumber);
            _lastErrorMessage = ex.Message;
            ClearSession();
            NotifyChanged();
            return false;
        }
    }

    public async Task<bool> RestoreFromCheckpointAsync(CancellationToken cancellationToken = default)
    {
        var data = await _checkpointStore.LoadLatestRestorableAsync(cancellationToken).ConfigureAwait(false);
        if (data is null)
            return false;
        return await RestoreJobMemoryAsync(data.SerialNumber, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>启动时拒绝恢复：保留按 SN 记忆供再扫恢复。</summary>
    public Task DiscardCheckpointAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    /// <summary>写条码后按记忆恢复（Accept + 换产之后）。</summary>
    public async Task ContinueRestoreAfterSerialAcceptedAsync(
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        if (_phase != JobSessionPhase.LoadingRecipe ||
            string.IsNullOrWhiteSpace(_serialNumber) ||
            string.IsNullOrWhiteSpace(_partNumber))
            throw new InvalidOperationException("No accepted SN to restore.");

        try
        {
            await _controllerTrace.WriteSerialNumberAsync(_serialNumber!, cancellationToken).ConfigureAwait(false);
            AuditOperation("Operation.WriteBarcode", $"sn={_serialNumber}", serialNumber: _serialNumber);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Write barcode to controller failed for SN={SerialNumber}", _serialNumber);
            AuditOperation("Operation.WriteBarcode", $"sn={_serialNumber};error={ex.Message}", success: false, _serialNumber);
            AbortAcceptedSerial(ex.Message);
            return;
        }

        var ok = await RestoreJobMemoryAsync(serialNumber, cancellationToken).ConfigureAwait(false);
        if (!ok)
        {
            AbortAcceptedSerial(_lastErrorMessage ?? "Restore job memory failed.");
        }
    }

    public void RequestScanDialog()
    {
        _lastErrorMessage = null;
        if (_phase == JobSessionPhase.SnPending)
        {
            NotifyChanged();
            return;
        }

        if (!TryApply(JobSessionTrigger.RequestScan))
            throw new InvalidOperationException($"Cannot open scan from {_phase}.");

        NotifyChanged();
    }

    public sealed record SerialAcceptResult(bool Accepted, string? SerialNumber, string? PartNumber, string? ErrorMessage);

    /// <summary>MES 校验并进入 LoadingRecipe；不写条码、不加载配方（供换产确认插入）。</summary>
    public async Task<SerialAcceptResult> AcceptSerialNumberAsync(
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serialNumber);
        _lastErrorMessage = null;
        var ct = BeginLoadOperations(cancellationToken);

        if (IsActiveJobPhase)
        {
            var input = serialNumber.Trim();
            if (string.Equals(_serialNumber, input, StringComparison.OrdinalIgnoreCase))
            {
                return new SerialAcceptResult(
                    false,
                    _serialNumber,
                    _partNumber,
                    "ActiveJobSameSn");
            }

            return new SerialAcceptResult(
                false,
                _serialNumber,
                _partNumber,
                "ActiveJobMustReset");
        }

        if (_phase == JobSessionPhase.SnRejected)
            TryApply(JobSessionTrigger.RequestScan);

        if (_phase != JobSessionPhase.SnPending)
            throw new InvalidOperationException("Not awaiting SN.");

        var validation = await _mesClient.ValidateSnAsync(serialNumber.Trim(), ct).ConfigureAwait(false);
        if (!validation.IsValid || string.IsNullOrWhiteSpace(validation.PartNumber))
        {
            _serialNumber = serialNumber.Trim();
            _partNumber = null;
            TryApply(JobSessionTrigger.SnRejected);
            _lastErrorMessage = validation.Message ?? "SN invalid.";
            AuditOperation("Operation.SnRejected", $"sn={_serialNumber}", success: false, _serialNumber);
            NotifyChanged();
            return new SerialAcceptResult(false, _serialNumber, null, _lastErrorMessage);
        }

        _serialNumber = serialNumber.Trim();
        _partNumber = validation.PartNumber!;
        if (!TryApply(JobSessionTrigger.SnValidated))
            throw new InvalidOperationException("State error after SN validation.");

        AuditOperation("Operation.SnAccepted", $"sn={_serialNumber};pn={_partNumber}");
        NotifyChanged();
        return new SerialAcceptResult(true, _serialNumber, _partNumber, null);
    }

    /// <summary>换产取消或下发失败：清空刚接受的 SN/PN，回到 SnPending。</summary>
    public void AbortAcceptedSerial(string? errorMessage = null)
    {
        if (_phase != JobSessionPhase.LoadingRecipe)
            return;

        TryApply(JobSessionTrigger.Abort);
        _serialNumber = null;
        _partNumber = null;
        _lastErrorMessage = errorMessage;
        AuditOperation(
            "Operation.SnAcceptAborted",
            errorMessage is null ? "changeover cancelled" : errorMessage,
            success: false);
        NotifyChanged();
    }

    /// <summary>写条码并加载配方/模板（换产跳过或下发成功后调用）。</summary>
    public async Task ContinueAfterSerialAcceptedAsync(CancellationToken cancellationToken = default)
    {
        if (_phase != JobSessionPhase.LoadingRecipe ||
            string.IsNullOrWhiteSpace(_serialNumber) ||
            string.IsNullOrWhiteSpace(_partNumber))
            throw new InvalidOperationException("No accepted SN to continue.");

        var ct = BeginLoadOperations(cancellationToken);

        try
        {
            await _controllerTrace.WriteSerialNumberAsync(_serialNumber!, ct).ConfigureAwait(false);
            AuditOperation("Operation.WriteBarcode", $"sn={_serialNumber}", serialNumber: _serialNumber);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Write barcode to controller failed for SN={SerialNumber}", _serialNumber);
            AuditOperation("Operation.WriteBarcode", $"sn={_serialNumber};error={ex.Message}", success: false, _serialNumber);
            AbortAcceptedSerial(ex.Message);
            return;
        }

        await LoadRecipeAndTemplateAsync(ct).ConfigureAwait(false);
    }

    /// <summary>无换产确认的兼容路径：Accept → Continue。</summary>
    public async Task SubmitSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        var accepted = await AcceptSerialNumberAsync(serialNumber, cancellationToken).ConfigureAwait(false);
        if (!accepted.Accepted)
            return;

        await ContinueAfterSerialAcceptedAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task LoadRecipeAndTemplateAsync(CancellationToken cancellationToken)
    {
        try
        {
            var provisioned = await _recipeProvisioning
                .GetProvisionedRecipeAsync(_serialNumber!, _partNumber!, cancellationToken)
                .ConfigureAwait(false);

            var recipe = provisioned.Recipe;
            var templatePath = provisioned.ResolvedTemplatePath;
            if (!string.IsNullOrWhiteSpace(provisioned.InfoMessage))
            {
                _logger.LogInformation("Template provisioning info: {Message}", provisioned.InfoMessage);
                _lastErrorMessage = provisioned.InfoMessage;
            }

            if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
            {
                _lastErrorMessage = "Template file not found for PN.";
                TryApply(JobSessionTrigger.LoadFailed);
                ClearSession();
                NotifyChanged();
                return;
            }

            var productLoad = await _templateLoader.LoadProductAsync(templatePath, cancellationToken).ConfigureAwait(false);
            var ordered = productLoad.Product.Surfaces
                .Where(s => s.Enabled)
                .OrderBy(s => s.Order)
                .ThenBy(s => s.SurfaceId, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ordered.Count == 0)
            {
                _lastErrorMessage = "Product template has no enabled surfaces.";
                TryApply(JobSessionTrigger.LoadFailed);
                ClearSession();
                NotifyChanged();
                return;
            }

            _surfaces.Clear();
            foreach (var surface in ordered)
            {
                var positions = BuildPositions(surface);
                var states = Enumerable.Repeat(StationScrewState.Pending, positions.Count).ToArray();
                var imagePath = ResolveImagePath(productLoad.BaseDirectory, surface);
                _surfaces.Add(new OrderedSurfaceRuntime(
                    surface.SurfaceId,
                    surface.Name,
                    surface.Order,
                    surface.BoardWidth,
                    surface.BoardHeight,
                    imagePath,
                    positions,
                    states,
                    SurfaceProgressState.Locked));
            }

            _templateSurfaceCount = _surfaces.Count;
            _activeSurfaceOrdinal = 0;
            _surfaces[0].ProgressState = SurfaceProgressState.Active;
            _recipeScrews = recipe.Screws.ToImmutableArray();
            _screwRecords.Clear();
            ApplyActiveSurfaceToBoard(recipe);

            if (!TryApply(JobSessionTrigger.RecipeLoaded))
            {
                _lastErrorMessage = "State machine rejected RecipeLoaded.";
                ClearSession();
                NotifyChanged();
                return;
            }

            _currentIndex = NextPendingIndex(0);
            _sessionStartedAt = DateTimeOffset.UtcNow;
            var sequenceId = await ResolveJobSequenceIdAsync(cancellationToken).ConfigureAwait(false);
            await _hardware.PrepareForJobAsync(cancellationToken, sequenceId).ConfigureAwait(false);
            await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
            NotifyChanged();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadRecipe failed");
            _lastErrorMessage = ex.Message;
            TryApply(JobSessionTrigger.LoadFailed);
            ClearSession();
            NotifyChanged();
        }
    }

    private async Task<int?> ResolveJobSequenceIdAsync(CancellationToken cancellationToken)
    {
        if (_changeover is null || string.IsNullOrWhiteSpace(_partNumber))
            return null;

        try
        {
            return await _changeover.ResolveActiveSequenceIdAsync(_partNumber, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Resolve job sequence id failed for PN={PartNumber}", _partNumber);
            return null;
        }
    }

    public (string? SurfaceId, string? Name) GetPendingFlipTarget()
    {
        if (_phase != JobSessionPhase.AwaitFlip || _activeSurfaceOrdinal >= _surfaces.Count - 1)
            return (null, null);

        var next = _surfaces[_activeSurfaceOrdinal + 1];
        return (next.SurfaceId, next.Name);
    }

    public (string? SurfaceId, string? Name) GetCompletedSurfaceForFlip()
    {
        if (_phase != JobSessionPhase.AwaitFlip || _activeSurfaceOrdinal >= _surfaces.Count)
            return (null, null);

        var current = _surfaces[_activeSurfaceOrdinal];
        return (current.SurfaceId, current.Name);
    }

    public void ConfirmAdvanceToNextSurface()
    {
        if (_phase != JobSessionPhase.AwaitFlip)
            throw new InvalidOperationException("Not awaiting surface flip confirmation.");

        if (_activeSurfaceOrdinal >= _surfaces.Count - 1)
            throw new InvalidOperationException("No next surface to advance to.");

        if (!ValidateSurfaceAllOk(_activeSurfaceOrdinal, out var missingMessage))
            throw new InvalidOperationException(missingMessage);

        _activeSurfaceOrdinal++;
        _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.Active;
        ApplyActiveSurfaceToBoard(null);
        _currentIndex = NextPendingIndex(0);

        if (!TryApply(JobSessionTrigger.SurfaceAdvanceConfirmed))
            throw new InvalidOperationException("State machine rejected SurfaceAdvanceConfirmed.");

        AuditOperation(
            "Operation.SurfaceFlip",
            $"surface={_activeSurfaceId};ordinal={_activeSurfaceOrdinal}");
        NotifyChanged();
    }

    private void ApplyActiveSurfaceToBoard(RecipeBundle? recipe)
    {
        var surface = _surfaces[_activeSurfaceOrdinal];
        _activeSurfaceId = surface.SurfaceId;
        _activeSurfaceName = surface.Name;
        _boardWidth = surface.BoardWidth;
        _boardHeight = surface.BoardHeight;
        _resolvedImagePath = surface.ResolvedImagePath;
        _positions = surface.Positions.ToImmutableArray();
        _states = surface.States.ToImmutableArray();
        _programs = recipe is not null
            ? BuildPrograms(recipe, _positions.Length)
            : BuildProgramsFromExisting(_positions.Length);
    }

    private ImmutableArray<SegmentedTorqueProgram> BuildProgramsFromExisting(int markerCount)
    {
        if (_programs.Length == markerCount)
            return _programs;

        return BuildPrograms(new RecipeBundle(_partNumber ?? "", null, null, _recipeScrews), markerCount);
    }

    private static ImmutableArray<SegmentedTorqueProgram> BuildPrograms(RecipeBundle recipe, int markerCount)
    {
        var list = new List<SegmentedTorqueProgram>(markerCount);
        for (var i = 0; i < markerCount; i++)
        {
            var dto = recipe.Screws.FirstOrDefault(s => s.PositionIndex == i + 1)
                      ?? recipe.Screws.ElementAtOrDefault(i);
            var target = dto?.TargetTorqueNm ?? 0.35;
            var lower = dto?.TorqueLowerNm ?? target * 0.75;
            var upper = dto?.TorqueUpperNm ?? target * 1.05;
            var angle = dto?.AngleLimitDeg ?? 720;
            list.Add(new SegmentedTorqueProgram(
                target,
                lower,
                upper,
                angle,
                maxAxisSkewDeg: 3.0,
                stripDetectionMinSlopeNmPerDeg: 0.5,
                jamTorqueDeltaNm: 0.08,
                jamRpmDropRatio: 0.35));
        }

        return list.ToImmutableArray();
    }

    private static List<ScrewPosition> BuildPositions(SurfaceLayoutDto surface)
    {
        var markers = surface.Markers.OrderBy(m => m.Index).ToList();
        var positions = new List<ScrewPosition>(markers.Count);
        foreach (var m in markers)
        {
            var diameter = m.CircleDiameter ?? surface.CircleDiameter;
            positions.Add(new ScrewPosition(m.Index, m.CenterX, m.CenterY, diameter, m.ScrewTypeId, m.PartNo));
        }

        return positions;
    }

    private static string? ResolveImagePath(string baseDir, SurfaceLayoutDto surface)
    {
        if (!string.IsNullOrWhiteSpace(surface.ProductImageRelativePath))
        {
            var rel = Path.Combine(baseDir, surface.ProductImageRelativePath);
            if (File.Exists(rel))
                return rel;
        }

        if (!string.IsNullOrWhiteSpace(surface.ProductImageAbsolutePath) && File.Exists(surface.ProductImageAbsolutePath))
            return surface.ProductImageAbsolutePath;

        return null;
    }

    public void SetReworkMode(bool enabled)
    {
        if (_currentUser.Role < UserRole.Technician)
            throw new UnauthorizedAccessException("Rework flag requires technician.");
        _isRework = enabled;
        NotifyChanged();
    }

    /// <summary>Minimum trimmed length for emergency unlock reason.</summary>
    public const int EmergencyUnlockReasonMinLength = 4;

    public async Task RunCurrentScrewCycleAsync(CancellationToken cancellationToken = default)
    {
        if (_phase != JobSessionPhase.Running)
            throw new InvalidOperationException("Not in Running phase.");

        if (_currentIndex < 0 || _currentIndex >= _positions.Length)
            throw new InvalidOperationException("No active screw index.");

        if (Interlocked.CompareExchange(ref _cycleInProgress, 1, 0) != 0)
            throw new InvalidOperationException("Screw cycle already in progress.");

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var previous = Interlocked.Exchange(ref _cycleCts, linked);
        previous?.Dispose();

        try
        {
            await RunCurrentScrewCycleCoreAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            RestoreInProgressScrewToPending();
            _logger.LogInformation("Screw cycle cancelled (session reset / park).");
            NotifyChanged();
        }
        finally
        {
            Interlocked.Exchange(ref _cycleInProgress, 0);
            Interlocked.CompareExchange(ref _cycleCts, null, linked);
        }
    }

    /// <summary>
    /// Cancel in-flight pick/tighten and SN load so DeviceSession.IsBusy can release
    /// and workbench UI can return to Idle input.
    /// </summary>
    public async Task StopActiveScrewCycleAsync(CancellationToken cancellationToken = default)
    {
        CancelLoadOperations();

        try
        {
            _cycleCts?.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // ignored
        }

        var deadline = Environment.TickCount64 + 8_000;
        while (Volatile.Read(ref _cycleInProgress) != 0 && Environment.TickCount64 < deadline)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
        }

        if (Volatile.Read(ref _cycleInProgress) != 0)
            _logger.LogWarning("Screw cycle still marked in-progress after cancel wait.");
    }

    private void CancelLoadOperations()
    {
        var cts = Interlocked.Exchange(ref _loadCts, null);
        if (cts is null)
            return;

        try
        {
            cts.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // ignored
        }

        cts.Dispose();
    }

    /// <summary>Token for Accept/Continue/recipe load; cancelled by reset/park.</summary>
    private CancellationToken BeginLoadOperations(CancellationToken external)
    {
        var linked = CancellationTokenSource.CreateLinkedTokenSource(external);
        var previous = Interlocked.Exchange(ref _loadCts, linked);
        previous?.Dispose();
        return linked.Token;
    }

    private void RestoreInProgressScrewToPending()
    {
        if (_currentIndex >= 0
            && _currentIndex < _states.Length
            && _states[_currentIndex] == StationScrewState.InProgress)
        {
            SetState(_currentIndex, StationScrewState.Pending);
        }
    }

    private async Task RunCurrentScrewCycleCoreAsync(CancellationToken cancellationToken)
    {
        var idx = _currentIndex;
        var localIndex = _positions[idx].Index;
        var globalIndex = ComputeGlobalIndex(_activeSurfaceOrdinal, idx);
        var surfaceName = _activeSurfaceName ?? _activeSurfaceId ?? "";
        SetState(idx, StationScrewState.InProgress);
        await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
        NotifyChanged();

        NotifyScrewCycleProgress(ScrewCycleProgressStep.Started, surfaceName, localIndex);

        try
        {
            NotifyScrewCycleProgress(ScrewCycleProgressStep.Picking, surfaceName, localIndex);
            await _hardware.PickScrewAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (FeedFaultException ex)
        {
            NotifyScrewCycleProgress(
                ScrewCycleProgressStep.FeedFailed,
                surfaceName,
                localIndex,
                ex.Message,
                ex.ErrorCode);
            await HandleFeedFailureAsync(idx, ex, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (_options.Value.UseSimulatedHardware)
        {
            var pickToTightenDelay = Math.Max(0, _simulation.Value.PickToTightenDelayMs);
            if (pickToTightenDelay > 0)
                await Task.Delay(pickToTightenDelay, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            NotifyScrewCycleProgress(ScrewCycleProgressStep.PickCompleteWaitTrigger, surfaceName, localIndex);
        }

        NotifyScrewCycleProgress(ScrewCycleProgressStep.Tightening, surfaceName, localIndex);

        var dto = _recipeScrews.FirstOrDefault(s => s.PositionIndex == localIndex)
                  ?? _recipeScrews.FirstOrDefault(s => s.PositionIndex == globalIndex)
                  ?? _recipeScrews.ElementAtOrDefault(idx);
        var paramId = dto?.ControllerParameterId ?? localIndex;
        var tighteningContext = new TighteningContext(globalIndex, paramId);

        var samples = new List<TorqueAngleSample>();
        LastTighteningSamples = Array.Empty<TorqueAngleSample>();
        NotifyTighteningProgress();

        var progressCounter = 0;
        await foreach (var sample in _hardware.RunTighteningAsync(tighteningContext, cancellationToken).ConfigureAwait(false))
        {
            samples.Add(sample);
            progressCounter++;
            if (progressCounter % 3 == 0 || progressCounter == 1)
            {
                LastTighteningSamples = samples.ToArray();
                NotifyTighteningProgress();
            }
        }

        LastTighteningSamples = samples.ToArray();
        NotifyTighteningProgress();

        var program = _programs[idx];
        var eval = LockCurveEvaluator.Evaluate(samples.ToArray(), program);
        var device = _hardware.LastOutcome;
        var deviceOk = device?.DeviceOk ?? true;
        // 产线 OK/NG 以设备周期 IsOk 为准；LockCurveEvaluator 仅 advisory，不锁屏。
        var combinedOk = deviceOk;

        if (deviceOk && !eval.IsOk)
        {
            _logger.LogWarning(
                "Screw {Index} curve advisory (device OK): {RuleCode} — {Message}",
                idx + 1,
                eval.ErrorCode,
                eval.Message);
        }

        var curvePath = await _curveArchive
            .SaveCurveCsvAsync(_serialNumber!, globalIndex, samples, cancellationToken)
            .ConfigureAwait(false);

        var finalTorque = device?.FinalTorqueNm ?? (samples.Count > 0 ? samples[^1].TorqueNm : (double?)null);
        var finalAngle = device?.FinalAngleDeg ?? (samples.Count > 0 ? samples[^1].AngleDeg : (double?)null);
        string? errorCode = null;
        if (!combinedOk)
        {
            if (device?.DeviceErrorCode is ushort dc)
            {
                errorCode = $"DEVICE_{dc}";
                _lastDeviceErrorCode = dc;
                _lastErrorMessage = DeviceNgDisplayFormat.BuildDeviceMessage(dc);
            }
            else
            {
                errorCode = "DEVICE_NG";
                _lastDeviceErrorCode = null;
                _lastErrorMessage = "设备判定 NG";
            }

            _lastFailedScrewLocalIndex = localIndex;
        }
        else
        {
            _lastDeviceErrorCode = null;
            _lastFailedScrewLocalIndex = 0;
        }

        _lastErrorCode = combinedOk ? null : errorCode;
        _screwRecords.Add(new ScrewCycleRecord(
            _activeSurfaceId!,
            localIndex,
            globalIndex,
            combinedOk,
            errorCode,
            finalTorque,
            finalAngle,
            curvePath));

        if (combinedOk)
        {
            NotifyScrewCycleProgress(ScrewCycleProgressStep.CompletedOk, surfaceName, localIndex);
            SetState(idx, StationScrewState.Ok);
            _currentIndex = NextPendingIndex(idx + 1);
            if (_currentIndex < 0)
                await OnActiveSurfaceCompleteAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var ngMessage = _lastErrorMessage ?? "设备判定 NG";
            NotifyScrewCycleProgress(
                ScrewCycleProgressStep.CompletedNg,
                surfaceName,
                localIndex,
                ngMessage,
                errorCode);
            SetState(idx, StationScrewState.Ng);
            _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.NgLocked;
            TryApply(JobSessionTrigger.ScrewNg);
            LogDeviceNg(idx, device);
        }

        AuditOperation(
            combinedOk ? "Operation.ScrewOk" : "Operation.ScrewNg",
            $"surface={_activeSurfaceId};screw={localIndex};global={globalIndex};error={errorCode}",
            success: combinedOk);

        await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
        NotifyChanged();
    }

    private async Task OnActiveSurfaceCompleteAsync(CancellationToken cancellationToken)
    {
        if (!ValidateSurfaceAllOk(_activeSurfaceOrdinal, out var surfaceMessage))
        {
            _lastErrorCode = "MISSING_SCREW_001";
            _lastErrorMessage = surfaceMessage;
            AuditOperation("Operation.MissingScrew", surfaceMessage, success: false);
            NotifyChanged();
            return;
        }

        _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.Complete;

        if (_activeSurfaceOrdinal < _surfaces.Count - 1)
        {
            TryApply(JobSessionTrigger.SurfaceComplete);
            return;
        }

        if (!ValidateAllScrewsOk(out var jobMessage))
        {
            _lastErrorCode = "MISSING_SCREW_001";
            _lastErrorMessage = jobMessage;
            AuditOperation("Operation.MissingScrew", jobMessage, success: false);
            NotifyChanged();
            return;
        }

        if (TryApply(JobSessionTrigger.AllScrewsComplete))
            await CompleteSessionAsync(cancellationToken).ConfigureAwait(false);
    }

    private int ComputeGlobalIndex(int surfaceOrdinal, int screwIndexInSurface)
    {
        var global = 1;
        for (var s = 0; s < surfaceOrdinal; s++)
            global += _surfaces[s].Positions.Count;

        return global + screwIndexInSurface;
    }

    private async Task CompleteSessionAsync(CancellationToken cancellationToken)
    {
        var screws = new List<ScrewResultDto>();
        var globalIndex = 1;
        foreach (var surface in _surfaces)
        {
            for (var i = 0; i < surface.States.Length; i++)
            {
                var st = surface.States[i];
                var localIndex = surface.Positions[i].Index;
                var record = _screwRecords.FirstOrDefault(r =>
                    r.SurfaceId == surface.SurfaceId && r.LocalIndex == localIndex);
                screws.Add(new ScrewResultDto
                {
                    PositionIndex = globalIndex,
                    Result = st == StationScrewState.Ok ? "OK" : st == StationScrewState.Ng ? "NG" : "SKIPPED",
                    ErrorCode = record?.ErrorCode ?? (st == StationScrewState.Ng ? "NG" : null),
                    FinalTorqueNm = record?.FinalTorqueNm,
                    FinalAngleDeg = record?.FinalAngleDeg,
                    CurveRelativePath = record?.CurveRelativePath
                });
                globalIndex++;
            }
        }

        var started = _sessionStartedAt == default ? DateTimeOffset.UtcNow.AddMinutes(-5) : _sessionStartedAt;
        var overallOk = screws.All(s => s.Result == "OK");
        var payload = new LockJobResultPayload
        {
            SerialNumber = _serialNumber!,
            PartNumber = _partNumber!,
            StationId = _options.Value.StationId,
            HostIp = _hostIdentity.IpAddress,
            HostMac = _hostIdentity.MacAddress,
            OperatorId = _currentUser.UserId,
            IsRework = _isRework,
            StartedAt = started,
            CompletedAt = DateTimeOffset.UtcNow,
            OverallResult = overallOk ? "OK" : "NG",
            Screws = screws,
            LockLogJson = JsonSerializer.Serialize(new { note = "minimal lock log v0", surfaceCount = _surfaces.Count })
        };

        var logJson = JsonSerializer.Serialize(payload);
        await _curveArchive.SaveLockLogJsonAsync(_serialNumber!, logJson, cancellationToken).ConfigureAwait(false);

        try
        {
            await _checkpointStore.SaveLockRecordAsync(payload, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist lock record for SN={SerialNumber}", _serialNumber);
        }

        var upload = await _mesClient.UploadResultAsync(payload, cancellationToken).ConfigureAwait(false);
        if (!upload.Accepted)
            await _outbox.EnqueueAsync(payload, upload.Message, cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(_serialNumber))
        {
            var surfaceCheckpoints = _surfaces.Select(s => new SurfaceCheckpointSurface(
                s.SurfaceId,
                s.ProgressState,
                s.States.ToList())).ToList();
            var data = new SessionCheckpointData(
                JobSessionPhase.Completed,
                _serialNumber!,
                _partNumber ?? "",
                _activeSurfaceOrdinal,
                _currentIndex,
                surfaceCheckpoints,
                DateTimeOffset.UtcNow);
            await _checkpointStore
                .SaveJobMemoryAsync(data, SnJobMemoryStatus.Completed, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private void LogDeviceNg(int index, LockHardwareOutcome? device)
    {
        _logger.LogWarning(
            "Screw {Index} NG: deviceOk=false deviceCode={DeviceCode}",
            index + 1,
            device?.DeviceErrorCode);
    }

    private sealed record ScrewCycleRecord(
        string SurfaceId,
        int LocalIndex,
        int GlobalIndex,
        bool Ok,
        string? ErrorCode,
        double? FinalTorqueNm,
        double? FinalAngleDeg,
        string? CurveRelativePath);

    /// <summary>技术员正式解锁并重打当前钉（清错后解锁）。</summary>
    public async Task UnlockNgContinueAsync(CancellationToken cancellationToken = default)
    {
        if (_currentUser.Role < UserRole.Technician)
            throw new UnauthorizedAccessException("Unlock NG requires technician.");

        await TryClearHardwareErrorsAsync(cancellationToken).ConfigureAwait(false);
        ApplyUnlockNgCore("Operation.UnlockNg", $"sn={_serialNumber}");
    }

    /// <summary>技术员标记返修并解锁重打（会话级 IsRework）。</summary>
    public async Task BeginReworkAndUnlockAsync(CancellationToken cancellationToken = default)
    {
        if (_currentUser.Role < UserRole.Technician)
            throw new UnauthorizedAccessException("Rework requires technician.");

        _isRework = true;
        await TryClearHardwareErrorsAsync(cancellationToken).ConfigureAwait(false);
        ApplyUnlockNgCore("Operation.EnterRework", $"sn={_serialNumber};rework=true");
    }

    /// <summary>操作员紧急解除：必填理由，审计身份后清错解锁（不设返修）。</summary>
    public async Task EmergencyUnlockNgAsync(string reason, CancellationToken cancellationToken = default)
    {
        if (_currentUser.Role != UserRole.Operator)
            throw new UnauthorizedAccessException("Emergency unlock is for operators only.");

        var trimmed = (reason ?? string.Empty).Trim();
        if (trimmed.Length < EmergencyUnlockReasonMinLength)
            throw new ArgumentException(
                $"Emergency unlock reason must be at least {EmergencyUnlockReasonMinLength} characters.",
                nameof(reason));

        var detail =
            $"userId={_currentUser.UserId};displayName={_currentUser.DisplayName};role={_currentUser.Role};" +
            $"sn={_serialNumber};reason={trimmed};errorCode={_lastErrorCode}";
        AuditOperation("Operation.EmergencyUnlockNg", detail, serialNumber: _serialNumber);

        await TryClearHardwareErrorsAsync(cancellationToken).ConfigureAwait(false);
        ApplyUnlockNgCore(auditAction: null, auditDetail: null);
    }

    /// <summary>同步入口（兼容旧调用）；等价于技术员正式解锁且不等待清错完成。</summary>
    public void UnlockNgContinue() =>
        UnlockNgContinueAsync().GetAwaiter().GetResult();

    private async Task TryClearHardwareErrorsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _hardware.ClearErrorsAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ClearErrors before unlock failed; continuing unlock.");
        }
    }

    private void ApplyUnlockNgCore(string? auditAction, string? auditDetail)
    {
        PrepareParkResumeFromNgIfNeeded();
        if (_phase != JobSessionPhase.Running)
            throw new InvalidOperationException("Unlock not allowed in current phase.");

        if (!string.IsNullOrEmpty(auditAction))
            AuditOperation(auditAction, auditDetail ?? $"sn={_serialNumber}");

        NotifyChanged();
    }

    /// <summary>NgLocked → Running，当前钉 Pending，不 Notify（避免挂起前触发 AutoRun）。</summary>
    private void PrepareParkResumeFromNgIfNeeded()
    {
        if (_phase != JobSessionPhase.NgLocked)
            return;

        if (!TryApply(JobSessionTrigger.TechUnlockContinue))
            throw new InvalidOperationException("Cannot leave NG lock in current phase.");

        if (_activeSurfaceOrdinal < _surfaces.Count)
            _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.Active;

        if (_currentIndex >= 0
            && _currentIndex < _states.Length
            && _states[_currentIndex] == StationScrewState.Ng)
        {
            SetState(_currentIndex, StationScrewState.Pending);
        }

        _lastErrorMessage = null;
        _lastErrorCode = null;
        _lastDeviceErrorCode = null;
        _lastFailedScrewLocalIndex = 0;
    }

    /// <summary>
    /// NG 挂起：当前钉改回 Pending、会话记为 Running，再退出到 Idle。
    /// 修好设备后扫同一 SN 恢复，不会带着 NgLocked 遮罩。
    /// </summary>
    public Task ParkJobAsync(CancellationToken cancellationToken = default) =>
        ResetToIdleAsync(cancellationToken);

    public void ResetToIdle() =>
        ResetToIdleAsync().GetAwaiter().GetResult();

    public async Task ResetToIdleAsync(CancellationToken cancellationToken = default)
    {
        // Release device session held by WaitFinish / #751 before clearing job state.
        await StopActiveScrewCycleAsync(cancellationToken).ConfigureAwait(false);

        PrepareParkResumeFromNgIfNeeded();

        if (!string.IsNullOrWhiteSpace(_serialNumber) && IsActiveJobPhase)
            await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);

        if (!TryApply(JobSessionTrigger.ResetToIdle))
            TryApply(JobSessionTrigger.Abort);

        AuditOperation("Operation.ResetSession", $"sn={_serialNumber}");
        ClearSession();
        NotifyChanged();
    }

    public void AbortToIdle() =>
        AbortToIdleAsync().GetAwaiter().GetResult();

    public async Task AbortToIdleAsync(CancellationToken cancellationToken = default)
    {
        await StopActiveScrewCycleAsync(cancellationToken).ConfigureAwait(false);

        PrepareParkResumeFromNgIfNeeded();

        if (!string.IsNullOrWhiteSpace(_serialNumber) &&
            _phase is JobSessionPhase.Running or JobSessionPhase.AwaitFlip or JobSessionPhase.NgLocked
                or JobSessionPhase.LoadingRecipe)
            await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);

        TryApply(JobSessionTrigger.Abort);
        ClearSession();
        NotifyChanged();
    }

    private void ClearSession()
    {
        _serialNumber = null;
        _partNumber = null;
        _surfaces.Clear();
        _positions = ImmutableArray<ScrewPosition>.Empty;
        _states = ImmutableArray<StationScrewState>.Empty;
        _programs = ImmutableArray<SegmentedTorqueProgram>.Empty;
        _recipeScrews = ImmutableArray<ScrewRecipeDto>.Empty;
        _screwRecords.Clear();
        _activeSurfaceOrdinal = 0;
        _currentIndex = 0;
        _resolvedImagePath = null;
        _boardWidth = 0;
        _boardHeight = 0;
        _templateSurfaceCount = 1;
        _activeSurfaceId = null;
        _activeSurfaceName = null;
        _lastErrorMessage = null;
        _lastErrorCode = null;
        _lastDeviceErrorCode = null;
        _lastFailedScrewLocalIndex = 0;
        _isRework = false;
        LastTighteningSamples = Array.Empty<TorqueAngleSample>();
    }

    private async Task HandleFeedFailureAsync(int idx, FeedFaultException ex, CancellationToken cancellationToken)
    {
        SetState(idx, StationScrewState.Pending);
        _lastErrorCode = ex.ErrorCode;
        _lastErrorMessage = ex.Message;
        _lastDeviceErrorCode = null;
        _lastFailedScrewLocalIndex = _positions[idx].Index;
        _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.NgLocked;
        TryApply(JobSessionTrigger.ScrewNg);
        AuditOperation("Operation.FeedNg", $"surface={_activeSurfaceId};screw={_positions[idx].Index};error={ex.ErrorCode}", success: false);
        await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
        NotifyChanged();
    }

    private void NotifyTighteningProgress() => TighteningProgress?.Invoke(this, EventArgs.Empty);

    private void NotifyScrewCycleProgress(
        ScrewCycleProgressStep step,
        string surfaceName,
        int localScrewIndex,
        string? errorMessage = null,
        string? errorCode = null) =>
        ScrewCycleProgress?.Invoke(this, new ScrewCycleProgressEventArgs
        {
            Step = step,
            SurfaceName = surfaceName,
            LocalScrewIndex = localScrewIndex,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        });

    private static bool IsRestorablePhase(JobSessionPhase phase) =>
        phase is JobSessionPhase.Running or JobSessionPhase.AwaitFlip or JobSessionPhase.NgLocked;

    private async Task ReloadTemplateForRestoreAsync(CancellationToken cancellationToken)
    {
        var provisioned = await _recipeProvisioning
            .GetProvisionedRecipeAsync(_serialNumber!, _partNumber!, cancellationToken)
            .ConfigureAwait(false);

        var recipe = provisioned.Recipe;
        var templatePath = provisioned.ResolvedTemplatePath;
        if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
            throw new InvalidOperationException("Template file not found for checkpoint restore.");

        var productLoad = await _templateLoader.LoadProductAsync(templatePath, cancellationToken).ConfigureAwait(false);
        var ordered = productLoad.Product.Surfaces
            .Where(s => s.Enabled)
            .OrderBy(s => s.Order)
            .ThenBy(s => s.SurfaceId, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (ordered.Count == 0)
            throw new InvalidOperationException("Product template has no enabled surfaces.");

        _surfaces.Clear();
        foreach (var surface in ordered)
        {
            var positions = BuildPositions(surface);
            var states = Enumerable.Repeat(StationScrewState.Pending, positions.Count).ToArray();
            var imagePath = ResolveImagePath(productLoad.BaseDirectory, surface);
            _surfaces.Add(new OrderedSurfaceRuntime(
                surface.SurfaceId,
                surface.Name,
                surface.Order,
                surface.BoardWidth,
                surface.BoardHeight,
                imagePath,
                positions,
                states,
                SurfaceProgressState.Locked));
        }

        _templateSurfaceCount = _surfaces.Count;
        _recipeScrews = recipe.Screws.ToImmutableArray();
        _screwRecords.Clear();
        ApplyActiveSurfaceToBoard(recipe);
    }

    private void MergeCheckpointSurfaceStates(SessionCheckpointData data)
    {
        for (var i = 0; i < data.Surfaces.Count && i < _surfaces.Count; i++)
        {
            var cp = data.Surfaces[i];
            var runtime = _surfaces[i];
            if (!string.Equals(cp.SurfaceId, runtime.SurfaceId, StringComparison.OrdinalIgnoreCase))
                _logger.LogWarning("Checkpoint surface {CheckpointId} mismatch template {TemplateId}", cp.SurfaceId, runtime.SurfaceId);

            var states = cp.ScrewStates.ToArray();
            if (states.Length == runtime.States.Length)
                runtime.States = states;

            runtime.ProgressState = cp.ProgressState;
        }

        ApplyActiveSurfaceToBoard(null);
    }

    private bool ValidateSurfaceAllOk(int surfaceOrdinal, out string message)
    {
        message = "";
        if (surfaceOrdinal < 0 || surfaceOrdinal >= _surfaces.Count)
            return true;

        var surface = _surfaces[surfaceOrdinal];
        for (var i = 0; i < surface.States.Length; i++)
        {
            var st = surface.States[i];
            if (st == StationScrewState.Pending)
            {
                message = $"Surface {surface.Name}: screw {surface.Positions[i].Index} not completed (missing screw).";
                return false;
            }

            if (st == StationScrewState.Ng)
            {
                message = $"Surface {surface.Name}: screw {surface.Positions[i].Index} is NG.";
                return false;
            }
        }

        return true;
    }

    private bool ValidateAllScrewsOk(out string message)
    {
        for (var s = 0; s < _surfaces.Count; s++)
        {
            if (!ValidateSurfaceAllOk(s, out message))
                return false;
        }

        message = "";
        return true;
    }

    private void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

    private bool TryApply(JobSessionTrigger trigger)
    {
        if (!JobSessionPhaseMachine.TryTransition(_phase, trigger, out var next))
            return false;
        _phase = next;
        return true;
    }

    private int NextPendingIndex(int start)
    {
        for (var i = start; i < _states.Length; i++)
        {
            if (_states[i] == StationScrewState.Pending)
                return i;
        }

        return -1;
    }

    private void SetState(int index, StationScrewState state)
    {
        var arr = _states.ToArray();
        arr[index] = state;
        _states = arr.ToImmutableArray();
        _surfaces[_activeSurfaceOrdinal].States = arr;
    }

    private async Task PersistCheckpointAsync(CancellationToken cancellationToken)
    {
        if (_serialNumber is null)
            return;

        // 完成后由 MarkJobCompleted 保留成功记录，勿再写成可恢复记忆
        if (_phase == JobSessionPhase.Completed || _phase == JobSessionPhase.Idle)
            return;

        var surfaceCheckpoints = _surfaces.Select(s => new SurfaceCheckpointSurface(
            s.SurfaceId,
            s.ProgressState,
            s.States.ToList())).ToList();

        var data = new SessionCheckpointData(
            _phase,
            _serialNumber,
            _partNumber ?? "",
            _activeSurfaceOrdinal,
            _currentIndex,
            surfaceCheckpoints,
            DateTimeOffset.UtcNow);

        var status = _phase == JobSessionPhase.NgLocked
            ? SnJobMemoryStatus.NgPaused
            : SnJobMemoryStatus.InProgress;

        await _checkpointStore.SaveJobMemoryAsync(data, status, cancellationToken).ConfigureAwait(false);
    }

    private static CheckpointRestoreOffer ToRestoreOffer(SessionCheckpointData data)
    {
        var total = 0;
        var completed = 0;
        foreach (var surface in data.Surfaces)
        {
            foreach (var st in surface.ScrewStates)
            {
                total++;
                if (st == StationScrewState.Ok)
                    completed++;
            }
        }

        return new CheckpointRestoreOffer(
            data.SerialNumber,
            data.PartNumber,
            data.Phase,
            completed,
            total);
    }

    private void AuditOperation(string action, string? detail, bool success = true, string? serialNumber = null) =>
        _audit.Log(new UserAuditEntry(
            DateTimeOffset.Now,
            _options.Value.StationId,
            _currentUser.UserId,
            _currentUser.DisplayName,
            _currentUser.Role,
            AuditCategory.Operation,
            action,
            null,
            detail,
            success,
            serialNumber ?? _serialNumber));

    private sealed class OrderedSurfaceRuntime
    {
        public OrderedSurfaceRuntime(
            string surfaceId,
            string name,
            int order,
            double boardWidth,
            double boardHeight,
            string? resolvedImagePath,
            List<ScrewPosition> positions,
            StationScrewState[] states,
            SurfaceProgressState progressState)
        {
            SurfaceId = surfaceId;
            Name = name;
            Order = order;
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            ResolvedImagePath = resolvedImagePath;
            Positions = positions;
            States = states;
            ProgressState = progressState;
        }

        public string SurfaceId { get; }

        public string Name { get; }

        public int Order { get; }

        public double BoardWidth { get; }

        public double BoardHeight { get; }

        public string? ResolvedImagePath { get; }

        public List<ScrewPosition> Positions { get; }

        public StationScrewState[] States { get; set; }

        public SurfaceProgressState ProgressState { get; set; }
    }
}
