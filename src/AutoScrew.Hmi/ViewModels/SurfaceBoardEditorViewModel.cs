using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoScrew.Application.Editing;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Models;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;
using CommunityToolkit.Mvvm.Input;

namespace AutoScrew.Hmi.ViewModels;

/// <summary>单面画板编辑（尺寸、底图、螺钉标注）。</summary>
public partial class SurfaceBoardEditorViewModel : ObservableObject
{
    public static ScrewTypePreset DefaultScrewType => ScrewTypeCatalog.Default;

    public IReadOnlyList<ScrewTypePreset> ScrewTypes => ScrewTypeCatalog.All;

    private readonly LocalizationService _localization;
    private readonly IUserAuditService? _audit;
    private readonly IOptions<AutoScrewAppOptions>? _appOptions;
    private readonly ICurrentUser? _user;
    private string? _templateDirectory;
    private string? _productImageAbsolutePath;
    private string? _currentSurfaceId;
    private string? _loadedSurfaceName;

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

    public SurfaceBoardEditorViewModel(LocalizationService localization)
        : this(localization, null, null, null)
    {
    }

    public SurfaceBoardEditorViewModel(
        LocalizationService localization,
        IUserAuditService? audit,
        IOptions<AutoScrewAppOptions>? appOptions,
        ICurrentUser? user)
    {
        _localization = localization;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        _localization.CultureChanged += (_, _) => RefreshLocalizedMessages();
    }

    protected void AuditBoard(string action, string? detail = null)
    {
        if (_audit is null || _appOptions is null || _user is null)
            return;

        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, action, detail: detail);
    }

    public void LoadFrom(SurfaceLayoutDocument surface, string? templateDirectory)
    {
        _templateDirectory = templateDirectory;
        _currentSurfaceId = surface.SurfaceId;
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
        _loadedSurfaceName = surface.Name;
        StatusMessage = Loc.Format("S.Template.BoardLoaded", surface.Name, Markers.Count);
    }

    public SurfaceLayoutDocument ToDocument(string surfaceId, string name, int order)
    {
        RenumberMarkers();
        var defaultDiameter = DefaultScrewType.DiameterPx;
        var (rel, abs) = BuildImagePathsForSave(surfaceId);
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
        _loadedSurfaceName = null;
        _currentSurfaceId = null;
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        ClearProductImageCommand.NotifyCanExecuteChanged();
        MatchBoardToImageSizeCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void ApplyBoardSize()
    {
        AuditBoard("Configuration.BoardApplySize", $"{BoardWidthInput}x{BoardHeightInput}");
        if (!double.TryParse(BoardWidthInput, out var w) || w <= 0 || w > 32_000)
        {
            StatusMessage = Loc.Get("S.Template.BoardWidthInvalid");
            return;
        }

        if (!double.TryParse(BoardHeightInput, out var h) || h <= 0 || h > 32_000)
        {
            StatusMessage = Loc.Get("S.Template.BoardHeightInvalid");
            return;
        }

        BoardWidth = w;
        BoardHeight = h;
        StatusMessage = Loc.Format("S.Template.BoardSizeApplied", w, h);
        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    [RelayCommand]
    private void LoadProductImage()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = Loc.Get("S.Template.ImageFilter"),
        };

        if (dlg.ShowDialog() != true)
            return;

        AuditBoard("Configuration.BoardLoadImage", dlg.FileName);
        try
        {
            LoadProductImageFromAbsolutePath(dlg.FileName, importToTemplate: true);
            RaiseContentChanged();
        }
        catch (Exception ex)
        {
            StatusMessage = Loc.Format("S.Template.ImageLoadFailed", ex.Message);
        }
    }

    [RelayCommand(CanExecute = nameof(CanClearProductImage))]
    private void ClearProductImage()
    {
        AuditBoard("Configuration.BoardClearImage");
        BoardImageSource = null;
        _productImageAbsolutePath = null;
        BoardImageOpacity = 1.0;
        StatusMessage = Loc.Get("S.Template.ImageCleared");
        RaiseContentChanged();
    }

    private bool CanClearProductImage() => BoardImageSource is not null;

    [RelayCommand(CanExecute = nameof(CanMatchBoardToImage))]
    private void MatchBoardToImageSize()
    {
        AuditBoard("Configuration.BoardMatchImageSize");
        if (BoardImageSource is not BitmapImage bmp)
            return;

        BoardWidth = bmp.PixelWidth;
        BoardHeight = bmp.PixelHeight;
        BoardWidthInput = BoardWidth.ToString("0.##");
        BoardHeightInput = BoardHeight.ToString("0.##");
        StatusMessage = Loc.Format("S.Template.BoardMatched", bmp.PixelWidth, bmp.PixelHeight);
        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    private bool CanMatchBoardToImage() => BoardImageSource is BitmapImage;

    public void AddMarkerAt(double centerX, double centerY)
    {
        if (centerX < 0 || centerY < 0 || centerX > BoardWidth || centerY > BoardHeight)
        {
            StatusMessage = Loc.Get("S.Template.MarkerOutOfBounds");
            return;
        }

        var marker = new ScrewMarkerViewModel(centerX, centerY, DefaultScrewType);
        Markers.Add(marker);
        RenumberMarkers();
        SelectSingle(marker);
        StatusMessage = Loc.Format("S.Template.MarkerAdded", marker.Index);
        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    private void SelectSingle(ScrewMarkerViewModel marker)
    {
        foreach (var m in Markers)
            m.IsSelected = ReferenceEquals(m, marker);

        DeleteSelectedCommand.NotifyCanExecuteChanged();
        NudgeSelectedMarkersCommand.NotifyCanExecuteChanged();
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
        NudgeSelectedMarkersCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void SelectAllMarkers()
    {
        foreach (var m in Markers)
            m.IsSelected = true;

        DeleteSelectedCommand.NotifyCanExecuteChanged();
        NudgeSelectedMarkersCommand.NotifyCanExecuteChanged();
        StatusMessage = Markers.Count == 0
            ? Loc.Get("S.Template.NoMarkers")
            : Loc.Format("S.Template.AllSelected", Markers.Count);
    }

    [RelayCommand(CanExecute = nameof(CanDeleteSelected))]
    private void DeleteSelected()
    {
        var toRemove = Markers.Where(m => m.IsSelected).ToList();
        if (toRemove.Count == 0)
            return;

        AuditBoard("Configuration.BoardDeleteMarkers", $"count={toRemove.Count}");

        foreach (var m in toRemove)
            Markers.Remove(m);

        RenumberMarkers();
        StatusMessage = Loc.Format("S.Template.MarkersDeleted", toRemove.Count);
        RefreshOutOfBoundsWarning();
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        NudgeSelectedMarkersCommand.NotifyCanExecuteChanged();
        RaiseContentChanged();
    }

    private bool CanDeleteSelected() => Markers.Any(m => m.IsSelected);

    [RelayCommand]
    private void ClearSelection()
    {
        foreach (var m in Markers)
            m.IsSelected = false;

        DeleteSelectedCommand.NotifyCanExecuteChanged();
        NudgeSelectedMarkersCommand.NotifyCanExecuteChanged();
    }

    public void MoveSelectedMarkersBy(double deltaX, double deltaY)
    {
        var selected = Markers.Where(m => m.IsSelected).ToList();
        if (selected.Count == 0)
            return;

        foreach (var marker in selected)
        {
            var (x, y) = BoardMarkerMovement.ApplyDelta(
                marker.CenterX,
                marker.CenterY,
                deltaX,
                deltaY,
                BoardWidth,
                BoardHeight);
            marker.CenterX = x;
            marker.CenterY = y;
        }

        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    public void SetAnchorMarkerCenter(double centerX, double centerY, ScrewMarkerViewModel anchor)
    {
        var deltaX = centerX - anchor.CenterX;
        var deltaY = centerY - anchor.CenterY;
        if (Math.Abs(deltaX) < double.Epsilon && Math.Abs(deltaY) < double.Epsilon)
            return;

        var targets = anchor.IsSelected
            ? Markers.Where(m => m.IsSelected).ToList()
            : [anchor];

        foreach (var marker in targets)
        {
            var (x, y) = BoardMarkerMovement.ApplyDelta(
                marker.CenterX,
                marker.CenterY,
                deltaX,
                deltaY,
                BoardWidth,
                BoardHeight);
            marker.CenterX = x;
            marker.CenterY = y;
        }

        RefreshOutOfBoundsWarning();
        RaiseContentChanged();
    }

    [RelayCommand(CanExecute = nameof(CanNudgeSelected))]
    private void NudgeSelectedMarkers(NudgeDirection direction)
    {
        var step = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? 10.0 : 1.0;
        var (dx, dy) = direction switch
        {
            NudgeDirection.Left => (-step, 0.0),
            NudgeDirection.Right => (step, 0.0),
            NudgeDirection.Up => (0.0, -step),
            NudgeDirection.Down => (0.0, step),
            _ => (0.0, 0.0),
        };

        MoveSelectedMarkersBy(dx, dy);
    }

    private bool CanNudgeSelected() => Markers.Any(m => m.IsSelected);

    public void NotifyDeleteCommandCanExecute() => DeleteSelectedCommand.NotifyCanExecuteChanged();

    partial void OnBoardImageSourceChanged(ImageSource? value)
    {
        ClearProductImageCommand.NotifyCanExecuteChanged();
        MatchBoardToImageSizeCommand.NotifyCanExecuteChanged();
    }

    private void LoadProductImageFromAbsolutePath(string absolutePath, bool importToTemplate = false)
    {
        if (!File.Exists(absolutePath))
            throw new FileNotFoundException(absolutePath);

        var resolvedPath = absolutePath;
        string? importedRelative = null;
        if (importToTemplate
            && !string.IsNullOrWhiteSpace(_templateDirectory)
            && !string.IsNullOrWhiteSpace(_currentSurfaceId))
        {
            var (relative, storedAbsolute) = ProductTemplatePathHelper.EnsureImageInTemplateFolder(
                absolutePath,
                _templateDirectory,
                _currentSurfaceId);
            if (!string.IsNullOrEmpty(storedAbsolute))
                resolvedPath = storedAbsolute;
            importedRelative = relative;
        }

        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(Path.GetFullPath(resolvedPath), UriKind.Absolute);
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        bmp.EndInit();
        bmp.Freeze();

        BoardImageSource = bmp;
        _productImageAbsolutePath = Path.GetFullPath(resolvedPath);
        if (importToTemplate && !string.IsNullOrEmpty(importedRelative))
            StatusMessage = Loc.Format("S.Template.ImageImportedToTemplate", importedRelative);
        else if (importToTemplate
                 && (string.IsNullOrWhiteSpace(_templateDirectory) || string.IsNullOrWhiteSpace(_currentSurfaceId)))
            StatusMessage = Loc.Format("S.Template.ImageLoaded", Path.GetFileName(absolutePath))
                            + " " + Loc.Get("S.Template.ImageNeedsProductFolder");
        else
            StatusMessage = Loc.Format("S.Template.ImageLoaded", Path.GetFileName(resolvedPath));
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

    private (string? Relative, string? Absolute) BuildImagePathsForSave(string surfaceId) =>
        ProductTemplatePathHelper.BuildImagePathsForSave(_productImageAbsolutePath, _templateDirectory, surfaceId);

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
            : Loc.Format("S.Template.MarkersOutOfBounds", bad);
    }

    private void RefreshLocalizedMessages()
    {
        RefreshOutOfBoundsWarning();
        if (!string.IsNullOrEmpty(_loadedSurfaceName) && Markers.Count >= 0)
            StatusMessage = Loc.Format("S.Template.BoardLoaded", _loadedSurfaceName, Markers.Count);
    }

    private void RaiseContentChanged()
    {
        ContentChanged?.Invoke(this, EventArgs.Empty);
    }
}
