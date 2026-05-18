using System.Collections.Immutable;
using System.Text.Json;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Domain.Curves;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Application.Services;

/// <summary>
/// Orchestrates operator job session: SN → recipe → per-screw pick/tighten/evaluate → MES/outbox.
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
    private readonly ILogger<OperatorSessionController> _logger;

    private JobSessionPhase _phase = JobSessionPhase.Idle;
    private string? _serialNumber;
    private string? _partNumber;
    private ImmutableArray<ScrewPosition> _positions = ImmutableArray<ScrewPosition>.Empty;
    private ImmutableArray<StationScrewState> _states = ImmutableArray<StationScrewState>.Empty;
    private ImmutableArray<SegmentedTorqueProgram> _programs = ImmutableArray<SegmentedTorqueProgram>.Empty;
    private ImmutableArray<ScrewRecipeDto> _recipeScrews = ImmutableArray<ScrewRecipeDto>.Empty;
    private readonly List<ScrewCycleRecord> _screwRecords = new();
    private int _currentIndex;
    private string? _resolvedImagePath;
    private double _boardWidth;
    private double _boardHeight;
    private bool _isRework;
    private string? _lastErrorMessage;

    public OperatorSessionController(
        IMesClient mesClient,
        ITemplateLayoutLoader templateLoader,
        ILockStationHardware hardware,
        ICurveArchive curveArchive,
        ILockSessionRepository checkpointStore,
        IOutboundMesQueue outbox,
        ICurrentUser currentUser,
        IOptions<AutoScrewAppOptions> options,
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
        _logger = logger;
    }

    public JobSessionPhase Phase => _phase;

    public string? SerialNumber => _serialNumber;

    public string? PartNumber => _partNumber;

    public string? LastErrorMessage => _lastErrorMessage;

    public int CurrentScrewIndex => _currentIndex;

    public bool IsRework => _isRework;

    public IReadOnlyList<ScrewPosition> Positions => _positions;

    public IReadOnlyList<StationScrewState> ScrewStates => _states;

    public string? ResolvedProductImagePath => _resolvedImagePath;

    public double BoardWidth => _boardWidth;

    public double BoardHeight => _boardHeight;

    public IReadOnlyList<TorqueAngleSample> LastTighteningSamples { get; private set; } = Array.Empty<TorqueAngleSample>();

    public void RequestScanDialog()
    {
        _lastErrorMessage = null;
        if (!TryApply(JobSessionTrigger.RequestScan))
            throw new InvalidOperationException($"Cannot open scan from {_phase}.");
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
            return;
        }

        _serialNumber = serialNumber.Trim();
        _partNumber = validation.PartNumber!;
        if (!TryApply(JobSessionTrigger.SnValidated))
            throw new InvalidOperationException("State error after SN validation.");

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
                return;
            }

            var layout = await _templateLoader.LoadAsync(templatePath, cancellationToken).ConfigureAwait(false);
            _boardWidth = layout.Raw.BoardWidth;
            _boardHeight = layout.Raw.BoardHeight;
            _resolvedImagePath = layout.ResolvedProductImagePath;
            _positions = layout.Positions.ToImmutableArray();

            var states = new StationScrewState[_positions.Length];
            for (var i = 0; i < states.Length; i++)
                states[i] = StationScrewState.Pending;
            _states = states.ToImmutableArray();

            _programs = BuildPrograms(recipe, _positions.Length);
            _recipeScrews = recipe.Screws.ToImmutableArray();
            _screwRecords.Clear();

            if (!TryApply(JobSessionTrigger.RecipeLoaded))
            {
                _lastErrorMessage = "State machine rejected RecipeLoaded.";
                ClearSession();
                return;
            }

            _currentIndex = NextPendingIndex(0);
            await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadRecipe failed");
            _lastErrorMessage = ex.Message;
            TryApply(JobSessionTrigger.LoadFailed);
            ClearSession();
        }
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
        SetState(idx, StationScrewState.InProgress);
        await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);

        await _hardware.PickScrewAsync(cancellationToken).ConfigureAwait(false);

        var dto = _recipeScrews.FirstOrDefault(s => s.PositionIndex == idx + 1)
                  ?? _recipeScrews.ElementAtOrDefault(idx);
        var paramId = dto?.ControllerParameterId ?? idx + 1;
        var tighteningContext = new TighteningContext(idx + 1, paramId);

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
            .SaveCurveCsvAsync(_serialNumber!, idx + 1, samples, cancellationToken)
            .ConfigureAwait(false);

        var finalTorque = device?.FinalTorqueNm ?? (samples.Count > 0 ? samples[^1].TorqueNm : (double?)null);
        var finalAngle = device?.FinalAngleDeg ?? (samples.Count > 0 ? samples[^1].AngleDeg : (double?)null);
        string? errorCode = null;
        if (!combinedOk)
            errorCode = !deviceOk
                ? device?.DeviceErrorCode?.ToString() ?? "DEVICE_NG"
                : eval.ErrorCode;

        _screwRecords.Add(new ScrewCycleRecord(idx + 1, combinedOk, errorCode, finalTorque, finalAngle, curvePath));

        if (combinedOk)
        {
            SetState(idx, StationScrewState.Ok);
            _currentIndex = NextPendingIndex(idx + 1);
            if (_currentIndex < 0)
            {
                if (TryApply(JobSessionTrigger.AllScrewsComplete))
                    await CompleteSessionAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        else
        {
            SetState(idx, StationScrewState.Ng);
            TryApply(JobSessionTrigger.ScrewNg);
            _lastErrorMessage = !deviceOk
                ? $"Device NG (code {device?.DeviceErrorCode})"
                : eval.Message ?? eval.ErrorCode;
            await LogErrorAsync(idx, eval, device, cancellationToken).ConfigureAwait(false);
        }

        await PersistCheckpointAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task CompleteSessionAsync(CancellationToken cancellationToken)
    {
        var screws = new List<ScrewResultDto>(_positions.Length);
        for (var i = 0; i < _positions.Length; i++)
        {
            var st = _states[i];
            var record = _screwRecords.FirstOrDefault(r => r.PositionIndex == i + 1);
            screws.Add(new ScrewResultDto
            {
                PositionIndex = i + 1,
                Result = st == StationScrewState.Ok ? "OK" : st == StationScrewState.Ng ? "NG" : "SKIPPED",
                ErrorCode = record?.ErrorCode ?? (st == StationScrewState.Ng ? "NG" : null),
                FinalTorqueNm = record?.FinalTorqueNm,
                FinalAngleDeg = record?.FinalAngleDeg,
                CurveRelativePath = record?.CurveRelativePath
            });
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
            LockLogJson = JsonSerializer.Serialize(new { note = "minimal lock log v0" })
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
        int PositionIndex,
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
    }

    public void ResetToIdle()
    {
        TryApply(JobSessionTrigger.ResetToIdle);
        ClearSession();
    }

    public void AbortToIdle()
    {
        TryApply(JobSessionTrigger.Abort);
        ClearSession();
    }

    private void ClearSession()
    {
        _serialNumber = null;
        _partNumber = null;
        _positions = ImmutableArray<ScrewPosition>.Empty;
        _states = ImmutableArray<StationScrewState>.Empty;
        _programs = ImmutableArray<SegmentedTorqueProgram>.Empty;
        _recipeScrews = ImmutableArray<ScrewRecipeDto>.Empty;
        _screwRecords.Clear();
        _currentIndex = 0;
        _resolvedImagePath = null;
        _boardWidth = 0;
        _boardHeight = 0;
        _lastErrorMessage = null;
        LastTighteningSamples = Array.Empty<TorqueAngleSample>();
    }

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
    }

    private async Task PersistCheckpointAsync(CancellationToken cancellationToken)
    {
        if (_serialNumber is null)
            return;

        var data = new SessionCheckpointData(_phase, _serialNumber, _partNumber ?? "", _currentIndex, _states.ToList(), DateTimeOffset.UtcNow);
        await _checkpointStore.SaveCheckpointAsync(data, cancellationToken).ConfigureAwait(false);
    }
}
