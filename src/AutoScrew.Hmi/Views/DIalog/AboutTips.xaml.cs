using System.Windows;
using System.Windows.Media.Animation;

namespace AutoScrew.Hmi.Dialog
{
    /// <summary>
    /// AboutTips.xaml 的交互逻辑
    /// </summary>
    public partial class AboutTips
    {
        public AboutTips()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = backImage.Width;
            animation.Duration = TimeSpan.FromSeconds(2);

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, backImage);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Width"));

            storyboard.Begin();
        }
    }
}