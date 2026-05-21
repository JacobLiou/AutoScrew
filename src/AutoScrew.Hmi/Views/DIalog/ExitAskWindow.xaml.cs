using System.Windows;
using System.Windows.Input;

namespace AutoScrew.Hmi.Dialog
{
    /// <summary>
    /// ExitAskWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExitAskWindow
    {
        public ExitAskWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = this.MinHeight = this.ActualHeight;
            this.MaxWidth = this.MinWidth = this.ActualWidth;
        }

        private void btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}