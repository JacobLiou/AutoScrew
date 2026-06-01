using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.Converters;

/// <summary>
/// 用于将 <see cref="NavigationViewPaneDisplayMode"/> 映射到 ComboBox.SelectedIndex。
/// 0=Left, 1=LeftFluent, 2=Top, 3=Bottom
/// </summary>
internal sealed class PaneDisplayModeToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            NavigationViewPaneDisplayMode.LeftFluent => 1,
            NavigationViewPaneDisplayMode.Top => 2,
            NavigationViewPaneDisplayMode.Bottom => 3,
            _ => 0,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            1 => NavigationViewPaneDisplayMode.LeftFluent,
            2 => NavigationViewPaneDisplayMode.Top,
            3 => NavigationViewPaneDisplayMode.Bottom,
            _ => NavigationViewPaneDisplayMode.Left,
        };
    }
}

