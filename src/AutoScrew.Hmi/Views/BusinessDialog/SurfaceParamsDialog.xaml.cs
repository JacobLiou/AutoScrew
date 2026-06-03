using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.BusinessDialog;

public sealed record SurfaceParamsResult(
    string SurfaceId,
    string Name,
    int Order,
    double BoardWidth,
    double BoardHeight);

public partial class SurfaceParamsDialog : UserControl
{
    private IReadOnlyCollection<string> _existingIds = Array.Empty<string>();
    private string? _excludeId;

    public SurfaceParamsDialog()
    {
        InitializeComponent();
    }

    public SurfaceParamsResult? Result { get; private set; }

    public void SetInitial(SurfaceParamsResult initial, IReadOnlyCollection<string> existingIds, string? excludeId)
    {
        _existingIds = existingIds;
        _excludeId = excludeId;
        SurfaceIdBox.Text = initial.SurfaceId;
        NameBox.Text = initial.Name;
        OrderBox.Text = initial.Order.ToString(CultureInfo.InvariantCulture);
        BoardWidthBox.Text = initial.BoardWidth.ToString("0.##", CultureInfo.InvariantCulture);
        BoardHeightBox.Text = initial.BoardHeight.ToString("0.##", CultureInfo.InvariantCulture);
    }

    public bool ValidateAndCapture()
    {
        var surfaceId = SurfaceIdBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(surfaceId))
        {
            ShowError(Loc.Get("S.Template.ErrSurfaceIdEmpty"));
            return false;
        }

        if (!int.TryParse(OrderBox.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var order) || order < 1)
        {
            ShowError(Loc.Get("S.Template.ErrOrder"));
            return false;
        }

        if (!double.TryParse(BoardWidthBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var width) || width <= 0)
        {
            ShowError(Loc.Get("S.Template.ErrBoardWidth"));
            return false;
        }

        if (!double.TryParse(BoardHeightBox.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var height) || height <= 0)
        {
            ShowError(Loc.Get("S.Template.ErrBoardHeight"));
            return false;
        }

        var duplicate = _existingIds.Any(id =>
            !string.Equals(id, _excludeId, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(id, surfaceId, StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            ShowError(Loc.Format("S.Template.ErrSurfaceIdDup", surfaceId));
            return false;
        }

        var name = NameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
            name = surfaceId;

        Result = new SurfaceParamsResult(surfaceId, name, order, width, height);
        ErrorText.Visibility = Visibility.Collapsed;
        return true;
    }

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
    }

    public static bool TryShow(
        SurfaceParamsResult initial,
        IReadOnlyCollection<string> existingIds,
        string? excludeId,
        string title,
        out SurfaceParamsResult? result,
        Window? owner = null)
    {
        result = null;
        var dialog = new SurfaceParamsDialog();
        dialog.SetInitial(initial, existingIds, excludeId);

        var container = new ContainerWindow();
        container.Title = title;
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

        var retry = new SurfaceParamsResult(
            dialog.SurfaceIdBox.Text.Trim(),
            dialog.NameBox.Text.Trim(),
            int.TryParse(dialog.OrderBox.Text.Trim(), out var o) ? o : initial.Order,
            double.TryParse(dialog.BoardWidthBox.Text.Trim(), out var w) ? w : initial.BoardWidth,
            double.TryParse(dialog.BoardHeightBox.Text.Trim(), out var h) ? h : initial.BoardHeight);
        return TryShow(retry, existingIds, excludeId, title, out result, owner);
    }
}
