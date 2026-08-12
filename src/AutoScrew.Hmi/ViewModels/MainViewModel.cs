using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.ViewModels.Operation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace AutoScrew.Hmi.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly OperatorSessionController _session;
    private readonly IProcessChangeoverService _changeover;
    private readonly LocalizationService _localization;
    private readonly IUserAuditService _audit;
    private readonly IOperationActivityLogService _activityLog;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly IOptions<SimulationOptions> _simulationOptions;
    private readonly ICurrentUser _user;
    private readonly SemaphoreSlim _autoRunGate = new(1, 1);
    private bool _localCycleInProgress;
    private JobSessionPhase _previousPhase = JobSessionPhase.Idle;

    public MainViewModel(
        OperatorSessionController session,
        IProcessChangeoverService changeover,
        LocalizationService localization,
        IUserAuditService audit,
        IOperationActivityLogService activityLog,
        IOptions<AutoScrewAppOptions> appOptions,
        IOptions<SimulationOptions> simulationOptions,
        ICurrentUser user)
    {
        _session = session;
        _changeover = changeover;
        _localization = localization;
        _audit = audit;
        _activityLog = activityLog;
        _appOptions = appOptions;
        _simulationOptions = simulationOptions;
        _user = user;
        _session.Changed += OnSessionChanged;
        _session.TighteningProgress += OnTighteningProgress;
        _session.ScrewCycleProgress += OnScrewCycleProgress;
        _localization.CultureChanged += (_, _) => RefreshFromSession();
        ProgressTreeRoot = new OperatorProgressRootViewModel();
        ProgressTreeRoots.Add(ProgressTreeRoot);
        GuideHint = BuildGuideHint();
        RefreshDeviceProcessPn();
    }

    public OperatorSessionController Session => _session;

    public OperatorProgressRootViewModel ProgressTreeRoot { get; }

    public ObservableCollection<OperatorProgressRootViewModel> ProgressTreeRoots { get; } = new();

    public event EventHandler? CurveChanged;

    public event EventHandler? RequestSelectActiveSurface;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(
        nameof(SubmitSnCommand),
        nameof(OpenScanCommand),
        nameof(ConfirmFlipCommand),
        nameof(RunCurrentScrewCommand))]
    private string _serialNumberInput = "";

    [ObservableProperty]
    private string _phaseDisplay = JobSessionPhase.Idle.ToString();

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private string _deviceProcessPnText = "";

    [ObservableProperty]
    private string _guideHint = "";

    [ObservableProperty]
    private string? _productImagePath;

    [ObservableProperty]
    private double _boardWidth;

    [ObservableProperty]
    private double _boardHeight;

    [ObservableProperty]
    private bool _isSnInputEnabled = true;

    [ObservableProperty]
    private bool _isCompletionVisible;

    [ObservableProperty]
    private string _completionTitle = "";

    [ObservableProperty]
    private string _completionSummary = "";

    [ObservableProperty]
    private string _completionStats = "";

    [ObservableProperty]
    private string _completionHint = "";
    [ObservableProperty]
    private bool _isNgOverlayVisible;

    [ObservableProperty]
    private string _ngErrorCode = "";

    [ObservableProperty]
    private string _ngErrorMessage = "";

    [ObservableProperty]
    private string _ngErrorAdvice = "";

    [ObservableProperty]
    private string _ngOverlayTitle = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(
        nameof(SubmitSnCommand),
        nameof(OpenScanCommand),
        nameof(ConfirmFlipCommand),
        nameof(RunCurrentScrewCommand))]
    private bool _isOperationLocked;

    public bool ShowRunScrewButton =>
        !_appOptions.Value.AutoRunScrewCycle
        || (_appOptions.Value.ShowManualRunScrewButton && _user.Role >= UserRole.Technician)
        || (_appOptions.Value.UseSimulatedHardware && _session.Phase == JobSessionPhase.Running);

    public bool CanUnlockNgOverlay =>
        _session.Phase == JobSessionPhase.NgLocked && _user.CanUnlockNg;

    public bool IsOperatorRole => _user.Role == UserRole.Operator;

    public bool ShowTechnicianNgActions =>
        _session.Phase == JobSessionPhase.NgLocked && _user.Role >= UserRole.Technician;

    public bool ShowOperatorNgActions =>
        _session.Phase == JobSessionPhase.NgLocked && _user.Role == UserRole.Operator;

    public bool IsReworkMode => _session.IsRework;

    public ObservableCollection<ScrewMarkerVm> Markers { get; } = new();

    public ReadOnlyObservableCollection<OperationActivityLogEntry> ActivityLog => _activityLog.Entries;

    public OperatorSurfaceNodeViewModel? ActiveSurfaceNode { get; private set; }

    private void OnTighteningProgress(object? sender, EventArgs e) =>
        CurveChanged?.Invoke(this, EventArgs.Empty);

    private void OnSessionChanged(object? sender, EventArgs e) =>
        RunOnUiThread(ApplySessionChanged);

    private void OnScrewCycleProgress(object? sender, ScrewCycleProgressEventArgs e) =>
        RunOnUiThread(() => LogScrewCycleProgress(e));

    private void ApplySessionChanged()
    {
        var previousPhase = _previousPhase;
        RefreshFromSession();
        LogPhaseTransitionIfNeeded(previousPhase, _session.Phase);
        NotifyCommandStates();
        OnPropertyChanged(nameof(ShowRunScrewButton));
        OnPropertyChanged(nameof(CanUnlockNgOverlay));
        OnPropertyChanged(nameof(IsOperatorRole));
        OnPropertyChanged(nameof(ShowTechnicianNgActions));
        OnPropertyChanged(nameof(ShowOperatorNgActions));
        OnPropertyChanged(nameof(IsReworkMode));
        _ = TryAutoRunScrewCycleAsync();
    }

    private static void RunOnUiThread(Action action)
    {
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            action();
        else
            dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
    }

    private bool CanOpenScan() =>
        !IsOperationLocked
        && _session.Phase is JobSessionPhase.Idle or JobSessionPhase.SnRejected;

    private bool CanSubmitSn() =>
        !IsOperationLocked
        && _session.Phase is JobSessionPhase.Idle
            or JobSessionPhase.SnPending
            or JobSessionPhase.SnRejected
            or JobSessionPhase.Completed
            or JobSessionPhase.Running
            or JobSessionPhase.AwaitFlip
            or JobSessionPhase.NgLocked
        && !string.IsNullOrWhiteSpace(SerialNumberInput);

    private bool CanRunScrew() =>
        ShowRunScrewButton
        && !_localCycleInProgress
        && !_session.IsCycleInProgress
        && _session.Phase == JobSessionPhase.Running
        && _session.CurrentScrewIndex >= 0;

    private bool CanConfirmFlip() => !IsOperationLocked && _session.Phase == JobSessionPhase.AwaitFlip;

    private bool CanUnlockNg() => _session.Phase == JobSessionPhase.NgLocked && _user.CanUnlockNg;

    private bool CanEnterRework() =>
        _session.Phase == JobSessionPhase.NgLocked && _user.Role >= UserRole.Technician;

    private bool CanEmergencyUnlockNg() =>
        _session.Phase == JobSessionPhase.NgLocked && _user.Role == UserRole.Operator;

    [RelayCommand(CanExecute = nameof(CanOpenScan))]
    private void OpenScan()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Operation, "Operation.OpenScan");
        try
        {
            _session.RequestScanDialog();
            IsSnInputEnabled = true;
            StatusMessage = Loc.Get("S.Operation.StatusEnterSn");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSubmitSn))]
    private async Task SubmitSnAsync()
    {
        var inputSn = SerialNumberInput.Trim();
        AuditHelper.Log(
            _audit,
            _appOptions,
            _user,
            AuditCategory.Operation,
            "Operation.SubmitSn",
            detail: $"sn={inputSn}");
        try
        {
            if (_session.IsActiveJobPhase)
            {
                if (string.Equals(_session.SerialNumber, inputSn, StringComparison.OrdinalIgnoreCase))
                {
                    StatusMessage = Loc.Get("S.Operation.ActiveJobSameSn");
                    MessageTips.ShowDialog(
                        Loc.Get("S.Operation.ActiveJobSameSn"),
                        System.Windows.Application.Current?.MainWindow,
                        Loc.Get("S.Operation.ActiveJobTitle"));
                    return;
                }

                MessageTips.ShowDialog(
                    Loc.Format("S.Operation.ActiveJobMustReset", _session.SerialNumber ?? "—", inputSn),
                    System.Windows.Application.Current?.MainWindow,
                    Loc.Get("S.Operation.ActiveJobTitle"));
                StatusMessage = Loc.Get("S.Operation.ActiveJobBlocked");
                return;
            }

            EnsureAwaitingSn();
            StatusMessage = Loc.Get("S.Operation.StatusValidating");
            var accept = await _session.AcceptSerialNumberAsync(inputSn).ConfigureAwait(true);
            if (!accept.Accepted)
            {
                StatusMessage = accept.ErrorMessage switch
                {
                    "ActiveJobMustReset" => Loc.Get("S.Operation.ActiveJobBlocked"),
                    "ActiveJobSameSn" => Loc.Get("S.Operation.ActiveJobSameSn"),
                    _ => accept.ErrorMessage ?? Loc.Get("S.Operation.GuideSnRejected"),
                };
                return;
            }

            var pn = accept.PartNumber!;
            var decision = await _changeover.EvaluateAsync(pn).ConfigureAwait(true);
            if (decision.NeedsChangeover)
            {
                if (!ConfirmChangeover(decision))
                {
                    _session.AbortAcceptedSerial(Loc.Get("S.Operation.ChangeoverCancelled"));
                    StatusMessage = Loc.Get("S.Operation.ChangeoverCancelled");
                    AuditHelper.Log(
                        _audit,
                        _appOptions,
                        _user,
                        AuditCategory.Operation,
                        "Operation.ChangeoverPrompt",
                        detail: $"cancelled;new={decision.NewProductPn};old={decision.PreviousProductPn}",
                        success: false);
                    return;
                }

                AuditHelper.Log(
                    _audit,
                    _appOptions,
                    _user,
                    AuditCategory.Operation,
                    "Operation.ChangeoverPrompt",
                    detail: $"confirmed;reason={decision.Reason};new={decision.NewProductPn};old={decision.PreviousProductPn}");

                StatusMessage = Loc.Get("S.Operation.ChangeoverDeploying");
                AddLog(Loc.Format("S.Operation.LogChangeoverDeploy", decision.NewProductPn));
                try
                {
                    await _changeover.DeployAndCommitAsync(pn).ConfigureAwait(true);
                    RefreshDeviceProcessPn();
                    AuditHelper.Log(
                        _audit,
                        _appOptions,
                        _user,
                        AuditCategory.Operation,
                        "Operation.ChangeoverDeploy",
                        detail: $"pn={pn};updatedUtc={decision.LibraryUpdatedUtc}");
                }
                catch (Exception ex)
                {
                    _session.AbortAcceptedSerial(ex.Message);
                    StatusMessage = ex.Message;
                    AuditHelper.Log(
                        _audit,
                        _appOptions,
                        _user,
                        AuditCategory.Operation,
                        "Operation.ChangeoverDeploy",
                        detail: $"pn={pn};error={ex.Message}",
                        success: false);
                    return;
                }
            }
            else
            {
                AuditHelper.Log(
                    _audit,
                    _appOptions,
                    _user,
                    AuditCategory.Operation,
                    "Operation.ChangeoverSkipped",
                    detail: $"pn={pn}");
            }

            var memory = await _session.TryGetRestorableMemoryAsync(inputSn).ConfigureAwait(true);
            if (memory is not null)
            {
                var restore = ConfirmTips.ShowDialog(
                    Loc.Format(
                        "S.Operation.RestoreMemoryPrompt",
                        memory.SerialNumber,
                        memory.PartNumber,
                        memory.Phase,
                        memory.CompletedScrewCount,
                        memory.TotalScrewCount),
                    System.Windows.Application.Current?.MainWindow,
                    Loc.Get("S.Operation.RestoreMemoryTitle"));
                if (restore)
                {
                    StatusMessage = Loc.Get("S.Operation.StatusRestoring");
                    AddLog(Loc.Format("S.Operation.LogRestoringMemory", memory.SerialNumber));
                    await _session.ContinueRestoreAfterSerialAcceptedAsync(inputSn).ConfigureAwait(true);
                    if (_session.IsActiveJobPhase || _session.Phase == JobSessionPhase.Running)
                    {
                        StatusMessage = Loc.Format("S.Operation.StatusRestored", _session.SerialNumber!);
                        AddLog(Loc.Format("S.Operation.LogRestored", _session.SerialNumber!));
                        RefreshFromSession();
                    }
                    else
                    {
                        StatusMessage = _session.LastErrorMessage ?? Loc.Get("S.Operation.RestoreFailed");
                    }

                    return;
                }

                AuditHelper.Log(
                    _audit,
                    _appOptions,
                    _user,
                    AuditCategory.Operation,
                    "Operation.RestoreMemoryDeclined",
                    detail: $"sn={inputSn}");
            }

            StatusMessage = Loc.Get("S.Operation.LogLoadingRecipe");
            AddLog(Loc.Get("S.Operation.LogLoadingRecipe"));
            await _session.ContinueAfterSerialAcceptedAsync().ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(_session.LastErrorMessage) &&
                _session.Phase != JobSessionPhase.Running)
                StatusMessage = _session.LastErrorMessage;
            else if (_session.Phase == JobSessionPhase.Running)
            {
                StatusMessage = Loc.Get("S.Operation.StatusRecipeLoaded");
                AddLog(Loc.Format(
                    "S.Operation.LogSnLoaded",
                    _session.SerialNumber!,
                    _session.PartNumber!,
                    _session.TemplateSurfaceCount));
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            RefreshSnInputEnabled();
            NotifyCommandStates();
        }
    }

    private void RefreshSnInputEnabled() =>
        IsSnInputEnabled = _session.Phase is JobSessionPhase.SnPending
            or JobSessionPhase.SnRejected
            or JobSessionPhase.Idle
            or JobSessionPhase.Completed
            or JobSessionPhase.Running
            or JobSessionPhase.AwaitFlip
            or JobSessionPhase.NgLocked;

    private bool ConfirmChangeover(ChangeoverDecision decision)
    {
        var title = Loc.Get("S.Operation.ChangeoverTitle");
        var body = decision.Reason switch
        {
            ChangeoverReason.FirstDeploy => Loc.Format(
                "S.Operation.ChangeoverBodyFirst",
                decision.NewProductPn),
            ChangeoverReason.ProductPnChanged => Loc.Format(
                "S.Operation.ChangeoverBodyPnChanged",
                decision.PreviousProductPn ?? "—",
                decision.NewProductPn),
            ChangeoverReason.ProcessVersionChanged => Loc.Format(
                "S.Operation.ChangeoverBodyVersion",
                decision.NewProductPn),
            ChangeoverReason.ProductMissing => Loc.Format(
                "S.Operation.ChangeoverBodyMissing",
                decision.NewProductPn),
            _ => Loc.Format(
                "S.Operation.ChangeoverBodyPnChanged",
                decision.PreviousProductPn ?? "—",
                decision.NewProductPn),
        };

        return ConfirmTips.ShowDialog(
            body,
            System.Windows.Application.Current?.MainWindow,
            title);
    }

    private void RefreshDeviceProcessPn()
    {
        var state = _changeover.GetStationState();
        DeviceProcessPnText = state is null || string.IsNullOrWhiteSpace(state.ProductPn)
            ? Loc.Get("S.Operation.DeviceProcessPnNone")
            : Loc.Format("S.Operation.DeviceProcessPn", state.ProductPn);
    }

    private void EnsureAwaitingSn()
    {
        switch (_session.Phase)
        {
            case JobSessionPhase.Idle:
                _session.RequestScanDialog();
                break;
            case JobSessionPhase.Completed:
                _session.ResetToIdle();
                _session.RequestScanDialog();
                break;
        }
    }

    [RelayCommand(CanExecute = nameof(CanRunScrew))]
    private async Task RunCurrentScrewAsync()
    {
        await ExecuteScrewCycleAsync(manualTrigger: true).ConfigureAwait(true);
    }

    [RelayCommand(CanExecute = nameof(CanConfirmFlip))]
    private async Task ConfirmFlipAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Operation, "Operation.ConfirmFlip", serialNumber: _session.SerialNumber);
        await PromptAndConfirmFlipAsync().ConfigureAwait(true);
    }

    [RelayCommand(CanExecute = nameof(CanUnlockNg))]
    private async Task UnlockNgAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Operation, "Operation.UnlockNg", serialNumber: _session.SerialNumber);
        try
        {
            await _session.UnlockNgContinueAsync().ConfigureAwait(true);
            AfterNgUnlocked(Loc.Get("S.Operation.LogUnlock"));
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanEnterRework))]
    private async Task EnterReworkAsync()
    {
        AuditHelper.Log(
            _audit,
            _appOptions,
            _user,
            AuditCategory.Operation,
            "Operation.EnterRework",
            serialNumber: _session.SerialNumber);
        try
        {
            await _session.BeginReworkAndUnlockAsync().ConfigureAwait(true);
            AfterNgUnlocked(Loc.Get("S.Operation.LogEnterRework"));
            OnPropertyChanged(nameof(IsReworkMode));
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanEmergencyUnlockNg))]
    private async Task EmergencyUnlockNgAsync()
    {
        if (!EmergencyUnlockDialog.TryPrompt(
                out var reason,
                System.Windows.Application.Current.MainWindow,
                _user))
            return;

        try
        {
            await _session.EmergencyUnlockNgAsync(reason).ConfigureAwait(true);
            AfterNgUnlocked(Loc.Get("S.Operation.LogEmergencyUnlock"));
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void AfterNgUnlocked(string logMessage)
    {
        IsNgOverlayVisible = false;
        IsOperationLocked = false;
        StatusMessage = Loc.Get("S.Operation.StatusUnlocked");
        AddLog(logMessage);
        OnPropertyChanged(nameof(CanUnlockNgOverlay));
        OnPropertyChanged(nameof(ShowTechnicianNgActions));
        OnPropertyChanged(nameof(ShowOperatorNgActions));
        OnPropertyChanged(nameof(IsReworkMode));
    }

    [RelayCommand]
    private async Task ResetSessionAsync()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Operation, "Operation.ResetSession", serialNumber: _session.SerialNumber);
        if (_session.Phase != JobSessionPhase.Idle
            && _session.Phase != JobSessionPhase.SnPending
            && !ConfirmTips.ShowDialog(
                Loc.Get("S.Dialog.AbortSessionPark"),
                System.Windows.Application.Current.MainWindow,
                Loc.Get("S.Dialog.AbortSessionTitle")))
            return;

        await _session.ResetToIdleAsync().ConfigureAwait(true);
        SerialNumberInput = "";
        _activityLog.ClearRecent();
        IsSnInputEnabled = true;
        IsNgOverlayVisible = false;
        IsOperationLocked = false;
        StatusMessage = Loc.Get("S.Operation.StatusResetParked");
        try
        {
            _session.RequestScanDialog();
        }
        catch
        {
            // ignore
        }

        CurveChanged?.Invoke(this, EventArgs.Empty);
        NotifyCommandStates();
    }

    public async Task TryRestoreCheckpointOnStartupAsync()
    {
        var offer = await _session.GetCheckpointRestoreOfferAsync().ConfigureAwait(true);
        if (offer is null)
        {
            EnsureScanReady();
            return;
        }

        var message = Loc.Format(
            "S.Operation.RestoreMemoryPrompt",
            offer.SerialNumber,
            offer.PartNumber,
            offer.Phase,
            offer.CompletedScrewCount,
            offer.TotalScrewCount);
        if (ConfirmTips.ShowDialog(message, System.Windows.Application.Current.MainWindow, Loc.Get("S.Operation.RestoreMemoryTitle")))
        {
            StatusMessage = Loc.Get("S.Operation.StatusRestoring");
            var ok = await _session.RestoreFromCheckpointAsync().ConfigureAwait(true);
            if (ok)
            {
                SerialNumberInput = _session.SerialNumber ?? "";
                StatusMessage = Loc.Format("S.Operation.StatusRestored", _session.SerialNumber!);
                AddLog(Loc.Format("S.Operation.LogRestored", _session.SerialNumber!));
                RefreshFromSession();
                return;
            }

            StatusMessage = _session.LastErrorMessage ?? Loc.Get("S.Operation.RestoreFailed");
        }
        else
        {
            await _session.DiscardCheckpointAsync().ConfigureAwait(true);
        }

        EnsureScanReady();
    }

    public void EnsureScanReady()
    {
        try
        {
            if (_session.Phase == JobSessionPhase.Idle)
                _session.RequestScanDialog();
            RefreshSnInputEnabled();
            StatusMessage = Loc.Get("S.Operation.StatusEnterSn");
        }
        catch
        {
            // ignore
        }
    }

    public void RefreshFromSession()
    {
        PhaseDisplay = _session.Phase.ToString();
        GuideHint = BuildGuideHint();
        ProductImagePath = _session.ResolvedProductImagePath;
        BoardWidth = _session.BoardWidth;
        BoardHeight = _session.BoardHeight;
        RefreshDeviceProcessPn();
        RefreshSnInputEnabled();
        RefreshCompletionState();

        if (_session.Phase == JobSessionPhase.Completed)
            StatusMessage = Loc.Get("S.Operation.GuideCompleted");

        var ngLocked = _session.Phase == JobSessionPhase.NgLocked;
        if (ngLocked)
        {
            if (_previousPhase != JobSessionPhase.NgLocked)
            {
                NgErrorCode = _session.LastErrorCode ?? "";
                NgErrorMessage = _session.LastErrorMessage ?? Loc.Get("S.Operation.GuideNgLocked");
                NgErrorAdvice = ScrewNgAdvisor.GetAdvice(_session.LastErrorCode);
            }

            NgOverlayTitle = ScrewNgAdvisor.IsFeedError(_session.LastErrorCode)
                ? Loc.Get("S.Operation.NgFeedTitle")
                : Loc.Get("S.Operation.NgScrewTitle");
        }

        IsNgOverlayVisible = ngLocked;
        IsOperationLocked = ngLocked;
        _previousPhase = _session.Phase;
        OnPropertyChanged(nameof(CanUnlockNgOverlay));

        Markers.Clear();
        var positions = _session.Positions;
        var states = _session.ScrewStates;
        var nextIndex = _session.Phase == JobSessionPhase.Running ? _session.CurrentScrewIndex : -1;
        for (var i = 0; i < positions.Count; i++)
        {
            var p = positions[i];
            var diameter = p.CircleDiameterPx ?? 26;
            var st = i < states.Count ? states[i] : StationScrewState.Pending;
            var isNextTarget = nextIndex >= 0 && i == nextIndex && st == StationScrewState.Pending;
            Markers.Add(new ScrewMarkerVm(p.Index, p.CenterX, p.CenterY, diameter, st, isNextTarget));
        }

        RefreshProgressTree();
    }

    private void RefreshCompletionState()
    {
        var completed = _session.Phase == JobSessionPhase.Completed;
        IsCompletionVisible = completed;

        if (!completed)
        {
            CompletionTitle = string.Empty;
            CompletionSummary = string.Empty;
            CompletionStats = string.Empty;
            CompletionHint = string.Empty;
            return;
        }

        var total = _session.ScrewStates.Count;
        var ok = _session.ScrewStates.Count(static state => state == StationScrewState.Ok);
        var ng = _session.ScrewStates.Count(static state => state == StationScrewState.Ng);
        var pending = _session.ScrewStates.Count(static state => state == StationScrewState.Pending);
        var resultKey = ng == 0
            ? "S.Operation.CompletionResultOk"
            : "S.Operation.CompletionResultNg";

        CompletionTitle = Loc.Get("S.Operation.CompletionTitle");
        CompletionSummary = Loc.Format(
            "S.Operation.CompletionSummary",
            _session.SerialNumber ?? "—",
            _session.PartNumber ?? "—");
        CompletionStats = Loc.Format("S.Operation.CompletionStats", ok, ng, pending, total);
        CompletionHint = Loc.Format(
            "S.Operation.CompletionHint",
            Loc.Get(resultKey));
    }

    private async Task TryAutoRunScrewCycleAsync()
    {
        if (!_appOptions.Value.AutoRunScrewCycle || !CanAutoRunCurrentPendingScrew())
            return;

        await _autoRunGate.WaitAsync().ConfigureAwait(true);
        try
        {
            while (CanAutoRunCurrentPendingScrew())
            {
                await ExecuteScrewCycleAsync(manualTrigger: false).ConfigureAwait(true);

                if (!ShouldAutoChainNextScrew() || !CanAutoRunCurrentPendingScrew())
                    break;

                var betweenDelay = Math.Max(0, _simulationOptions.Value.BetweenScrewDelayMs);
                if (betweenDelay > 0)
                    await Task.Delay(betweenDelay).ConfigureAwait(true);
            }
        }
        finally
        {
            _autoRunGate.Release();
        }
    }

    private bool CanAutoRunCurrentPendingScrew()
    {
        if (!_appOptions.Value.AutoRunScrewCycle || _localCycleInProgress || _session.IsCycleInProgress)
            return false;

        if (_session.Phase != JobSessionPhase.Running || _session.CurrentScrewIndex < 0)
            return false;

        var idx = _session.CurrentScrewIndex;
        return idx < _session.ScrewStates.Count && _session.ScrewStates[idx] == StationScrewState.Pending;
    }

    private async Task ExecuteScrewCycleAsync(bool manualTrigger)
    {
        if (_localCycleInProgress || _session.IsCycleInProgress)
            return;

        _localCycleInProgress = true;
        NotifyCommandStates();

        AuditHelper.Log(
            _audit,
            _appOptions,
            _user,
            AuditCategory.Operation,
            manualTrigger ? "Operation.RunScrew" : "Operation.AutoRunScrew",
            detail: $"sn={_session.SerialNumber};screw={_session.CurrentScrewLocalIndex}",
            serialNumber: _session.SerialNumber);

        try
        {
            var surfaceName = ActiveSurfaceDisplayName();
            var screwNo = _session.CurrentScrewLocalIndex;
            StatusMessage = _appOptions.Value.UseSimulatedHardware
                ? Loc.Get("S.Operation.StatusPickTighten")
                : Loc.Get("S.Operation.StatusWaitTrigger");

            await _session.RunCurrentScrewCycleAsync().ConfigureAwait(true);

            if (_session.LastErrorMessage is not null)
            {
                StatusMessage = _session.LastErrorMessage;
                if (_session.Phase != JobSessionPhase.NgLocked)
                    CurveChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                StatusMessage = Loc.Get("S.Operation.StatusStepDone");
                CurveChanged?.Invoke(this, EventArgs.Empty);
            }

            if (_session.Phase == JobSessionPhase.AwaitFlip)
                await PromptAndConfirmFlipAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            _localCycleInProgress = false;
            NotifyCommandStates();
        }
    }

    private bool ShouldAutoChainNextScrew() =>
        _appOptions.Value.AutoChainNextScrew || _appOptions.Value.UseSimulatedHardware;

    private string BuildGuideHint()
    {
        return _session.Phase switch
        {
            JobSessionPhase.Idle => Loc.Get("S.Operation.GuideIdle"),
            JobSessionPhase.SnPending => Loc.Get("S.Operation.GuideSnPending"),
            JobSessionPhase.SnRejected => _session.LastErrorMessage ?? Loc.Get("S.Operation.GuideSnRejected"),
            JobSessionPhase.LoadingRecipe => Loc.Get("S.Operation.GuideLoading"),
            JobSessionPhase.Running when _session.CurrentScrewIndex >= 0 && _session.Positions.Count > 0 =>
                Loc.Format(
                    "S.Operation.GuideRunningScrew",
                    _session.ActiveSurfaceName ?? _session.ActiveSurfaceId ?? Loc.Get("S.Operation.CurrentSurface"),
                    _session.CurrentScrewLocalIndex),
            JobSessionPhase.Running =>
                Loc.Format(
                    "S.Operation.GuideRunningSurface",
                    _session.ActiveSurfaceName ?? _session.ActiveSurfaceId ?? Loc.Get("S.Operation.CurrentSurface")),
            JobSessionPhase.AwaitFlip => BuildAwaitFlipGuideHint(),
            JobSessionPhase.NgLocked =>
                _session.LastErrorMessage ?? Loc.Get("S.Operation.GuideNgLocked"),
            JobSessionPhase.Completed => Loc.Get("S.Operation.GuideCompleted"),
            _ => ""
        };
    }

    private string BuildAwaitFlipGuideHint()
    {
        var (_, completedName) = _session.GetCompletedSurfaceForFlip();
        var (_, nextName) = _session.GetPendingFlipTarget();
        var done = completedName ?? _session.ActiveSurfaceName ?? Loc.Get("S.Operation.CurrentSurface");
        var next = nextName ?? Loc.Get("S.Operation.NextSurface");
        return Loc.Format("S.Operation.GuideAwaitFlip", done, next);
    }

    [RelayCommand]
    private void StartNextSn()
    {
        try
        {
            SerialNumberInput = string.Empty;
            EnsureAwaitingSn();
            StatusMessage = Loc.Get("S.Operation.StatusEnterSn");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void RefreshProgressTree()
    {
        ProgressTreeRoot.SerialNumber = _session.SerialNumber ?? "";
        ProgressTreeRoot.PartNumber = _session.PartNumber ?? "";
        ProgressTreeRoot.DisplayLabel = string.IsNullOrWhiteSpace(_session.SerialNumber)
            ? Loc.Get("S.Operation.WaitScan")
            : Loc.Format("S.Operation.SnPnLabel", _session.SerialNumber!, _session.PartNumber!);
        ProgressTreeRoot.IsExpanded = true;

        ProgressTreeRoot.Surfaces.Clear();
        ActiveSurfaceNode = null;
        var activeOrdinal = _session.ActiveSurfaceOrdinal;
        var ordinal = 0;
        foreach (var snapshot in _session.SurfaceSnapshots)
        {
            var isActive = ordinal == activeOrdinal
                           && _session.Phase is JobSessionPhase.Running or JobSessionPhase.AwaitFlip or JobSessionPhase.NgLocked;
            var node = new OperatorSurfaceNodeViewModel(
                snapshot.SurfaceId,
                snapshot.Name,
                snapshot.Order,
                snapshot.ProgressState)
            {
                IsExpanded = true,
                IsActive = isActive
            };

            for (var i = 0; i < snapshot.ScrewStates.Count; i++)
            {
                var localIndex = i < snapshot.ScrewLocalIndices.Count
                    ? snapshot.ScrewLocalIndices[i]
                    : i + 1;
                string? partNo = null;
                if (isActive)
                    partNo = _session.Positions.FirstOrDefault(p => p.Index == localIndex)?.PartNumber;
                var label = string.IsNullOrWhiteSpace(partNo)
                    ? Loc.Format("S.Operation.ScrewNode", localIndex)
                    : Loc.Format("S.Operation.ScrewNodeWithPart", localIndex, partNo);
                node.Screws.Add(new OperatorScrewNodeViewModel(
                    localIndex,
                    snapshot.ScrewStates[i],
                    label));
            }

            if (isActive)
                ActiveSurfaceNode = node;

            ProgressTreeRoot.Surfaces.Add(node);
            ordinal++;
        }

        RequestSelectActiveSurface?.Invoke(this, EventArgs.Empty);
    }

    private async Task PromptAndConfirmFlipAsync()
    {
        var (completedId, completedName) = _session.GetCompletedSurfaceForFlip();
        var (_, nextName) = _session.GetPendingFlipTarget();
        var done = completedName ?? completedId ?? Loc.Get("S.Operation.CurrentSurface");
        var next = nextName ?? Loc.Get("S.Operation.NextSurface");
        var message = Loc.Format("S.Operation.FlipConfirm", done, next);

        if (!ConfirmTips.ShowDialog(message, System.Windows.Application.Current.MainWindow))
        {
            StatusMessage = Loc.Get("S.Operation.AwaitFlipStatus");
            return;
        }

        try
        {
            _session.ConfirmAdvanceToNextSurface();
            var surface = _session.ActiveSurfaceName ?? _session.ActiveSurfaceId ?? "";
            StatusMessage = Loc.Format("S.Operation.EnteredSurface", surface);
            AddLog(Loc.Format("S.Operation.LogFlip", surface));
            if (_session.CurrentScrewLocalIndex > 0)
            {
                AddLog(Loc.Format(
                    "S.Operation.LogStartSurface",
                    surface,
                    _session.CurrentScrewLocalIndex,
                    _session.Positions.Count));
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }

        await Task.CompletedTask;
    }

    private void AddLog(string line) =>
        _activityLog.Append(line, _session.SerialNumber);

    private string ActiveSurfaceDisplayName() =>
        _session.ActiveSurfaceName ?? _session.ActiveSurfaceId ?? Loc.Get("S.Operation.CurrentSurface");

    private void LogScrewCycleProgress(ScrewCycleProgressEventArgs e)
    {
        var surface = string.IsNullOrWhiteSpace(e.SurfaceName)
            ? ActiveSurfaceDisplayName()
            : e.SurfaceName;
        var screwNo = e.LocalScrewIndex;

        var message = e.Step switch
        {
            ScrewCycleProgressStep.Started =>
                Loc.Format("S.Operation.LogScrewStart", surface, screwNo),
            ScrewCycleProgressStep.Picking =>
                Loc.Format("S.Operation.LogScrewPicking", surface, screwNo),
            ScrewCycleProgressStep.PickCompleteWaitTrigger =>
                Loc.Format("S.Operation.LogScrewWaitTrigger", surface, screwNo),
            ScrewCycleProgressStep.Tightening =>
                Loc.Format("S.Operation.LogScrewTightening", surface, screwNo),
            ScrewCycleProgressStep.CompletedOk =>
                Loc.Format("S.Operation.LogOk", surface, screwNo),
            ScrewCycleProgressStep.CompletedNg =>
                Loc.Format("S.Operation.LogNg", surface, screwNo, e.ErrorMessage ?? e.ErrorCode ?? ""),
            ScrewCycleProgressStep.FeedFailed =>
                Loc.Format("S.Operation.LogFeedNg", surface, screwNo, e.ErrorMessage ?? e.ErrorCode ?? ""),
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(message))
            AddLog(message);
    }

    private void LogPhaseTransitionIfNeeded(JobSessionPhase from, JobSessionPhase to)
    {
        if (from == to)
            return;

        switch (to)
        {
            case JobSessionPhase.LoadingRecipe:
                break;
            case JobSessionPhase.Running when from is JobSessionPhase.LoadingRecipe or JobSessionPhase.SnPending:
                AddLog(Loc.Format(
                    "S.Operation.LogStartWork",
                    ActiveSurfaceDisplayName(),
                    _session.CurrentScrewLocalIndex,
                    _session.Positions.Count));
                break;
            case JobSessionPhase.Running when from == JobSessionPhase.NgLocked:
                AddLog(Loc.Format(
                    "S.Operation.LogResumeWork",
                    ActiveSurfaceDisplayName(),
                    _session.CurrentScrewLocalIndex));
                break;
            case JobSessionPhase.AwaitFlip:
            {
                var (completedId, completedName) = _session.GetCompletedSurfaceForFlip();
                var (_, nextName) = _session.GetPendingFlipTarget();
                var done = completedName ?? completedId ?? ActiveSurfaceDisplayName();
                var next = nextName ?? Loc.Get("S.Operation.NextSurface");
                AddLog(Loc.Format("S.Operation.LogSurfaceDone", done, _session.Positions.Count));
                AddLog(Loc.Format("S.Operation.LogAwaitFlip", done, next));
                break;
            }
            case JobSessionPhase.Completed:
                AddLog(Loc.Format("S.Operation.LogJobDone", _session.SerialNumber ?? ""));
                break;
            case JobSessionPhase.SnRejected when !string.IsNullOrWhiteSpace(_session.LastErrorMessage):
                AddLog(Loc.Format("S.Operation.LogSnRejected", _session.LastErrorMessage));
                break;
        }
    }

    private void NotifyCommandStates()
    {
        OpenScanCommand.NotifyCanExecuteChanged();
        SubmitSnCommand.NotifyCanExecuteChanged();
        RunCurrentScrewCommand.NotifyCanExecuteChanged();
        ConfirmFlipCommand.NotifyCanExecuteChanged();
        UnlockNgCommand.NotifyCanExecuteChanged();
        EnterReworkCommand.NotifyCanExecuteChanged();
        EmergencyUnlockNgCommand.NotifyCanExecuteChanged();
    }
}

public sealed partial class ScrewMarkerVm : ObservableObject
{
    public ScrewMarkerVm(
        int index,
        double centerX,
        double centerY,
        double diameterPx,
        StationScrewState state,
        bool isNextTarget = false)
    {
        Index = index;
        CenterX = centerX;
        CenterY = centerY;
        DiameterPx = diameterPx;
        _state = state;
        _isNextTarget = isNextTarget;
    }

    public int Index { get; }

    public double CenterX { get; }

    public double CenterY { get; }

    public double DiameterPx { get; }

    public double CanvasLeft => CenterX - DiameterPx / 2;

    public double CanvasTop => CenterY - DiameterPx / 2;

    [ObservableProperty]
    private StationScrewState _state;

    [ObservableProperty]
    private bool _isNextTarget;
}
