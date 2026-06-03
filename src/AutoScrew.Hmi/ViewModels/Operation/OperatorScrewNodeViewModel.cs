using AutoScrew.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels.Operation;

public sealed partial class OperatorScrewNodeViewModel : ObservableObject
{
    public OperatorScrewNodeViewModel(int localIndex, StationScrewState state)
    {
        LocalIndex = localIndex;
        _state = state;
    }

    public int LocalIndex { get; }

    public string DisplayLabel => $"钉 {LocalIndex}";

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
