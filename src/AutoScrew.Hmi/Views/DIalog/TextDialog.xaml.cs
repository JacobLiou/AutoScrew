using System.Windows;
using System.Windows.Controls;

namespace AutoScrew.Hmi.Dialog;

public partial class TextDialog
{
    public TextDialog(string info = "请稍候…")
    {
        InitializeComponent();
        textBlock.Text = info;
    }

    private void BtnClose_OnClick(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        window?.Close();
    }
}
