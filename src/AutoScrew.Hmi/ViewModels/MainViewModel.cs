using AutoScrew.Application.Services;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.ViewModels.Operation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AutoScrew.Hmi.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const int MaxLogEntries = 50;
    private readonly OperatorSessionController _session;

    public MainViewModel(OperatorSessionController session)
    {
        _session = session;
        _session.Changed += (_, _) =>
        {
            RefreshFromSession();
            NotifyCommandStates();
        };
        ProgressTreeRoot = new OperatorProgressRootViewModel();
        ProgressTreeRoots.Add(ProgressTreeRoot);
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
    private string _statusMessage = "Ready.";

    [ObservableProperty]
    private string _guideHint = "请点击或扫描 SN 开始作业。";

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
            StatusMessage = "Enter SN and confirm.";
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
            StatusMessage = "Validating SN…";
            await _session.SubmitSerialNumberAsync(SerialNumberInput).ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(_session.LastErrorMessage))
                StatusMessage = _session.LastErrorMessage;
            else if (_session.Phase == JobSessionPhase.Running)
            {
                StatusMessage = "Recipe loaded.";
                AddLog($"SN {_session.SerialNumber} → PN {_session.PartNumber}，共 {_session.TemplateSurfaceCount} 面");
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
            var surfaceName = _session.ActiveSurfaceName ?? _session.ActiveSurfaceId ?? "当前面";
            var screwNo = _session.CurrentScrewLocalIndex;
            StatusMessage = "Pick + tighten (simulated)…";
            await _session.RunCurrentScrewCycleAsync().ConfigureAwait(true);

            if (_session.LastErrorMessage is not null)
            {
                StatusMessage = _session.LastErrorMessage;
                AddLog($"{DateTime.Now:HH:mm:ss} NG 【{surfaceName}】#{screwNo} — {_session.LastErrorMessage}");
            }
            else
            {
                StatusMessage = "Step done.";
                AddLog($"{DateTime.Now:HH:mm:ss} OK 【{surfaceName}】#{screwNo}");
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
            StatusMessage = "Unlocked; continue.";
            AddLog($"{DateTime.Now:HH:mm:ss} 技术员解锁 NG");
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
            && !ConfirmTips.ShowDialog("确定放弃当前作业？", System.Windows.Application.Current.MainWindow))
            return;

        _session.ResetToIdle();
        SerialNumberInput = "";
        ActivityLog.Clear();
        IsSnInputEnabled = true;
        StatusMessage = "Session reset.";
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
        }
        catch
        {
            // ignore — page may load before shell ready
        }
    }

    public void RefreshFromSession()
    {
        PhaseDisplay = _session.Phase.ToString();
        GuideHint = _session.BuildGuideHint();
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

    private void RefreshProgressTree()
    {
        ProgressTreeRoot.SerialNumber = _session.SerialNumber ?? "";
        ProgressTreeRoot.PartNumber = _session.PartNumber ?? "";
        if (string.IsNullOrWhiteSpace(_session.SerialNumber))
            ProgressTreeRoot.DisplayLabel = "等待扫码";
        else
            ProgressTreeRoot.DisplayLabel = $"SN: {_session.SerialNumber}  PN: {_session.PartNumber}";

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
                node.Screws.Add(new OperatorScrewNodeViewModel(localIndex, snapshot.ScrewStates[i]));
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
        var done = completedName ?? completedId ?? "当前面";
        var next = nextName ?? "下一面";
        var message = $"【{done}】已完成，请翻面至【{next}】后点击确认。";

        if (!ConfirmTips.ShowDialog(message, System.Windows.Application.Current.MainWindow))
        {
            StatusMessage = "等待确认翻面。";
            return;
        }

        try
        {
            _session.ConfirmAdvanceToNextSurface();
            StatusMessage = $"已进入【{_session.ActiveSurfaceName ?? _session.ActiveSurfaceId}】。";
            AddLog($"{DateTime.Now:HH:mm:ss} 翻面确认 → 【{_session.ActiveSurfaceName}】");
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
