using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using AutoScrew.Hmi.Services;
using AutoScrew.Infrastructure.ProcessLibrary;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class ExportProcessCardDialog
{
    public ExportProcessCardDialog(string initialScrewPn, int initialSlotId)
    {
        InitializeComponent();
        Title = Loc.Get("S.ControllerParam.ExportProcessCardTitle");
        txt_ScrewPn.Text = initialScrewPn ?? string.Empty;
        txt_Slot.Text = initialSlotId.ToString("D2", CultureInfo.InvariantCulture);
        rb_Ver1.IsChecked = true;
        UpdateDeviceIdPreview();
        txt_Slot.TextChanged += (_, _) => UpdateDeviceIdPreview();
    }

    public bool Confirmed { get; private set; }

    public string ScrewPn { get; private set; } = "";

    public int SlotId { get; private set; }

    public ProcessCardTemplateVersion TemplateVersion { get; private set; } = ProcessCardTemplateVersion.Ver1;

    public static bool TryPrompt(
        string initialScrewPn,
        int initialSlotId,
        out string screwPn,
        out int slotId,
        out ProcessCardTemplateVersion templateVersion,
        Window? owner = null)
    {
        screwPn = "";
        slotId = 0;
        templateVersion = ProcessCardTemplateVersion.Ver1;
        var dlg = new ExportProcessCardDialog(initialScrewPn, initialSlotId)
        {
            Owner = owner ?? System.Windows.Application.Current?.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };
        dlg.ShowDialog();
        if (!dlg.Confirmed)
            return false;

        screwPn = dlg.ScrewPn;
        slotId = dlg.SlotId;
        templateVersion = dlg.TemplateVersion;
        return true;
    }

    private void UpdateDeviceIdPreview()
    {
        if (TryParseSlot(txt_Slot.Text, out var slot))
        {
            try
            {
                var deviceId = ProcessParameterCode.ToDeviceParameterId(slot);
                txt_DeviceId.Text = Loc.Format("S.ControllerParam.ExportDeviceIdPreview", deviceId);
                txt_Validation.Visibility = Visibility.Collapsed;
                return;
            }
            catch (InvalidDataException ex)
            {
                txt_DeviceId.Text = "—";
                txt_Validation.Text = ex.Message;
                txt_Validation.Visibility = Visibility.Visible;
                return;
            }
        }

        txt_DeviceId.Text = "—";
    }

    private void btn_OK_Click(object sender, RoutedEventArgs e)
    {
        var pn = ProcessParameterCode.SanitizeAscii(txt_ScrewPn.Text ?? string.Empty);
        if (string.IsNullOrEmpty(pn))
        {
            txt_Validation.Text = Loc.Get("S.ControllerParam.ExportScrewPnRequired");
            txt_Validation.Visibility = Visibility.Visible;
            return;
        }

        if (!TryParseSlot(txt_Slot.Text, out var slot))
        {
            txt_Validation.Text = Loc.Get("S.ControllerParam.ExportSlotInvalid");
            txt_Validation.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            _ = ProcessParameterCode.ToDeviceParameterId(slot);
        }
        catch (InvalidDataException ex)
        {
            txt_Validation.Text = ex.Message;
            txt_Validation.Visibility = Visibility.Visible;
            return;
        }

        ScrewPn = pn;
        SlotId = slot;
        TemplateVersion = rb_Ver2.IsChecked == true
            ? ProcessCardTemplateVersion.Ver2
            : ProcessCardTemplateVersion.Ver1;
        Confirmed = true;
        Close();
    }

    private void btn_Cancel_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private static bool TryParseSlot(string? raw, out int slot)
    {
        slot = 0;
        if (string.IsNullOrWhiteSpace(raw))
            return false;
        return int.TryParse(raw.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out slot)
               && slot is >= 0 and <= 499;
    }
}
