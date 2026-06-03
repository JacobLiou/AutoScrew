using System.Windows;
using System.Windows.Controls;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.BusinessDialog;

public sealed record ProductInfoResult(string ProductId, string DisplayName, string? Revision);

public partial class ProductInfoDialog : UserControl
{
    public ProductInfoDialog()
    {
        InitializeComponent();
    }

    public ProductInfoResult? Result { get; private set; }

    public void SetInitial(ProductInfoResult? initial)
    {
        if (initial is null)
        {
            RevisionBox.Text = DateTime.Now.ToString("yyyy-MM-dd");
            return;
        }

        ProductIdBox.Text = initial.ProductId;
        DisplayNameBox.Text = initial.DisplayName;
        RevisionBox.Text = initial.Revision ?? "";
    }

    public bool ValidateAndCapture()
    {
        var productId = ProductIdBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(productId))
        {
            ShowError(Loc.Get("S.Template.ErrProductId"));
            return false;
        }

        var displayName = DisplayNameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = productId;

        var revision = RevisionBox.Text.Trim();
        Result = new ProductInfoResult(productId, displayName, string.IsNullOrWhiteSpace(revision) ? null : revision);
        ErrorText.Visibility = Visibility.Collapsed;
        return true;
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }

    public static bool TryShow(ProductInfoResult? initial, out ProductInfoResult? result, Window? owner = null)
    {
        result = null;
        var dialog = new ProductInfoDialog();
        dialog.SetInitial(initial);

        var container = new ContainerWindow();
        container.Title = initial is null ? Loc.Get("S.Template.NewProductDialog") : Loc.Get("S.Template.ProductInfoDialog");
        container.SetChildControl(dialog);
        container.Owner = owner ?? System.Windows.Application.Current.MainWindow;
        container.WindowStartupLocation = container.Owner is null
            ? WindowStartupLocation.CenterScreen
            : WindowStartupLocation.CenterOwner;
        container.Topmost = true;
        container.Width = dialog.Width + 58;
        container.Height = dialog.Height + 88;
        container.ShowDialog();

        if (container.Result != MessageResult.OK)
            return false;

        if (dialog.ValidateAndCapture())
        {
            result = dialog.Result;
            return true;
        }

        var retry = new ProductInfoResult(
            dialog.ProductIdBox.Text.Trim(),
            dialog.DisplayNameBox.Text.Trim(),
            string.IsNullOrWhiteSpace(dialog.RevisionBox.Text) ? null : dialog.RevisionBox.Text.Trim());
        return TryShow(retry, out result, owner);
    }
}
