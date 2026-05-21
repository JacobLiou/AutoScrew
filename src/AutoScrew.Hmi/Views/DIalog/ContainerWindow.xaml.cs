using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AutoScrew.Hmi.Dialog
{
    /// <summary>
    /// ContainerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ContainerWindow
    {
        public MessageResult Result { get; private set; }

        private SynchronizationContext _sync = new DispatcherSynchronizationContext(App.Current.Dispatcher);

        public ContainerWindow()
        {
            InitializeComponent();
        }

        public void SetChildControl(UserControl userControl)
        {
            contentControl.Content = userControl;
            contentControl.HorizontalContentAlignment = HorizontalAlignment.Center;
        }

        public static MessageResult ShowDialog(UserControl userControl, string? title = null, Window? owner = null)
        {
            ContainerWindow container = new ContainerWindow();
            container.Result = MessageResult.None;
            container.Title = title ?? "";
            container.Owner = owner;
            container.SetChildControl(userControl);

            if (owner == null)
                container.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
                container.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            container.Topmost = true;
            container.Width = userControl.Width + 58;
            container.Height = userControl.Height + 38;
            container.ShowDialog();
            return container.Result;
        }

        private void btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageResult.Cancel;
            this.Close();
        }
    }
}