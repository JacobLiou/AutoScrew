using System.Windows;
using System.Windows.Input;
using AutoScrew.Hmi.Services;
using AutoScrew.Infrastructure.ProcessLibrary;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public enum SequenceExportLocationMode
{
    StepNumber = 0,
    Empty = 1,
    CustomPrefix = 2,
}

public partial class ExportSequenceExcelDialog
{
    public ExportSequenceExcelDialog(string? initialDefaultScrewPn)
    {
        InitializeComponent();
        Title = Loc.Get("S.ControllerSeq.ExportSequenceExcelTitle");
        txt_DefaultScrewPn.Text = initialDefaultScrewPn ?? string.Empty;
        cmb_LocationMode.Items.Clear();
        cmb_LocationMode.Items.Add(Loc.Get("S.ControllerSeq.ExportLocationStepNumber"));
        cmb_LocationMode.Items.Add(Loc.Get("S.ControllerSeq.ExportLocationEmpty"));
        cmb_LocationMode.Items.Add(Loc.Get("S.ControllerSeq.ExportLocationCustomPrefix"));
        cmb_LocationMode.SelectedIndex = 0;
        cmb_LocationMode.SelectionChanged += (_, _) => UpdatePrefixVisibility();
        UpdatePrefixVisibility();
    }

    public bool Confirmed { get; private set; }

    public string DefaultScrewPn { get; private set; } = "";

    public SequenceExportLocationMode LocationMode { get; private set; }

    public string LocationPrefix { get; private set; } = "";

    public static bool TryPrompt(
        string? initialDefaultScrewPn,
        out string defaultScrewPn,
        out SequenceExportLocationMode locationMode,
        out string locationPrefix,
        Window? owner = null)
    {
        defaultScrewPn = "";
        locationMode = SequenceExportLocationMode.StepNumber;
        locationPrefix = "";
        var dlg = new ExportSequenceExcelDialog(initialDefaultScrewPn)
        {
            Owner = owner ?? System.Windows.Application.Current?.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        dlg.ShowDialog();
        if (!dlg.Confirmed)
            return false;

        defaultScrewPn = dlg.DefaultScrewPn;
        locationMode = dlg.LocationMode;
        locationPrefix = dlg.LocationPrefix;
        return true;
    }

    public static string FormatLocation(SequenceExportLocationMode mode, string prefix, int order) =>
        mode switch
        {
            SequenceExportLocationMode.Empty => string.Empty,
            SequenceExportLocationMode.CustomPrefix =>
                string.IsNullOrWhiteSpace(prefix) ? order.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    : $"{prefix.Trim()}{order}",
            _ => Loc.Format("S.ControllerSeq.ExportLocationStepFormat", order),
        };

    private void UpdatePrefixVisibility()
    {
        txt_Prefix.Visibility = cmb_LocationMode.SelectedIndex == (int)SequenceExportLocationMode.CustomPrefix
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void btn_OK_Click(object sender, RoutedEventArgs e)
    {
        var pn = ProcessParameterCode.SanitizeAscii(txt_DefaultScrewPn.Text ?? string.Empty);
        if (string.IsNullOrEmpty(pn))
        {
            txt_Validation.Text = Loc.Get("S.ControllerSeq.ExportDefaultScrewPnRequired");
            txt_Validation.Visibility = Visibility.Visible;
            return;
        }

        DefaultScrewPn = pn;
        LocationMode = (SequenceExportLocationMode)Math.Clamp(cmb_LocationMode.SelectedIndex, 0, 2);
        LocationPrefix = txt_Prefix.Text?.Trim() ?? string.Empty;
        Confirmed = true;
        Close();
    }

    private void btn_Cancel_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
}
