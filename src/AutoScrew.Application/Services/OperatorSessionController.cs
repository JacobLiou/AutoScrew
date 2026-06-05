using System.Collections.Immutable;
using System.Text.Json;
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
    private readonly ITemplateLayoutLoader _templateLoader;
    private readonly ILockStationHardware _hardware;
    private readonly ICurveArchive _curveArchive;
    private readonly ILockSessionRepository _checkpointStore;
    private readonly IOutboundMesQueue _outbox;
    private readonly ICurrentUser _currentUser;
    private readonly IOptions<AutoScrewAppOptions> _options;
    private readonly IUserAuditService _audit;
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
    private int _templateSurfaceCount = 1;
    private string? _activeSurfaceId;
    private string? _activeSurfaceName;

    public OperatorSessionController(
        IMesClient mesClient,
        ITemplateLayoutLoader templateLoader,
        ILockStationHardware hardware,
        ICurveArchive curveArchive,
        ILockSessionRepository checkpointStore,
        IOutboundMesQueue outbox,
        ICurrentUser currentUser,
        IOptions<AutoScrewAppOptions> options,
        IUserAuditService audit,
        ILogger<OperatorSessionController> logger)
    {
        _mesClient = mesClient;
        _templateLoader = templateLoader;
        _hardware = hardware;
        _curveArchive = curveArchive;
        _checkpointStore = checkpointStore;
        _outbox = outbox;
        _currentUser = currentUser;
        _options = options;
        _audit = audit;
        _logger = logger;
    }

    public event EventHandler? Changed;

    public JobSessionPhase Phase => _phase;

    public string? SerialNumber => _serialNumber;

    public string? PartNumber => _partNumber;

    public string? LastErrorMessage => _lastErrorMessage;

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

    public async Task SubmitSerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serialNumber);
        _lastErrorMessage = null;

        if (_phase != JobSessionPhase.SnPending && _phase != JobSessionPhase.SnRejected)
            throw new InvalidOperationException("Not awaiting SN.");

        var validation = await _mesClient.ValidateSnAsync(serialNumber.Trim(), cancellationToken).ConfigureAwait(false);
        if (!validation.IsValid || string.IsNullOrWhiteSpace(validation.PartNumber))
        {
            _serialNumber = serialNumber.Trim();
            TryApply(JobSessionTrigger.SnRejected);
            _lastErrorMessage = validation.Message ?? "SN invalid.";
            AuditOperation("Operation.SnRejected", $"sn={_serialNumber}", success: false, _serialNumber);
            NotifyChanged();
            return;
        }

        _serialNumber = serialNumber.Trim();
        _partNumber = validation.PartNumber!;
        if (!TryApply(JobSessionTrigger.SnValidated))
            throw new InvalidOperationException("State error after SN validation.");

        AuditOperation("Operation.SnAccepted", $"sn={_serialNumber};pn={_partNumber}");
        await LoadRecipeAndTemplateAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task LoadRecipeAndTemplateAsync(CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await _mesClient
                .GetRecipeAsync(_serialNumber!, _partNumber!, cancellationToken)
                .ConfigureAwait(false);

            var templatePath = ResolveTemplatePath(recipe.TemplateJsonPath);
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
            await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
            NotifyChanged();
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

    private string? ResolveTemplatePath(string? templateJsonPathFromMes)
    {
        if (string.IsNullOrWhiteSpace(templateJsonPathFromMes))
            return null;

        if (File.Exists(templateJsonPathFromMes))
            return templateJsonPathFromMes;

        var dir = _options.Value.TemplateDirectory;
        if (!string.IsNullOrWhiteSpace(dir))
        {
            var combined = Path.Combine(dir, templateJsonPathFromMes);
            if (File.Exists(combined))
                return combined;
        }

        return templateJsonPathFromMes;
    }

    public void SetReworkMode(bool enabled)
    {
        if (_currentUser.Role < UserRole.Technician)
            throw new UnauthorizedAccessException("Rework flag requires technician.");
        _isRework = enabled;
    }

    public async Task RunCurrentScrewCycleAsync(CancellationToken cancellationToken = default)
    {
        if (_phase != JobSessionPhase.Running)
            throw new InvalidOperationException("Not in Running phase.");

        if (_currentIndex < 0 || _currentIndex >= _positions.Length)
            throw new InvalidOperationException("No active screw index.");

        var idx = _currentIndex;
        var localIndex = _positions[idx].Index;
        var globalIndex = ComputeGlobalIndex(_activeSurfaceOrdinal, idx);
        SetState(idx, StationScrewState.InProgress);
        await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
        NotifyChanged();

        await _hardware.PickScrewAsync(cancellationToken).ConfigureAwait(false);

        var dto = _recipeScrews.FirstOrDefault(s => s.PositionIndex == localIndex)
                  ?? _recipeScrews.FirstOrDefault(s => s.PositionIndex == globalIndex)
                  ?? _recipeScrews.ElementAtOrDefault(idx);
        var paramId = dto?.ControllerParameterId ?? localIndex;
        var tighteningContext = new TighteningContext(globalIndex, paramId);

        var samples = new List<TorqueAngleSample>();
        await foreach (var sample in _hardware.RunTighteningAsync(tighteningContext, cancellationToken).ConfigureAwait(false))
            samples.Add(sample);

        LastTighteningSamples = samples;

        var program = _programs[idx];
        var eval = LockCurveEvaluator.Evaluate(samples.ToArray(), program);
        var device = _hardware.LastOutcome;
        var deviceOk = device?.DeviceOk ?? true;
        var combinedOk = eval.IsOk && deviceOk;

        var curvePath = await _curveArchive
            .SaveCurveCsvAsync(_serialNumber!, globalIndex, samples, cancellationToken)
            .ConfigureAwait(false);

        var finalTorque = device?.FinalTorqueNm ?? (samples.Count > 0 ? samples[^1].TorqueNm : (double?)null);
        var finalAngle = device?.FinalAngleDeg ?? (samples.Count > 0 ? samples[^1].AngleDeg : (double?)null);
        string? errorCode = null;
        if (!combinedOk)
            errorCode = !deviceOk
                ? device?.DeviceErrorCode?.ToString() ?? "DEVICE_NG"
                : eval.ErrorCode;

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
            SetState(idx, StationScrewState.Ok);
            _currentIndex = NextPendingIndex(idx + 1);
            if (_currentIndex < 0)
                await OnActiveSurfaceCompleteAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            SetState(idx, StationScrewState.Ng);
            _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.NgLocked;
            TryApply(JobSessionTrigger.ScrewNg);
            _lastErrorMessage = !deviceOk
                ? $"Device NG (code {device?.DeviceErrorCode})"
                : eval.Message ?? eval.ErrorCode;
            await LogErrorAsync(idx, eval, device, cancellationToken).ConfigureAwait(false);
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
        _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.Complete;

        if (_activeSurfaceOrdinal < _surfaces.Count - 1)
        {
            TryApply(JobSessionTrigger.SurfaceComplete);
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

        var started = DateTimeOffset.UtcNow.AddMinutes(-5);
        var payload = new LockJobResultPayload
        {
            SerialNumber = _serialNumber!,
            PartNumber = _partNumber!,
            StationId = _options.Value.StationId,
            OperatorId = _currentUser.UserId,
            IsRework = _isRework,
            StartedAt = started,
            CompletedAt = DateTimeOffset.UtcNow,
            OverallResult = "OK",
            Screws = screws,
            LockLogJson = JsonSerializer.Serialize(new { note = "minimal lock log v0", surfaceCount = _surfaces.Count })
        };

        var logJson = JsonSerializer.Serialize(payload);
        await _curveArchive.SaveLockLogJsonAsync(_serialNumber!, logJson, cancellationToken).ConfigureAwait(false);

        var upload = await _mesClient.UploadResultAsync(payload, cancellationToken).ConfigureAwait(false);
        if (!upload.Accepted)
            await _outbox.EnqueueAsync(payload, upload.Message, cancellationToken).ConfigureAwait(false);

        await _checkpointStore.ClearCheckpointAsync(cancellationToken).ConfigureAwait(false);
    }

    private Task LogErrorAsync(
        int index,
        LockEvaluationResult eval,
        LockHardwareOutcome? device,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Screw {Index} NG: rule={RuleCode} deviceOk={DeviceOk} deviceCode={DeviceCode}",
            index + 1,
            eval.ErrorCode,
            device?.DeviceOk ?? true,
            device?.DeviceErrorCode);
        return Task.CompletedTask;
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

    public void UnlockNgContinue()
    {
        if (!_currentUser.CanUnlockNg)
            throw new UnauthorizedAccessException("Cannot unlock NG.");

        if (!TryApply(JobSessionTrigger.TechUnlockContinue))
            throw new InvalidOperationException("Unlock not allowed in current phase.");

        if (_activeSurfaceOrdinal < _surfaces.Count)
            _surfaces[_activeSurfaceOrdinal].ProgressState = SurfaceProgressState.Active;

        AuditOperation("Operation.UnlockNg", $"sn={_serialNumber}");
        NotifyChanged();
    }

    public void ResetToIdle()
    {
        if (!TryApply(JobSessionTrigger.ResetToIdle))
            TryApply(JobSessionTrigger.Abort);

        AuditOperation("Operation.ResetSession", $"sn={_serialNumber}");
        ClearSession();
        NotifyChanged();
    }

    public void AbortToIdle()
    {
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
        LastTighteningSamples = Array.Empty<TorqueAngleSample>();
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

        await _checkpointStore.SaveCheckpointAsync(data, cancellationToken).ConfigureAwait(false);
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
