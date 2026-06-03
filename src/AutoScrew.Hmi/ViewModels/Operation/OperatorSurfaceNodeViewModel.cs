using System.Collections.ObjectModel;
using AutoScrew.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels.Operation;

public sealed partial class OperatorSurfaceNodeViewModel : ObservableObject
{
    public OperatorSurfaceNodeViewModel(string surfaceId, string name, int order, SurfaceProgressState progressState)
    {
        SurfaceId = surfaceId;
        Name = name;
        Order = order;
        _progressState = progressState;
    }

    public string SurfaceId { get; }

    public string Name { get; }

    public int Order { get; }

    public string DisplayLabel => $"{Name} ({SurfaceId})";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StateGlyph))]
    private SurfaceProgressState _progressState;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isActive;

    public ObservableCollection<OperatorScrewNodeViewModel> Screws { get; } = new();

    public string StateGlyph => ProgressState switch
    {
        SurfaceProgressState.Active => "●",
        SurfaceProgressState.Complete => "✓",
        SurfaceProgressState.NgLocked => "✕",
        _ => "○"
    };
}
