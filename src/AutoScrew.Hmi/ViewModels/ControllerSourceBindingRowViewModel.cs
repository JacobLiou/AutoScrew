using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ControllerSourceBindingRowViewModel : ObservableObject
{
    public ControllerSourceBindingRowViewModel(int toolIndex)
    {
        ToolIndex = toolIndex;
        ToolLabel = toolIndex switch
        {
            0 => Loc.Get("S.Workbench.Source.Tool1"),
            1 => Loc.Get("S.Workbench.Source.Tool2"),
            _ => Loc.Format("S.Workbench.Source.ToolN", toolIndex + 1),
        };
        Advanced = SourceAdvancedSettingsCore.CreateDefaults();
    }

    public int ToolIndex { get; }

    public string ToolLabel { get; }

    [ObservableProperty] private ControllerSequenceListItem? _selectedSequence;

    [ObservableProperty] private int _screwCount = 1;

    [ObservableProperty] private int _bitId;

    [ObservableProperty] private string _summaryText = string.Empty;

    [ObservableProperty] private SourceAdvancedSettingsCore _advanced;

    partial void OnSelectedSequenceChanged(ControllerSequenceListItem? value)
    {
        if (value is null)
        {
            SummaryText = string.Empty;
            return;
        }

        SummaryText = Loc.Format("S.Workbench.Source.SequenceSummary", value.SequenceId, value.Name, ScrewCount);
    }

    partial void OnScrewCountChanged(int value)
    {
        if (SelectedSequence is not null)
            SummaryText = Loc.Format("S.Workbench.Source.SequenceSummary", SelectedSequence.SequenceId, SelectedSequence.Name, value);
    }

    public void ApplyFromEntry(ControllerSourceBindingEntry entry, IReadOnlyList<ControllerSequenceListItem> sequences)
    {
        ScrewCount = entry.ScrewCount;
        BitId = entry.BitId;
        Advanced = entry.Advanced ?? SourceAdvancedSettingsCore.CreateDefaults();
        SelectedSequence = sequences.FirstOrDefault(s => s.SequenceId == entry.TargetId);
        if (SelectedSequence is null && entry.TargetId > 0)
            SummaryText = Loc.Format("S.Workbench.Source.MissingSequence", entry.TargetId);
    }

    public ControllerSourceBindingEntry ToEntry()
    {
        return new ControllerSourceBindingEntry
        {
            ToolIndex = ToolIndex,
            BindingType = 1,
            TargetId = SelectedSequence?.SequenceId ?? 0,
            ScrewCount = ScrewCount,
            BitId = BitId,
            Advanced = Advanced,
        };
    }
}
