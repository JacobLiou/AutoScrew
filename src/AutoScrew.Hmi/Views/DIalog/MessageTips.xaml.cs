using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AutoScrew.Hmi.Dialog
{
    /// <summary>
    /// MessageTips.xaml 的交互逻辑
    /// </summary>
    public partial class MessageTips
    {
        public MessageResult Result;
        private SynchronizationContext _sync = new DispatcherSynchronizationContext(App.Current.Dispatcher);

        public MessageTips(string message)
        {
            InitializeComponent();
            txt_Content.Text = message;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = this.MinHeight = this.ActualHeight;
            this.MaxWidth = this.MinWidth = this.ActualWidth;
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.OK;
            this.Close();
        }

        public static void ShowDialog(string message, Window? owner = null)
        {
            MessageTips tips = new MessageTips(message);
            tips.Owner = owner;
            if (owner == null) tips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else tips.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            tips.Topmost = true;
            tips.ShowDialog();
        }

        public static void ShowDialog(string messageid, Window? owner = null, params string[] param)
        {
            var message = string.Format(messageid, param);
            ShowDialog(message, owner);
        }

        public static void Show(string message, Window? owner = null)
        {
            MessageTips tips = new MessageTips(message);
            tips.Owner = owner;
            if (owner == null) tips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else tips.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            tips.Show();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ShowWith(Action action)
        {
            if (System.Windows.Application.Current.Dispatcher.Thread == Thread.CurrentThread)
            {
                action.Invoke();
            }
            else
            {
                _sync.Post(p => action.Invoke(), null);
            }
        }
    }

    public enum MessageResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Ignore = 3
    }
}