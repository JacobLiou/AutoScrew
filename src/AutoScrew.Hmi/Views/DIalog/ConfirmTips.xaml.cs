using System.Windows;
using System.Windows.Input;
using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.Dialog;

/// <summary>
/// ConfirmTips.xaml 的交互逻辑
/// </summary>
public partial class ConfirmTips
{
    public ConfirmTips(string message, string? title = null)
    {
        InitializeComponent();
        txt_Content.Text = message;
        var header = string.IsNullOrWhiteSpace(title) ? Loc.Get("S.Common.Confirm") : title.Trim();
        txt_Title.Text = header;
        Title = header;
    }

    public static bool ShowDialog(string message, Window? owner = null, string? title = null)
    {
        var tips = new ConfirmTips(message, title);
        tips.Owner = owner ?? System.Windows.Application.Current.MainWindow;
        tips.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        tips.ShowDialog();
        if (AuditContext.IsInitialized)
        {
            var summary = message.Length > 200 ? message[..200] + "…" : message;
            AuditHelper.Log(
                AuditContext.Audit,
                AuditContext.Options,
                AuditContext.User,
                AuditCategory.Dialog,
                "Dialog.Confirm",
                title,
                $"message={summary};result={tips.Result}");
        }

        return tips.Result;
    }

    public bool Result { get; set; }

    private void btn_OK_Click(object sender, RoutedEventArgs e)
    {
        Result = true;
        Close();
    }

    private void btn_Cancel_Click(object sender, RoutedEventArgs e)
    {
        Result = false;
        Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}
