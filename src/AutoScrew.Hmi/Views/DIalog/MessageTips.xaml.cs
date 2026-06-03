using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.Dialog;

/// <summary>
/// MessageTips.xaml 的交互逻辑（单按钮提示）。
/// </summary>
public partial class MessageTips
{
    public MessageResult Result;

    private readonly SynchronizationContext _sync = new DispatcherSynchronizationContext(App.Current.Dispatcher);

    public MessageTips(string message, string? title = null)
    {
        InitializeComponent();
        txt_Content.Text = message;
        var header = ResolveTitle(title);
        txt_Title.Text = header;
        Title = header;
    }

    private static string ResolveTitle(string? title)
    {
        if (!string.IsNullOrWhiteSpace(title))
            return title.Trim();

        if (System.Windows.Application.Current.TryFindResource("WinCommon_Hint") is string hint
            && !string.IsNullOrWhiteSpace(hint))
            return hint;

        return Loc.Get("S.Common.Hint");
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        MaxHeight = MinHeight = ActualHeight;
        MaxWidth = MinWidth = ActualWidth;
    }

    private void btn_OK_Click(object sender, RoutedEventArgs e)
    {
        Result = MessageResult.OK;
        Close();
    }

    public static void ShowDialog(string message, Window? owner = null, string? title = null)
    {
        var tips = new MessageTips(message, title);
        tips.Owner = owner ?? System.Windows.Application.Current.MainWindow;
        tips.WindowStartupLocation = owner is null
            ? WindowStartupLocation.CenterScreen
            : WindowStartupLocation.CenterOwner;
        tips.Topmost = true;
        tips.ShowDialog();
    }

    public static void ShowDialog(string messageid, Window? owner, string? title, params string[] param)
    {
        var message = string.Format(messageid, param);
        ShowDialog(message, owner, title);
    }

    public static void Show(string message, Window? owner = null, string? title = null)
    {
        var tips = new MessageTips(message, title);
        tips.Owner = owner ?? System.Windows.Application.Current.MainWindow;
        tips.WindowStartupLocation = owner is null
            ? WindowStartupLocation.CenterScreen
            : WindowStartupLocation.CenterOwner;
        tips.Show();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void ShowWith(Action action)
    {
        if (System.Windows.Application.Current.Dispatcher.Thread == Thread.CurrentThread)
            action.Invoke();
        else
            _sync.Post(_ => action.Invoke(), null);
    }
}

public enum MessageResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Ignore = 3
}
