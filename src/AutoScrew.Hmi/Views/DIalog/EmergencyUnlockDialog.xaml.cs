using System.Windows;
using System.Windows.Input;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Services;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.Dialog;

public partial class EmergencyUnlockDialog
{
    public EmergencyUnlockDialog(string displayName, string userId)
    {
        InitializeComponent();
        var header = Loc.Get("S.Operation.EmergencyUnlockTitle");
        txt_Title.Text = header;
        Title = header;
        txt_User.Text = string.IsNullOrWhiteSpace(displayName)
            ? userId
            : $"{displayName} ({userId})";
        txt_Reason.PlaceholderText = Loc.Format(
            "S.Operation.EmergencyUnlockReasonPlaceholder",
            OperatorSessionController.EmergencyUnlockReasonMinLength);
    }

    public bool Result { get; private set; }

    public string Reason { get; private set; } = "";

    /// <summary>Returns true when the operator confirmed with a valid reason.</summary>
    public static bool TryPrompt(out string reason, Window? owner = null, ICurrentUser? user = null)
    {
        reason = "";
        var current = user
                      ?? (AuditContext.IsInitialized ? AuditContext.User : null);
        var displayName = current?.DisplayName ?? "";
        var userId = current?.UserId ?? "?";

        var dlg = new EmergencyUnlockDialog(displayName, userId)
        {
            Owner = owner ?? System.Windows.Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        dlg.ShowDialog();

        if (AuditContext.IsInitialized)
        {
            AuditHelper.Log(
                AuditContext.Audit,
                AuditContext.Options,
                AuditContext.User,
                AuditCategory.Dialog,
                "Dialog.EmergencyUnlock",
                Loc.Get("S.Operation.EmergencyUnlockTitle"),
                $"result={dlg.Result};reasonLen={dlg.Reason.Length}");
        }

        if (!dlg.Result)
            return false;

        reason = dlg.Reason;
        return true;
    }

    private void btn_OK_Click(object sender, RoutedEventArgs e)
    {
        var trimmed = (txt_Reason.Text ?? string.Empty).Trim();
        if (trimmed.Length < OperatorSessionController.EmergencyUnlockReasonMinLength)
        {
            txt_Validation.Text = Loc.Format(
                "S.Operation.EmergencyUnlockReasonTooShort",
                OperatorSessionController.EmergencyUnlockReasonMinLength);
            txt_Validation.Visibility = Visibility.Visible;
            return;
        }

        Reason = trimmed;
        Result = true;
        Close();
    }

    private void btn_Cancel_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Reason = "";
        Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
}
