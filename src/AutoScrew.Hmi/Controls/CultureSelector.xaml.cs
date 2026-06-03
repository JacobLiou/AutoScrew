using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace AutoScrew.Hmi.Controls;

public partial class CultureSelector : UserControl
{
    public static readonly DependencyProperty CultureOptionsProperty =
        DependencyProperty.Register(
            nameof(CultureOptions),
            typeof(IEnumerable),
            typeof(CultureSelector),
            new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedCultureProperty =
        DependencyProperty.Register(
            nameof(SelectedCulture),
            typeof(string),
            typeof(CultureSelector),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty VariantProperty =
        DependencyProperty.Register(
            nameof(Variant),
            typeof(CultureSelectorVariant),
            typeof(CultureSelector),
            new PropertyMetadata(CultureSelectorVariant.Shell, OnVariantChanged));

    public CultureSelector()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public IEnumerable? CultureOptions
    {
        get => (IEnumerable?)GetValue(CultureOptionsProperty);
        set => SetValue(CultureOptionsProperty, value);
    }

    public string SelectedCulture
    {
        get => (string)GetValue(SelectedCultureProperty);
        set => SetValue(SelectedCultureProperty, value);
    }

    public CultureSelectorVariant Variant
    {
        get => (CultureSelectorVariant)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => ApplyVariantStyle();

    private static void OnVariantChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CultureSelector selector && selector.IsLoaded)
            selector.ApplyVariantStyle();
    }

    private void ApplyVariantStyle()
    {
        var key = Variant == CultureSelectorVariant.Login
            ? "LoginCultureComboStyle"
            : "ShellCultureComboStyle";

        if (TryFindResource(key) is Style style)
            CultureCombo.Style = style;
    }
}

public enum CultureSelectorVariant
{
    Shell,
    Login,
}
