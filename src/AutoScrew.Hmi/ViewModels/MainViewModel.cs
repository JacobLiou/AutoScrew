using AutoScrew.Application.Services;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.ViewModels.Operation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AutoScrew.Hmi.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const int MaxLogEntries = 50;
    private readonly OperatorSessionController _session;
    private readonly LocalizationService _localization;

    public MainViewModel(OperatorSessionController session, LocalizationService localization)
    {
        _session = session;
        _localization = localization;
        _session.Changed += (_, _) =>
        {
            RefreshFromSession();
            NotifyCommandStates();
        };
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

    public ObservableCollection<ScrewMarkerVm> Markers { get; } = new();

    public ObservableCollection<string> ActivityLog { get; } = new();

    public OperatorSurfaceNodeViewModel? ActiveSurfaceNode { get; private set; }

    private bool CanOpenScan() =>
        _session.Phase is JobSessionPhase.Idle or JobSessionPhase.SnRejected;

    private bool CanSubmitSn() =>
        _session.Phase is JobSessionPhase.SnPending or JobSessionPhase.SnRejected
        && !string.IsNullOrWhiteSpace(SerialNumberInput);

    private bool CanRunScrew() =>
        _session.Phase == JobSessionPhase.Running
        && _session.CurrentScrewIndex >= 0;

    private bool CanConfirmFlip() => _session.Phase == JobSessionPhase.AwaitFlip;

    private bool CanUnlockNg() => _session.Phase == JobSessionPhase.NgLocked;

    [RelayCommand(CanExecute = nameof(CanOpenScan))]
    private void OpenScan()
    {
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
        try
        {
            var surfaceName = _session.ActiveSurfaceName ?? _session.ActiveSurfaceId ?? Loc.Get("S.Operation.CurrentSurface");
            var screwNo = _session.CurrentScrewLocalIndex;
            StatusMessage = Loc.Get("S.Operation.StatusPickTighten");
            await _session.RunCurrentScrewCycleAsync().ConfigureAwait(true);

            if (_session.LastErrorMessage is not null)
            {
                StatusMessage = _session.LastErrorMessage;
                AddLog(Loc.Format("S.Operation.LogNg", DateTime.Now.ToString("HH:mm:ss"), surfaceName, screwNo, _session.LastErrorMessage));
            }
            else
            {
                StatusMessage = Loc.Get("S.Operation.StatusStepDone");
                AddLog(Loc.Format("S.Operation.LogOk", DateTime.Now.ToString("HH:mm:ss"), surfaceName, screwNo));
                CurveChanged?.Invoke(this, EventArgs.Empty);
            }

            if (_session.Phase == JobSessionPhase.AwaitFlip)
                await PromptAndConfirmFlipAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanConfirmFlip))]
    private async Task ConfirmFlipAsync()
    {
        await PromptAndConfirmFlipAsync().ConfigureAwait(true);
    }

    [RelayCommand(CanExecute = nameof(CanUnlockNg))]
    private void UnlockNg()
    {
        try
        {
            _session.UnlockNgContinue();
            StatusMessage = Loc.Get("S.Operation.StatusUnlocked");
            AddLog($"{DateTime.Now:HH:mm:ss} {Loc.Get("S.Operation.LogUnlock")}");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void ResetSession()
    {
        if (_session.Phase != JobSessionPhase.Idle
            && _session.Phase != JobSessionPhase.SnPending
            && !ConfirmTips.ShowDialog(Loc.Get("S.Dialog.AbortSession"), System.Windows.Application.Current.MainWindow))
            return;

        _session.ResetToIdle();
        SerialNumberInput = "";
        ActivityLog.Clear();
        IsSnInputEnabled = true;
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
