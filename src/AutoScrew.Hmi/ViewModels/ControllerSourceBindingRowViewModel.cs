using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using UDL.Delta.IemdSd.Protocol;

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

    /// <summary>0 = 参数，1 = 顺序。</summary>
    [ObservableProperty] private int _bindingType = (int)TighteningSourceBindingType.Sequence;

    [ObservableProperty] private int _targetId;

    [ObservableProperty] private string _bindingDisplayText = string.Empty;

    public string BindingDisplayOrPlaceholder =>
        string.IsNullOrWhiteSpace(BindingDisplayText)
            ? Loc.Get("S.ControllerSource.PickerPlaceholder")
            : BindingDisplayText;

    public bool HasBindingSelection => !string.IsNullOrWhiteSpace(BindingDisplayText);

    [ObservableProperty] private int _screwCount = 1;

    [ObservableProperty] private int _bitId;

    [ObservableProperty] private string _summaryText = string.Empty;

    [ObservableProperty] private SourceAdvancedSettingsCore _advanced;

    partial void OnScrewCountChanged(int value) => RefreshSummary();

    partial void OnBindingDisplayTextChanged(string value)
    {
        OnPropertyChanged(nameof(BindingDisplayOrPlaceholder));
        OnPropertyChanged(nameof(HasBindingSelection));
    }

    /// <summary>从本地/设备条目恢复；保留条目内螺钉数与批头。</summary>
    public void ApplyFromEntry(
        ControllerSourceBindingEntry entry,
        IReadOnlyList<ControllerSequenceListItem> sequences,
        IReadOnlyList<ControllerParameterListItem> parameters)
    {
        BindingType = entry.BindingType;
        TargetId = entry.TargetId;
        ScrewCount = entry.ScrewCount;
        BitId = entry.BitId;
        Advanced = entry.Advanced ?? SourceAdvancedSettingsCore.CreateDefaults();

        if (entry.BindingType == (int)TighteningSourceBindingType.Parameter)
        {
            var param = parameters.FirstOrDefault(p => p.ParameterId == entry.TargetId);
            BindingDisplayText = param?.DisplayText
                ?? (entry.TargetId > 0
                    ? Loc.Format("S.Workbench.Source.MissingParameter", entry.TargetId)
                    : string.Empty);
        }
        else
        {
            var seq = sequences.FirstOrDefault(s => s.SequenceId == entry.TargetId);
            BindingDisplayText = seq?.DisplayText
                ?? (entry.TargetId > 0
                    ? Loc.Format("S.Workbench.Source.MissingSequence", entry.TargetId)
                    : string.Empty);
        }

        RefreshSummary();
    }

    /// <summary>弹窗选定参数或顺序后应用；顺序时携带步数与批头。</summary>
    public void ApplyPickerSelection(int bindingType, int targetId, string displayText, int screwCount, int bitId)
    {
        BindingType = bindingType;
        TargetId = targetId;
        BindingDisplayText = displayText;
        ScrewCount = screwCount > 0 ? screwCount : 1;
        BitId = bitId;
        RefreshSummary();
    }

    public ControllerSourceBindingEntry ToEntry()
    {
        return new ControllerSourceBindingEntry
        {
            ToolIndex = ToolIndex,
            BindingType = BindingType,
            TargetId = TargetId,
            ScrewCount = ScrewCount,
            BitId = BitId,
            Advanced = Advanced,
        };
    }

    private void RefreshSummary()
    {
        if (TargetId <= 0)
        {
            SummaryText = string.Empty;
            return;
        }

        var kind = BindingType == (int)TighteningSourceBindingType.Parameter
            ? Loc.Get("S.ControllerSource.BindParameter")
            : Loc.Get("S.ControllerSource.BindSequence");
        SummaryText = Loc.Format(
            "S.Workbench.Source.BindingSummary",
            kind,
            TargetId,
            ScrewCount);
    }
}
