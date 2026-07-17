using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.ViewModels;
using UDL.Delta.IemdSd.Protocol;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class SourceBindingPickerDialog : Window
{
    public SourceBindingPickerDialog(
        IReadOnlyList<ControllerParameterListItem> parameters,
        IReadOnlyList<ControllerSequenceListItem> sequences,
        int initialBindingType,
        int initialTargetId)
    {
        InitializeComponent();
        Title = Loc.Get("S.ControllerSource.PickerTitle");

        ParameterItems = new ObservableCollection<PickerRow>(
            parameters.Select(p => new PickerRow(
                (int)TighteningSourceBindingType.Parameter,
                p.ParameterId,
                p.DisplayText,
                ScrewCount: 1,
                BitId: 0)));

        SequenceItems = new ObservableCollection<PickerRow>(
            sequences.Select(s => new PickerRow(
                (int)TighteningSourceBindingType.Sequence,
                s.SequenceId,
                s.DisplayText,
                ScrewCount: s.StepCount > 0 ? s.StepCount : 1,
                BitId: s.BitId)));

        SelectedTabIndex = initialBindingType == (int)TighteningSourceBindingType.Parameter ? 0 : 1;
        DataContext = this;

        Loaded += (_, _) =>
        {
            if (initialTargetId <= 0)
                return;

            if (SelectedTabIndex == 0)
                ParameterList.SelectedItem = ParameterItems.FirstOrDefault(i => i.TargetId == initialTargetId);
            else
                SequenceList.SelectedItem = SequenceItems.FirstOrDefault(i => i.TargetId == initialTargetId);
        };
    }

    public ObservableCollection<PickerRow> ParameterItems { get; }

    public ObservableCollection<PickerRow> SequenceItems { get; }

    public int SelectedTabIndex { get; set; }

    public bool Confirmed { get; private set; }

    public PickerRow? SelectedRow { get; private set; }

    private void SelectRow(PickerRow? row)
    {
        if (row is null)
            return;

        SelectedRow = row;
        Confirmed = true;
        DialogResult = true;
        Close();
    }

    private void ParameterList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) =>
        SelectRow(ParameterList.SelectedItem as PickerRow);

    private void SequenceList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) =>
        SelectRow(SequenceList.SelectedItem as PickerRow);

    private void ParameterList_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            SelectRow(ParameterList.SelectedItem as PickerRow);
    }

    private void SequenceList_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            SelectRow(SequenceList.SelectedItem as PickerRow);
    }

    private void Select_Click(object sender, RoutedEventArgs e)
    {
        var list = PickerTabs.SelectedIndex == 0 ? ParameterList : SequenceList;
        SelectRow(list.SelectedItem as PickerRow);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        DialogResult = false;
        Close();
    }

    public sealed record PickerRow(
        int BindingType,
        int TargetId,
        string DisplayText,
        int ScrewCount,
        int BitId)
    {
        public string IdText => TargetId.ToString();
    }
}
