using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoScrew.Hmi.Models;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoScrew.Hmi.ViewModels;

/// <summary>单面画板编辑（尺寸、底图、螺钉标注）。</summary>
public partial class SurfaceBoardEditorViewModel : ObservableObject
{
    public static ScrewTypePreset DefaultScrewType => ScrewTypeCatalog.Default;

    public IReadOnlyList<ScrewTypePreset> ScrewTypes => ScrewTypeCatalog.All;

    private string? _templateDirectory;

    private string? _productImageAbsolutePath;

    [ObservableProperty]
    private string _boardWidthInput = "800";

    [ObservableProperty]
    private string _boardHeightInput = "600";

    [ObservableProperty]
    private double _boardWidth = 800;

    [ObservableProperty]
    private double _boardHeight = 600;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _outOfBoundsWarning = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBoardImage))]
    [NotifyCanExecuteChangedFor(nameof(MatchBoardToImageSizeCommand))]
    private ImageSource? _boardImageSource;

    [ObservableProperty]
    private double _boardImageOpacity = 1.0;

    public bool HasBoardImage => BoardImageSource is not null;

    public ObservableCollection<ScrewMarkerViewModel> Markers { get; } = new();

    public event EventHandler? ContentChanged;

    public void LoadFrom(SurfaceLayoutDocument surface, string? templateDirectory)
    {
        _templateDirectory = templateDirectory;
        BoardWidth = surface.BoardWidth;
        BoardHeight = surface.BoardHeight;
        BoardWidthInput = surface.BoardWidth.ToString("0.##");
        BoardHeightInput = surface.BoardHeight.ToString("0.##");
        BoardImageOpacity = surface.ProductImageOpacity is >= 0 and <= 1 ? surface.ProductImageOpacity.Value : 1.0;

        TryLoadProductImageFromDocument(surface);

        Markers.Clear();
        foreach (var r in surface.Markers.OrderBy(x => x.Index))
            Markers.Add(MarkerFromRecord(r, surface));

        RenumberMarkers();
        RefreshOutOfBoundsWarning();
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        ClearProductImageCommand.NotifyCanExecuteChanged();
        MatchBoardToImageSizeCommand.NotifyCanExecuteChanged();
        StatusMessage = $"已加载面 {surface.Name}（{Markers.Count} 个点位）。";
    }

    public SurfaceLayoutDocument ToDocument(string surfaceId, string name, int order)
    {
        RenumberMarkers();
        var defaultDiameter = DefaultScrewType.DiameterPx;
        var (rel, abs) = BuildImagePathsForSave();
        return new SurfaceLayoutDocument
        {
            SurfaceId = surfaceId,
            Name = name,
            Order = order,
            BoardWidth = BoardWidth,
            BoardHeight = BoardHeight,
            CircleDiameter = defaultDiameter,
            ProductImageRelativePath = rel,
            ProductImageAbsolutePath = abs,
            ProductImageOpacity = HasBoardImage ? BoardImageOpacity : null,
            Markers = Markers.Select(m => new MarkerRecord
            {
                Index = m.Index,
                CenterX = m.CenterX,
                CenterY = m.CenterY,
                ScrewTypeId = m.ScrewTypeId,
                CircleDiameter = m.CircleDiameter,
            }).ToList(),
        };
    }

    public void SetTemplateDirectory(string? directory) => _templateDirectory = directory;

    public void ClearBoard()
    {
        Markers.Clear();
        BoardImageSource = null;
        _productImageAbsolutePath = null;
        BoardWidth = 800;
        BoardHeight = 600;
        BoardWidthInput = "800";
        BoardHeightInput = "600";
        OutOfBoundsWarning = string.Empty;
        StatusMessage = string.Empty;
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        ClearProductImageCommand.NotifyCanExecuteChanged();
        MatchBoardToImageSizeCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void ApplyBoardSize()
    {
        if (!double.TryParse(BoardWidthInput, out var w) || w <= 0 || w > 32_000)
        {
            StatusMessage = "画板宽度无效，请输入正数。";
            return;
        }

        if (!double.TryParse(BoardHeightInput, out var h) || h <= 0 || h > 32_000)
        {
            StatusMessage = "画板高度无效，请输入正数。";
            return;
        }

        BoardWidth = w;
        BoardHeight = h;
        StatusMessage = $"画板尺寸已应用：{w:0.##} × {h:0.##}";
        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    [RelayCommand]
    private void LoadProductImage()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "图片 (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|所有文件 (*.*)|*.*",
        };

        if (dlg.ShowDialog() != true)
            return;

        try
        {
            LoadProductImageFromAbsolutePath(dlg.FileName);
            RaiseContentChanged();
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载底图失败：{ex.Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanClearProductImage))]
    private void ClearProductImage()
    {
        BoardImageSource = null;
        _productImageAbsolutePath = null;
        BoardImageOpacity = 1.0;
        StatusMessage = "已清除产品底图。";
        RaiseContentChanged();
    }

    private bool CanClearProductImage() => BoardImageSource is not null;

    [RelayCommand(CanExecute = nameof(CanMatchBoardToImage))]
    private void MatchBoardToImageSize()
    {
        if (BoardImageSource is not BitmapImage bmp)
            return;

        BoardWidth = bmp.PixelWidth;
        BoardHeight = bmp.PixelHeight;
        BoardWidthInput = BoardWidth.ToString("0.##");
        BoardHeightInput = BoardHeight.ToString("0.##");
        StatusMessage = $"画板已按底图像素设为 {bmp.PixelWidth} × {bmp.PixelHeight}。";
        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    private bool CanMatchBoardToImage() => BoardImageSource is BitmapImage;

    public void AddMarkerAt(double centerX, double centerY)
    {
        if (centerX < 0 || centerY < 0 || centerX > BoardWidth || centerY > BoardHeight)
        {
            StatusMessage = "标注位置超出画板范围。";
            return;
        }

        var marker = new ScrewMarkerViewModel(centerX, centerY, DefaultScrewType);
        Markers.Add(marker);
        RenumberMarkers();
        SelectSingle(marker);
        StatusMessage = $"已添加螺钉位 {marker.Index}。";
        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    private void SelectSingle(ScrewMarkerViewModel marker)
    {
        foreach (var m in Markers)
            m.IsSelected = ReferenceEquals(m, marker);

        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void SelectMarker(ScrewMarkerViewModel? marker)
    {
        if (marker is null)
            return;

        var additive = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        if (additive)
            marker.IsSelected = !marker.IsSelected;
        else
        {
            foreach (var m in Markers)
                m.IsSelected = ReferenceEquals(m, marker);
        }

        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void SelectAllMarkers()
    {
        foreach (var m in Markers)
            m.IsSelected = true;

        DeleteSelectedCommand.NotifyCanExecuteChanged();
        StatusMessage = Markers.Count == 0 ? "画板上没有标注。" : $"已全选 {Markers.Count} 个标注。";
    }

    [RelayCommand(CanExecute = nameof(CanDeleteSelected))]
    private void DeleteSelected()
    {
        var toRemove = Markers.Where(m => m.IsSelected).ToList();
        if (toRemove.Count == 0)
            return;

        foreach (var m in toRemove)
            Markers.Remove(m);

        RenumberMarkers();
        StatusMessage = $"已删除 {toRemove.Count} 个标注。";
        RefreshOutOfBoundsWarning();
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        RaiseContentChanged();
    }

    private bool CanDeleteSelected() => Markers.Any(m => m.IsSelected);

    [RelayCommand]
    private void ClearSelection()
    {
        foreach (var m in Markers)
            m.IsSelected = false;

        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    public void NotifyDeleteCommandCanExecute() => DeleteSelectedCommand.NotifyCanExecuteChanged();

    partial void OnBoardImageSourceChanged(ImageSource? value)
    {
        ClearProductImageCommand.NotifyCanExecuteChanged();
        MatchBoardToImageSizeCommand.NotifyCanExecuteChanged();
    }

    private void LoadProductImageFromAbsolutePath(string absolutePath)
    {
        if (!File.Exists(absolutePath))
            throw new FileNotFoundException(absolutePath);

        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(Path.GetFullPath(absolutePath), UriKind.Absolute);
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        bmp.EndInit();
        bmp.Freeze();

        BoardImageSource = bmp;
        _productImageAbsolutePath = Path.GetFullPath(absolutePath);
        StatusMessage = $"已加载产品底图：{Path.GetFileName(absolutePath)}。";
    }

    private void TryLoadProductImageFromDocument(SurfaceLayoutDocument doc)
    {
        BoardImageSource = null;
        _productImageAbsolutePath = null;

        var resolved = ProductTemplatePathHelper.ResolveSurfaceImagePath(doc, _templateDirectory ?? "");
        if (string.IsNullOrEmpty(resolved))
            return;

        try
        {
            LoadProductImageFromAbsolutePath(resolved);
        }
        catch
        {
            BoardImageSource = null;
            _productImageAbsolutePath = null;
        }
    }

    private (string? Relative, string? Absolute) BuildImagePathsForSave() =>
        ProductTemplatePathHelper.BuildImagePathsForSave(_productImageAbsolutePath, _templateDirectory);

    private static ScrewMarkerViewModel MarkerFromRecord(MarkerRecord r, SurfaceLayoutDocument doc)
    {
        var fallbackDiameter = doc.CircleDiameter > 0 ? doc.CircleDiameter : DefaultScrewType.DiameterPx;
        var diameter = r.CircleDiameter ?? fallbackDiameter;
        var typeId = r.ScrewTypeId ?? ResolveTypeIdByDiameter(diameter);
        return new ScrewMarkerViewModel(r.CenterX, r.CenterY, diameter, typeId);
    }

    private static int ResolveTypeIdByDiameter(double diameterPx) =>
        ScrewTypeCatalog.All
            .OrderBy(t => Math.Abs(t.DiameterPx - diameterPx))
            .ThenBy(t => t.Id)
            .First()
            .Id;

    private void RenumberMarkers()
    {
        for (var i = 0; i < Markers.Count; i++)
            Markers[i].Index = i + 1;
    }

    private void RefreshOutOfBoundsWarning()
    {
        var bad = Markers.Count(m => m.CenterX < 0 || m.CenterY < 0 || m.CenterX > BoardWidth || m.CenterY > BoardHeight);
        OutOfBoundsWarning = bad == 0
            ? string.Empty
            : $"有 {bad} 个点位超出当前画板范围。";
    }

    private void RaiseContentChanged()
    {
        ContentChanged?.Invoke(this, EventArgs.Empty);
    }
}
