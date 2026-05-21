using System.Windows;
using System.Windows.Input;

namespace AutoScrew.Hmi.Dialog
{
    /// <summary>
    /// ConfirmTips.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmTips
    {
        public ConfirmTips(string message)
        {
            InitializeComponent();
            txt_Content.Text = message;
        }

        public static bool ShowDialog(string message, Window? owner = null)
        {
            ConfirmTips tips = new ConfirmTips(message);
            if (owner == null) tips.Owner = System.Windows.Application.Current.MainWindow;
            else tips.Owner = owner;
            tips.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            tips.ShowDialog();
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
            this.DragMove();
        }
    }
}