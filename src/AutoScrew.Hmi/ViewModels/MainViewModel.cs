using System.Collections.ObjectModel;
using System.Windows;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Services;
using AutoScrew.Domain.Models;
using AutoScrew.Domain.Session;
using AutoScrew.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoScrew.Hmi.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly OperatorSessionController _session;
    private readonly SessionCurrentUser _currentUser;

    public MainViewModel(OperatorSessionController session, SessionCurrentUser currentUser)
    {
        _session = session;
        _currentUser = currentUser;
        RoleDisplay = RoleLabel(_currentUser.Role);
    }

    public OperatorSessionController Session => _session;

    public event EventHandler? CurveChanged;

    [ObservableProperty]
    private string _serialNumberInput = "";

    [ObservableProperty]
    private string _phaseDisplay = JobSessionPhase.Idle.ToString();

    [ObservableProperty]
    private string _statusMessage = "Ready.";

    [ObservableProperty]
    private string _roleDisplay = "";

    [ObservableProperty]
    private string? _productImagePath;

    [ObservableProperty]
    private double _boardWidth;

    [ObservableProperty]
    private double _boardHeight;

    public ObservableCollection<ScrewMarkerVm> Markers { get; } = new();

    [RelayCommand]
    private void OpenScan()
    {
        try
        {
            _session.RequestScanDialog();
            RefreshFromSession();
            StatusMessage = "Enter SN and confirm.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task SubmitSnAsync()
    {
        try
        {
            StatusMessage = "Validating SN…";
            await _session.SubmitSerialNumberAsync(SerialNumberInput).ConfigureAwait(true);
            RefreshFromSession();
            StatusMessage = _session.LastErrorMessage ?? "Recipe loaded.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task RunCurrentScrewAsync()
    {
        try
        {
            StatusMessage = "Pick + tighten (simulated)…";
            await _session.RunCurrentScrewCycleAsync().ConfigureAwait(true);
            RefreshFromSession();
            StatusMessage = _session.LastErrorMessage ?? "Step done.";
            CurveChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void UnlockNg()
    {
        try
        {
            _session.UnlockNgContinue();
            RefreshFromSession();
            StatusMessage = "Unlocked; continue.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void ResetSession()
    {
        _session.ResetToIdle();
        SerialNumberInput = "";
        RefreshFromSession();
        StatusMessage = "Session reset.";
        CurveChanged?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void UseOperatorRole() => ApplyRole(UserRole.Operator, "operator", "Operator");

    [RelayCommand]
    private void UseTechnicianRole() => ApplyRole(UserRole.Technician, "tech", "Technician");

    [RelayCommand]
    private void UseAdminRole() => ApplyRole(UserRole.Administrator, "admin", "Administrator");

    private void ApplyRole(UserRole role, string id, string name)
    {
        _currentUser.SetRole(role, id, name);
        RoleDisplay = RoleLabel(role);
        StatusMessage = $"Role: {RoleDisplay}";
    }

    public void RefreshFromSession()
    {
        PhaseDisplay = _session.Phase.ToString();
        ProductImagePath = _session.ResolvedProductImagePath;
        BoardWidth = _session.BoardWidth;
        BoardHeight = _session.BoardHeight;

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
    }

    private static string RoleLabel(UserRole role) =>
        role switch
        {
            UserRole.Technician => "Technician",
            UserRole.Administrator => "Administrator",
            _ => "Operator"
        };
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
