using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoScrew.Hmi.Models;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace AutoScrew.Hmi.ViewModels;

public partial class TemplateBoardViewModel : ObservableObject
{
    /// <summary>新建标注时使用的螺钉类型（默认 M2）。</summary>
    public static ScrewTypePreset DefaultScrewType => ScrewTypeCatalog.Default;

    public IReadOnlyList<ScrewTypePreset> ScrewTypes => ScrewTypeCatalog.All;

    /// <summary>当前 JSON 所在目录（保存/打开后用于解析相对底图路径）。</summary>
    private string? _templateDirectory;

    private string? _productImageAbsolutePath;

    [ObservableProperty]
    private string boardWidthInput = "800";

    [ObservableProperty]
    private string boardHeightInput = "600";

    [ObservableProperty]
    private double boardWidth = 800;

    [ObservableProperty]
    private double boardHeight = 600;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private string outOfBoundsWarning = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBoardImage))]
    [NotifyCanExecuteChangedFor(nameof(MatchBoardToImageSizeCommand))]
    private ImageSource? boardImageSource;

    [ObservableProperty]
    private double boardImageOpacity = 1.0;

    public bool HasBoardImage => BoardImageSource is not null;

    public ObservableCollection<ScrewMarkerViewModel> Markers { get; } = new();

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
    }

    [RelayCommand]
    private void LoadProductImage()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "图片 (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|所有文件 (*.*)|*.*",
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        try
        {
            LoadProductImageFromAbsolutePath(dlg.FileName);
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
    }

    private bool CanClearProductImage() => BoardImageSource is not null;

    [RelayCommand(CanExecute = nameof(CanMatchBoardToImage))]
    private void MatchBoardToImageSize()
    {
        if (BoardImageSource is not BitmapImage bmp)
        {
            return;
        }

        BoardWidth = bmp.PixelWidth;
        BoardHeight = bmp.PixelHeight;
        BoardWidthInput = BoardWidth.ToString("0.##");
        BoardHeightInput = BoardHeight.ToString("0.##");
        StatusMessage = $"画板已按底图像素设为 {bmp.PixelWidth} × {bmp.PixelHeight}（与标注坐标 1:1）。";
        RefreshOutOfBoundsWarning();
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
        StatusMessage = $"已添加螺钉位 {marker.Index}（{DefaultScrewType.DisplayName}）。";
        RefreshOutOfBoundsWarning();
    }

    [RelayCommand]
    private void SelectMarker(ScrewMarkerViewModel? marker)
    {
        if (marker is null)
        {
            return;
        }

        var additive = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        if (additive)
        {
            marker.IsSelected = !marker.IsSelected;
        }
        else
        {
            foreach (var m in Markers)
            {
                m.IsSelected = ReferenceEquals(m, marker);
            }
        }

        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void SelectAllMarkers()
    {
        foreach (var m in Markers)
        {
            m.IsSelected = true;
        }

        DeleteSelectedCommand.NotifyCanExecuteChanged();
        StatusMessage = Markers.Count == 0 ? "画板上没有标注。" : $"已全选 {Markers.Count} 个标注。";
    }

    private void SelectSingle(ScrewMarkerViewModel marker)
    {
        foreach (var m in Markers)
        {
            m.IsSelected = ReferenceEquals(m, marker);
        }

        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanDeleteSelected))]
    private void DeleteSelected()
    {
        var toRemove = Markers.Where(m => m.IsSelected).ToList();
        if (toRemove.Count == 0)
        {
            return;
        }

        foreach (var m in toRemove)
        {
            Markers.Remove(m);
        }

        RenumberMarkers();
        StatusMessage = $"已删除 {toRemove.Count} 个标注。";
        RefreshOutOfBoundsWarning();
        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    private bool CanDeleteSelected() => Markers.Any(m => m.IsSelected);

    [RelayCommand]
    private void ClearSelection()
    {
        foreach (var m in Markers)
        {
            m.IsSelected = false;
        }

        DeleteSelectedCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void SaveTemplate()
    {
        var dlg = new SaveFileDialog
        {
            Filter = "JSON 模板 (*.json)|*.json|所有文件 (*.*)|*.*",
            DefaultExt = ".json",
            FileName = "pn_template.json",
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        try
        {
            _templateDirectory = Path.GetDirectoryName(dlg.FileName);
            var doc = ToDocument();
            TemplateJsonSerializer.Save(dlg.FileName, doc);
            StatusMessage = $"已保存：{dlg.FileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"保存失败：{ex.Message}";
        }
    }

    [RelayCommand]
    private void OpenTemplate()
    {
        var dlg = new OpenFileDialog
        {
            Filter = "JSON 模板 (*.json)|*.json|所有文件 (*.*)|*.*",
        };

        if (dlg.ShowDialog() != true)
        {
            return;
        }

        try
        {
            var doc = TemplateJsonSerializer.Load(dlg.FileName);
            _templateDirectory = Path.GetDirectoryName(dlg.FileName);

            BoardWidth = doc.BoardWidth;
            BoardHeight = doc.BoardHeight;
            BoardWidthInput = doc.BoardWidth.ToString("0.##");
            BoardHeightInput = doc.BoardHeight.ToString("0.##");

            if (doc.ProductImageOpacity is >= 0 and <= 1)
            {
                BoardImageOpacity = doc.ProductImageOpacity.Value;
            }
            else
            {
                BoardImageOpacity = 1.0;
            }

            var hadImagePath = !string.IsNullOrWhiteSpace(doc.ProductImageRelativePath)
                               || !string.IsNullOrWhiteSpace(doc.ProductImageAbsolutePath);
            var imageLoaded = TryLoadProductImageFromDocument(doc);

            Markers.Clear();
            foreach (var r in doc.Markers.OrderBy(x => x.Index))
            {
                Markers.Add(MarkerFromRecord(r, doc));
            }

            RenumberMarkers();
            var suffix = imageLoaded
                ? "，已加载产品底图。"
                : hadImagePath
                    ? "，底图路径无效或文件不存在。"
                    : string.Empty;
            StatusMessage = $"已打开：{dlg.FileName}（{Markers.Count} 个点位）{suffix}";
            RefreshOutOfBoundsWarning();
            DeleteSelectedCommand.NotifyCanExecuteChanged();
            ClearProductImageCommand.NotifyCanExecuteChanged();
            MatchBoardToImageSizeCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            StatusMessage = $"打开失败：{ex.Message}";
        }
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
        {
            throw new FileNotFoundException(absolutePath);
        }

        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(Path.GetFullPath(absolutePath), UriKind.Absolute);
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        bmp.EndInit();
        bmp.Freeze();

        BoardImageSource = bmp;
        _productImageAbsolutePath = Path.GetFullPath(absolutePath);
        StatusMessage = $"已加载产品底图：{Path.GetFileName(absolutePath)}（{bmp.PixelWidth}×{bmp.PixelHeight}）。可使用「画板=底图像素」对齐坐标。";
    }

    private bool TryLoadProductImageFromDocument(TemplateDocument doc)
    {
        BoardImageSource = null;
        _productImageAbsolutePath = null;

        var resolved = ResolveProductImagePath(doc);
        if (string.IsNullOrEmpty(resolved) || !File.Exists(resolved))
        {
            return false;
        }

        try
        {
            LoadProductImageFromAbsolutePath(resolved);
            return true;
        }
        catch
        {
            BoardImageSource = null;
            _productImageAbsolutePath = null;
            return false;
        }
    }

    private string? ResolveProductImagePath(TemplateDocument doc)
    {
        if (!string.IsNullOrWhiteSpace(doc.ProductImageRelativePath) && !string.IsNullOrWhiteSpace(_templateDirectory))
        {
            var combined = Path.GetFullPath(Path.Combine(_templateDirectory, doc.ProductImageRelativePath));
            if (File.Exists(combined))
            {
                return combined;
            }
        }

        if (!string.IsNullOrWhiteSpace(doc.ProductImageAbsolutePath) && File.Exists(doc.ProductImageAbsolutePath))
        {
            return doc.ProductImageAbsolutePath;
        }

        return null;
    }

    private (string? Relative, string? Absolute) BuildImagePathsForSave()
    {
        if (string.IsNullOrEmpty(_productImageAbsolutePath) || !File.Exists(_productImageAbsolutePath))
        {
            return (null, null);
        }

        if (!string.IsNullOrEmpty(_templateDirectory))
        {
            try
            {
                var rel = Path.GetRelativePath(_templateDirectory, _productImageAbsolutePath);
                if (!rel.StartsWith("..", StringComparison.Ordinal) && !Path.IsPathRooted(rel))
                {
                    return (rel, null);
                }
            }
            catch
            {
                // fall through
            }
        }

        return (null, _productImageAbsolutePath);
    }

    private static ScrewMarkerViewModel MarkerFromRecord(MarkerRecord r, TemplateDocument doc)
    {
        var fallbackDiameter = doc.CircleDiameter > 0 ? doc.CircleDiameter : DefaultScrewType.DiameterPx;
        var diameter = r.CircleDiameter ?? fallbackDiameter;
        var typeId = r.ScrewTypeId ?? ResolveTypeIdByDiameter(diameter);
        return new ScrewMarkerViewModel(r.CenterX, r.CenterY, diameter, typeId);
    }

    private static int ResolveTypeIdByDiameter(double diameterPx)
    {
        return ScrewTypeCatalog.All
            .OrderBy(t => Math.Abs(t.DiameterPx - diameterPx))
            .ThenBy(t => t.Id)
            .First()
            .Id;
    }

    private TemplateDocument ToDocument()
    {
        RenumberMarkers();
        var defaultDiameter = DefaultScrewType.DiameterPx;
        var (rel, abs) = BuildImagePathsForSave();
        return new TemplateDocument
        {
            SchemaVersion = 1,
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

    private void RenumberMarkers()
    {
        for (var i = 0; i < Markers.Count; i++)
        {
            Markers[i].Index = i + 1;
        }
    }

    private void RefreshOutOfBoundsWarning()
    {
        var bad = Markers.Where(m => m.CenterX < 0 || m.CenterY < 0 || m.CenterX > BoardWidth || m.CenterY > BoardHeight).ToList();
        OutOfBoundsWarning = bad.Count == 0
            ? string.Empty
            : $"有 {bad.Count} 个点位超出当前画板范围，请调整尺寸或删除后重新保存。";
    }
}
