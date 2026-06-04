using AutoScrew.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels.Operation;

public sealed partial class OperatorScrewNodeViewModel : ObservableObject
{
    public OperatorScrewNodeViewModel(int localIndex, StationScrewState state, string displayLabel)
    {
        LocalIndex = localIndex;
        _state = state;
        _displayLabel = displayLabel;
    }

    public int LocalIndex { get; }

    [ObservableProperty]
    private string _displayLabel;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StateGlyph))]
    private StationScrewState _state;

    public string StateGlyph => State switch
    {
        StationScrewState.Ok => "✓",
        StationScrewState.Ng => "✕",
        StationScrewState.InProgress => "●",
        _ => "○"
    };
}
