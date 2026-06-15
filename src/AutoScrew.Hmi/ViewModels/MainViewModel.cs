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
    private const int MaxLogEntries = 50;
    private readonly OperatorSessionController _session;
    private readonly LocalizationService _localization;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;
    private readonly SemaphoreSlim _autoRunGate = new(1, 1);
    private bool _localCycleInProgress;
    private JobSessionPhase _previousPhase = JobSessionPhase.Idle;

    public MainViewModel(
        OperatorSessionController session,
        LocalizationService localization,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _session = session;
        _localization = localization;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        _session.Changed += OnSessionChanged;
        _session.TighteningProgress += OnTighteningProgress;
        _localization.CultureChanged += (_, _) => RefreshFromSession();
        ProgressTreeRoot = new OperatorProgressRootViewModel();
        ProgressTreeRoots.Add(ProgressTreeRoot);
        GuideHint = BuildGuideHint();
    }

    public OperatorSessionController Session => _session;

    public OperatorProgressRootViewModel ProgressTreeRoot { get; }

    public ObservableCollection<OperatorProgressRootViewModel> ProgressTreeRoots { get; } = new();

    public event EventHandler? CurveChanged;

    public event EventHandler? RequestSelectActiveSurface;

    [ObservableProperty]
    private string _serialNumberInput = "";

    [ObservableProperty]
    private string _phaseDisplay = JobSessionPhase.Idle.ToString();

    [ObservableProperty]
    private string _statusMessage = "";

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
    private bool _isOperationLocked;

    public bool ShowRunScrewButton =>
        !_appOptions.Value.AutoRunScrewCycle
        || (_appOptions.Value.ShowManualRunScrewButton && _user.Role >= UserRole.Technician);

    public bool CanUnlockNgOverlay =>
        _session.Phase == JobSessionPhase.NgLocked && _user.CanUnlockNg;

    public ObservableCollection<ScrewMarkerVm> Markers { get; } = new();

    public ObservableCollection<string> ActivityLog { get; } = new();

    public OperatorSurfaceNodeViewModel? ActiveSurfaceNode { get; private set; }

    private void OnTighteningProgress(object? sender, EventArgs e) =>
        CurveChanged?.Invoke(this, EventArgs.Empty);

    private void OnSessionChanged(object? sender, EventArgs e)
    {
        RefreshFromSession();
        NotifyCommandStates();
        OnPropertyChanged(nameof(ShowRunScrewButton));
        OnPropertyChanged(nameof(CanUnlockNgOverlay));

        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            _ = TryAutoRunScrewCycleAsync();
        else
            dispatcher.BeginInvoke(DispatcherPriority.Background, () => _ = TryAutoRunScrewCycleAsync());
    }

    private bool CanOpenScan() =>
        !IsOperationLocked
        && _session.Phase is JobSessionPhase.Idle or JobSessionPhase.SnRejected;

    private bool CanSubmitSn() =>
        !IsOperationLocked
        && _session.Phase is JobSessionPhase.SnPending or JobSessionPhase.SnRejected
        && !string.IsNullOrWhiteSpace(SerialNumberInput);

    private bool CanRunScrew() =>
        ShowRunScrewButton
        && !_localCycleInProgress
        && !_session.IsCycleInProgress
        && _session.Phase == JobSessionPhase.Running
        && _session.CurrentScrewIndex >= 0;

    private bool CanConfirmFlip() => !IsOperationLocked && _session.Phase == JobSessionPhase.AwaitFlip;

    private bool CanUnlockNg() => _session.Phase == JobSessionPhase.NgLocked && _user.CanUnlockNg;

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
        AuditHelper.Log(
            _audit,
            _appOptions,
            _user,
            AuditCategory.Operation,
            "Operation.SubmitSn",
            detail: $"sn={SerialNumberInput.Trim()}");
        try
        {
            StatusMessage = Loc.Get("S.Operation.StatusValidating");
            await _session.SubmitSerialNumberAsync(SerialNumberInput).ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(_session.LastErrorMessage))
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

            IsSnInputEnabled = _session.Phase is JobSessionPhase.SnPending or JobSessionPhase.SnRejected;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
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
    private void UnlockNg()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Operation, "Operation.UnlockNg", serialNumber: _session.SerialNumber);
        try
        {
            _session.UnlockNgContinue();
            IsNgOverlayVisible = false;
            IsOperationLocked = false;
            StatusMessage = Loc.Get("S.Operation.StatusUnlocked");
            AddLog($"{DateTime.Now:HH:mm:ss} {Loc.Get("S.Operation.LogUnlock")}");
            OnPropertyChanged(nameof(CanUnlockNgOverlay));
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void ResetSession()
    {
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Operation, "Operation.ResetSession", serialNumber: _session.SerialNumber);
        if (_session.Phase != JobSessionPhase.Idle
            && _session.Phase != JobSessionPhase.SnPending
            && !ConfirmTips.ShowDialog(Loc.Get("S.Dialog.AbortSession"), System.Windows.Application.Current.MainWindow))
            return;

        _session.ResetToIdle();
        SerialNumberInput = "";
        ActivityLog.Clear();
        IsSnInputEnabled = true;
        IsNgOverlayVisible = false;
        IsOperationLocked = false;
        StatusMessage = Loc.Get("S.Operation.StatusReset");
        try
        {
            _session.RequestScanDialog();
        }
        catch
        {
            // ignore
        }

        CurveChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task TryRestoreCheckpointOnStartupAsync()
    {
        var offer = await _session.GetCheckpointRestoreOfferAsync().ConfigureAwait(true);
        if (offer is null)
        {
            EnsureScanReady();
            return;
        }

        var message = Loc.Format("S.Operation.RestoreCheckpointPrompt", offer.SerialNumber, offer.PartNumber, offer.Phase);
        if (ConfirmTips.ShowDialog(message, System.Windows.Application.Current.MainWindow, Loc.Get("S.Operation.RestoreCheckpointTitle")))
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
            IsSnInputEnabled = _session.Phase is JobSessionPhase.SnPending or JobSessionPhase.SnRejected;
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
        IsSnInputEnabled = _session.Phase is JobSessionPhase.SnPending or JobSessionPhase.SnRejected or JobSessionPhase.Idle;

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
        for (var i = 0; i < positions.Count; i++)
        {
            var p = positions[i];
            var diameter = p.CircleDiameterPx ?? 26;
            var st = i < states.Count ? states[i] : StationScrewState.Pending;
            Markers.Add(new ScrewMarkerVm(p.Index, p.CenterX, p.CenterY, diameter, st));
        }

        RefreshProgressTree();
    }

    private async Task TryAutoRunScrewCycleAsync()
    {
        if (!_appOptions.Value.AutoRunScrewCycle || _localCycleInProgress || _session.IsCycleInProgress)
            return;

        if (_session.Phase != JobSessionPhase.Running || _session.CurrentScrewIndex < 0)
            return;

        var idx = _session.CurrentScrewIndex;
        if (idx >= _session.ScrewStates.Count)
            return;

        if (_session.ScrewStates[idx] != StationScrewState.Pending)
            return;

        await _autoRunGate.WaitAsync().ConfigureAwait(true);
        try
        {
            if (!_appOptions.Value.AutoRunScrewCycle || _localCycleInProgress || _session.IsCycleInProgress)
                return;
            if (_session.Phase != JobSessionPhase.Running || _session.CurrentScrewIndex < 0)
                return;
            idx = _session.CurrentScrewIndex;
            if (idx >= _session.ScrewStates.Count || _session.ScrewStates[idx] != StationScrewState.Pending)
                return;

            await ExecuteScrewCycleAsync(manualTrigger: false).ConfigureAwait(true);
        }
        finally
        {
            _autoRunGate.Release();
        }
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
            var surfaceName = _session.ActiveSurfaceName ?? _session.ActiveSurfaceId ?? Loc.Get("S.Operation.CurrentSurface");
            var screwNo = _session.CurrentScrewLocalIndex;
            StatusMessage = _appOptions.Value.UseSimulatedHardware
                ? Loc.Get("S.Operation.StatusPickTighten")
                : Loc.Get("S.Operation.StatusWaitTrigger");

            await _session.RunCurrentScrewCycleAsync().ConfigureAwait(true);

            if (_session.LastErrorMessage is not null)
            {
                StatusMessage = _session.LastErrorMessage;
                AddLog(Loc.Format("S.Operation.LogNg", DateTime.Now.ToString("HH:mm:ss"), surfaceName, screwNo, _session.LastErrorMessage));
                if (_session.Phase != JobSessionPhase.NgLocked)
                    CurveChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                StatusMessage = Loc.Get("S.Operation.StatusStepDone");
                AddLog(Loc.Format("S.Operation.LogOk", DateTime.Now.ToString("HH:mm:ss"), surfaceName, screwNo));
                CurveChanged?.Invoke(this, EventArgs.Empty);

                if (_appOptions.Value.AutoChainNextScrew
                    && _session.Phase == JobSessionPhase.Running
                    && _session.CurrentScrewIndex >= 0
                    && _session.CurrentScrewIndex < _session.ScrewStates.Count
                    && _session.ScrewStates[_session.CurrentScrewIndex] == StationScrewState.Pending)
                {
                    await TryAutoRunScrewCycleAsync().ConfigureAwait(true);
                }
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
                IsExpanded = isActive,
                IsActive = isActive
            };

            for (var i = 0; i < snapshot.ScrewStates.Count; i++)
            {
                var localIndex = i < snapshot.ScrewLocalIndices.Count
                    ? snapshot.ScrewLocalIndices[i]
                    : i + 1;
                node.Screws.Add(new OperatorScrewNodeViewModel(
                    localIndex,
                    snapshot.ScrewStates[i],
                    Loc.Format("S.Operation.ScrewNode", localIndex)));
            }

            if (isActive)
                ActiveSurfaceNode = node;

            ProgressTreeRoot.Surfaces.Add(node);
            ordinal++;
        }

        if (ActiveSurfaceNode is not null)
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
            AddLog($"{DateTime.Now:HH:mm:ss} {Loc.Format("S.Operation.LogFlip", surface)}");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }

        await Task.CompletedTask;
    }

    private void AddLog(string line)
    {
        ActivityLog.Insert(0, line);
        while (ActivityLog.Count > MaxLogEntries)
            ActivityLog.RemoveAt(ActivityLog.Count - 1);
    }

    private void NotifyCommandStates()
    {
        OpenScanCommand.NotifyCanExecuteChanged();
        SubmitSnCommand.NotifyCanExecuteChanged();
        RunCurrentScrewCommand.NotifyCanExecuteChanged();
        ConfirmFlipCommand.NotifyCanExecuteChanged();
        UnlockNgCommand.NotifyCanExecuteChanged();
    }
}

public sealed partial class ScrewMarkerVm : ObservableObject
{
    public ScrewMarkerVm(int index, double centerX, double centerY, double diameterPx, StationScrewState state)
    {
        Index = index;
        CenterX = centerX;
        CenterY = centerY;
        DiameterPx = diameterPx;
        _state = state;
    }

    public int Index { get; }

    public double CenterX { get; }

    public double CenterY { get; }

    public double DiameterPx { get; }

    public double CanvasLeft => CenterX - DiameterPx / 2;

    public double CanvasTop => CenterY - DiameterPx / 2;

    [ObservableProperty]
    private StationScrewState _state;
}
