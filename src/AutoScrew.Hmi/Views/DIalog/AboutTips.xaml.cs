using System.Reflection;
using System.Windows;

namespace AutoScrew.Hmi.Dialog;

public partial class AboutTips
{
    public AboutTips()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?";
        txtVersion.Text = $"版本 {v}";
    }

    private void btn_OK_Click(object sender, RoutedEventArgs e) => Close();
}
