using System.Collections.ObjectModel;
using System.IO;
using AutoScrew.Hmi.BusinessDialog;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Models;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace AutoScrew.Hmi.ViewModels;

public partial class ProductTemplateEditorViewModel : ObservableObject
{
    private readonly List<SurfaceLayoutDocument> _surfaceDocuments = new();
    private string? _filePath;
    private string? _templateDirectory;
    private bool _suppressDirty;
    private bool _suppressSelectionRevert;
    private bool _suppressTreeSelectionHandling;
    private SurfaceListItemViewModel? _loadedSurface;
    private readonly LocalizationService _localization;

    public ProductTemplateEditorViewModel(
        SurfaceBoardEditorViewModel currentSurfaceEditor,
        LocalizationService localization)
    {
        CurrentSurfaceEditor = currentSurfaceEditor;
        _localization = localization;
        CurrentSurfaceEditor.ContentChanged += (_, _) => MarkDirty();
        _localization.CultureChanged += (_, _) => RefreshLocalizedUi();
        ProductTreeRoots = new ObservableCollection<ProductTemplateTreeRootViewModel>();
        StatusMessage = Loc.Get("S.Template.StatusInitial");
    }

    public SurfaceBoardEditorViewModel CurrentSurfaceEditor { get; }

    public ObservableCollection<ProductTemplateTreeRootViewModel> ProductTreeRoots { get; }

    public ProductTemplateTreeRootViewModel? ProductRoot =>
        ProductTreeRoots.Count > 0 ? ProductTreeRoots[0] : null;

    [ObservableProperty]
    private string _productId = "";

    [ObservableProperty]
    private string _displayName = "";

    [ObservableProperty]
    private string _revision = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowTitle))]
    [NotifyPropertyChangedFor(nameof(HasProduct))]
    private bool _isDirty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSurfaceSelected))]
    [NotifyCanExecuteChangedFor(nameof(EditSurfaceCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteSurfaceCommand))]
    [NotifyCanExecuteChangedFor(nameof(MoveSurfaceUpCommand))]
    [NotifyCanExecuteChangedFor(nameof(MoveSurfaceDownCommand))]
    private SurfaceListItemViewModel? _selectedSurface;

    [ObservableProperty]
    private string _statusMessage = "";

    public bool HasProduct => ProductRoot is not null;

    public bool IsSurfaceSelected => SelectedSurface is not null;

    public string WindowTitle =>
        string.IsNullOrWhiteSpace(ProductId)
            ? Loc.Get("S.Template.Title")
            : Loc.Format("S.Template.TitleWithProduct", ProductId, IsDirty ? " *" : "");

    partial void OnSelectedSurfaceChanged(SurfaceListItemViewModel? value)
    {
        if (_suppressSelectionRevert)
            return;

        if (value is null)
            return;

        if (ReferenceEquals(value, _loadedSurface))
            return;

        if (_loadedSurface is not null && IsDirty && !ConfirmDiscardChanges())
        {
            _suppressSelectionRevert = true;
            SelectedSurface = _loadedSurface;
            _suppressSelectionRevert = false;
            return;
        }

        if (_loadedSurface is not null && !IsDirty)
            FlushSurfaceToModel(_loadedSurface);

        LoadSurfaceIntoEditor(value);
        _loadedSurface = value;
        IsDirty = false;
    }

    public void HandleTreeSelection(object? selectedItem)
    {
        if (_suppressTreeSelectionHandling)
            return;

        if (selectedItem is SurfaceListItemViewModel surface)
        {
            SelectedSurface = surface;
            return;
        }

        if (selectedItem is ProductTemplateTreeRootViewModel)
        {
            if (_loadedSurface is not null)
                FlushSurfaceToModel(_loadedSurface);

            _suppressSelectionRevert = true;
            SelectedSurface = null;
            _suppressSelectionRevert = false;
            if (HasProduct)
                StatusMessage = Loc.Format("S.Template.StatusSelectSurface", ProductRoot!.TreeHeader, ProductRoot.Surfaces.Count);
        }
    }

    [RelayCommand]
    private void OpenProduct()
    {
        if (IsDirty && !ConfirmDiscardChanges())
            return;

        var dlg = new OpenFileDialog
        {
            Filter = Loc.Get("S.Template.OpenFilter"),
        };

        if (dlg.ShowDialog() != true)
            return;

        try
        {
            RunWithTreeSelectionSuppressed(() =>
            {
                var doc = ProductTemplateJsonSerializer.Load(dlg.FileName);
                _filePath = dlg.FileName;
                _templateDirectory = Path.GetDirectoryName(dlg.FileName);
                LoadProductDocument(doc);
                IsDirty = false;
                StatusMessage = Loc.Format("S.Template.StatusOpened", dlg.FileName, doc.Surfaces.Count);
            });
        }
        catch (Exception ex)
        {
            StatusMessage = Loc.Format("S.Template.StatusOpenFailed", ex.Message);
        }
    }

    [RelayCommand]
    private void SaveProduct()
    {
        if (ProductRoot is null || string.IsNullOrWhiteSpace(ProductId))
        {
            StatusMessage = Loc.Get("S.Template.StatusNoProduct");
            return;
        }

        RunWithTreeSelectionSuppressed(() =>
        {
            FlushLoadedSurfaceToModel();
            NormalizeSurfaceOrders();

            if (string.IsNullOrWhiteSpace(_filePath))
            {
                var dlg = new SaveFileDialog
                {
                    Filter = Loc.Get("S.Template.SaveFilter"),
                    DefaultExt = ".product-template.json",
                    FileName = $"{ProductId}.product-template.json",
                };

                if (dlg.ShowDialog() != true)
                    return;

                _filePath = dlg.FileName;
                _templateDirectory = Path.GetDirectoryName(dlg.FileName);
            }

            try
            {
                var doc = BuildProductDocument();
                ProductTemplateJsonSerializer.Save(_filePath!, doc);
                IsDirty = false;
                RefreshTreeEditStates();
                StatusMessage = Loc.Format("S.Template.StatusSaved", _filePath);
            }
            catch (Exception ex)
            {
                StatusMessage = Loc.Format("S.Template.StatusSaveFailed", ex.Message);
            }
        });
    }

    [RelayCommand]
    private void NewProduct()
    {
        if (IsDirty && !ConfirmDiscardChanges())
            return;

        ProductInfoResult? info = null;
        if (!RunWithTreeSelectionSuppressed(() => ProductInfoDialog.TryShow(null, out info)))
            return;

        ResetSession();
        ApplyProductInfo(info!);
        IsDirty = true;
        StatusMessage = Loc.Get("S.Template.StatusCreated");
    }

    [RelayCommand]
    private void AddSurface()
    {
        if (!EnsureProductInfo())
            return;

        var surfaces = ProductRoot!.Surfaces;
        var nextIndex = surfaces.Count + 1;
        var maxOrder = surfaces.Count == 0 ? 0 : surfaces.Max(s => s.Order);
        var initial = new SurfaceParamsResult(
            $"S{nextIndex}",
            Loc.Format("S.Template.DefaultSurfaceName", nextIndex),
            maxOrder + 1,
            800,
            600);

        SurfaceParamsResult? result = null;
        if (!RunWithTreeSelectionSuppressed(() => SurfaceParamsDialog.TryShow(
                initial,
                GetExistingSurfaceIds(),
                excludeId: null,
                title: Loc.Get("S.Template.AddSurfaceDialog"),
                out result)))
            return;

        var doc = CreateSurfaceDocument(result!);
        var item = CreateSurfaceListItem(result!);
        var insertAt = ComputeInsertIndex(surfaces, item.Order);
        _surfaceDocuments.Insert(insertAt, doc);
        surfaces.Insert(insertAt, item);
        SelectSurfaceInTree(item);
        IsDirty = true;
        StatusMessage = Loc.Format("S.Template.StatusSurfaceAdded", item.Name);
    }

    [RelayCommand(CanExecute = nameof(CanEditSelectedSurface))]
    private void EditSurface()
    {
        if (SelectedSurface is null)
            return;

        var index = GetSurfaceIndex(SelectedSurface);
        if (index < 0)
            return;

        var doc = _surfaceDocuments[index];
        var initial = new SurfaceParamsResult(
            SelectedSurface.SurfaceId,
            SelectedSurface.Name,
            SelectedSurface.Order,
            doc.BoardWidth > 0 ? doc.BoardWidth : 800,
            doc.BoardHeight > 0 ? doc.BoardHeight : 600);

        SurfaceParamsResult? result = null;
        if (!RunWithTreeSelectionSuppressed(() => SurfaceParamsDialog.TryShow(
                initial,
                GetExistingSurfaceIds(),
                excludeId: SelectedSurface.SurfaceId,
                title: Loc.Get("S.Template.EditSurfaceDialog"),
                out result)))
            return;

        ApplySurfaceParams(index, SelectedSurface, result!);
        if (ReferenceEquals(_loadedSurface, SelectedSurface))
            LoadSurfaceIntoEditor(SelectedSurface);

        IsDirty = true;
        StatusMessage = Loc.Format("S.Template.StatusSurfaceUpdated", SelectedSurface.Name);
    }

    [RelayCommand(CanExecute = nameof(CanEditSelectedSurface))]
    private void DeleteSurface()
    {
        if (SelectedSurface is null || ProductRoot is null)
            return;

        if (!ConfirmTips.ShowDialog(Loc.Format("S.Template.ConfirmDeleteSurface", SelectedSurface.Name)))
            return;

        var index = GetSurfaceIndex(SelectedSurface);
        if (index < 0)
            return;

        var surfaces = ProductRoot.Surfaces;
        var selectIndex = Math.Min(index, surfaces.Count - 2);
        _surfaceDocuments.RemoveAt(index);
        surfaces.RemoveAt(index);

        _loadedSurface = null;
        _suppressSelectionRevert = true;
        SelectedSurface = selectIndex >= 0 ? surfaces[selectIndex] : null;
        _suppressSelectionRevert = false;

        if (SelectedSurface is null)
            ClearBoardEditor();

        IsDirty = true;
        StatusMessage = Loc.Get("S.Template.StatusSurfaceDeleted");
    }

    [RelayCommand(CanExecute = nameof(CanMoveSurfaceUp))]
    private void MoveSurfaceUp()
    {
        MoveSurface(-1);
    }

    [RelayCommand(CanExecute = nameof(CanMoveSurfaceDown))]
    private void MoveSurfaceDown()
    {
        MoveSurface(1);
    }

    private bool CanEditSelectedSurface() => SelectedSurface is not null;

    private bool CanMoveSurfaceUp()
    {
        if (SelectedSurface is null || ProductRoot is null)
            return false;

        var surfaces = ProductRoot.Surfaces;
        var index = surfaces.IndexOf(SelectedSurface);
        return index > 0;
    }

    private bool CanMoveSurfaceDown()
    {
        if (SelectedSurface is null || ProductRoot is null)
            return false;

        var surfaces = ProductRoot.Surfaces;
        var index = surfaces.IndexOf(SelectedSurface);
        return index >= 0 && index < surfaces.Count - 1;
    }

    private void MoveSurface(int delta)
    {
        if (SelectedSurface is null || ProductRoot is null)
            return;

        var surfaces = ProductRoot.Surfaces;
        var index = surfaces.IndexOf(SelectedSurface);
        var otherIndex = index + delta;
        if (index < 0 || otherIndex < 0 || otherIndex >= surfaces.Count)
            return;

        FlushCurrentSurfaceToModel();

        var currentOrder = surfaces[index].Order;
        surfaces[index].Order = surfaces[otherIndex].Order;
        surfaces[otherIndex].Order = currentOrder;

        _surfaceDocuments[index].Order = surfaces[index].Order;
        _surfaceDocuments[otherIndex].Order = surfaces[otherIndex].Order;

        surfaces.Move(index, otherIndex);
        (_surfaceDocuments[index], _surfaceDocuments[otherIndex]) = (_surfaceDocuments[otherIndex], _surfaceDocuments[index]);

        IsDirty = true;
        StatusMessage = Loc.Format("S.Template.StatusOrderChanged", SelectedSurface.Name, SelectedSurface.Order);
        MoveSurfaceUpCommand.NotifyCanExecuteChanged();
        MoveSurfaceDownCommand.NotifyCanExecuteChanged();
    }

    private bool EnsureProductInfo()
    {
        if (ProductRoot is not null)
            return true;

        ProductInfoResult? info = null;
        if (!RunWithTreeSelectionSuppressed(() => ProductInfoDialog.TryShow(null, out info)))
            return false;

        ApplyProductInfo(info!);
        IsDirty = true;
        return true;
    }

    private void ApplyProductInfo(ProductInfoResult info)
    {
        ProductId = info.ProductId;
        DisplayName = info.DisplayName;
        Revision = info.Revision ?? "";

        ProductTreeRoots.Clear();
        ProductTreeRoots.Add(new ProductTemplateTreeRootViewModel(ProductId, DisplayName, Revision));
        OnPropertyChanged(nameof(ProductRoot));
        OnPropertyChanged(nameof(HasProduct));
    }

    private void ResetSession()
    {
        _filePath = null;
        _templateDirectory = null;
        _surfaceDocuments.Clear();
        ProductTreeRoots.Clear();
        ProductId = "";
        DisplayName = "";
        Revision = "";
        _loadedSurface = null;
        _suppressSelectionRevert = true;
        SelectedSurface = null;
        _suppressSelectionRevert = false;
        ClearBoardEditor();
        OnPropertyChanged(nameof(ProductRoot));
        OnPropertyChanged(nameof(HasProduct));
    }

    private void LoadProductDocument(ProductTemplateDocument doc)
    {
        _suppressDirty = true;
        ProductId = doc.ProductId;
        DisplayName = doc.DisplayName ?? doc.ProductId;
        Revision = doc.Revision ?? "";

        _surfaceDocuments.Clear();
        _surfaceDocuments.AddRange(doc.Surfaces.OrderBy(s => s.Order).ThenBy(s => s.SurfaceId));

        ProductTreeRoots.Clear();
        var root = new ProductTemplateTreeRootViewModel(ProductId, DisplayName, Revision);
        foreach (var s in _surfaceDocuments)
        {
            root.Surfaces.Add(new SurfaceListItemViewModel(s.SurfaceId, s.Name, s.Order)
            {
                MarkerCount = s.Markers.Count,
                EditState = s.Markers.Count > 0 ? SurfaceEditState.Valid : SurfaceEditState.Empty,
            });
        }

        ProductTreeRoots.Add(root);
        CurrentSurfaceEditor.SetTemplateDirectory(_templateDirectory);
        _loadedSurface = null;
        SelectedSurface = root.Surfaces.FirstOrDefault();
        RefreshTreeEditStates();
        _suppressDirty = false;
        OnPropertyChanged(nameof(ProductRoot));
        OnPropertyChanged(nameof(HasProduct));
    }

    private void LoadSurfaceIntoEditor(SurfaceListItemViewModel item)
    {
        var index = GetSurfaceIndex(item);
        if (index < 0)
            return;

        _suppressDirty = true;
        CurrentSurfaceEditor.SetTemplateDirectory(_templateDirectory);
        CurrentSurfaceEditor.LoadFrom(_surfaceDocuments[index], _templateDirectory);
        item.MarkerCount = _surfaceDocuments[index].Markers.Count;
        StatusMessage = Loc.Format("S.Template.StatusEditing", item.Name, item.MarkerCount);
        _suppressDirty = false;
    }

    private void FlushLoadedSurfaceToModel()
    {
        if (_loadedSurface is not null)
            FlushSurfaceToModel(_loadedSurface);
    }

    private void FlushSurfaceToModel(SurfaceListItemViewModel surface)
    {
        var index = GetSurfaceIndex(surface);
        if (index < 0)
            return;

        _surfaceDocuments[index] = CurrentSurfaceEditor.ToDocument(
            surface.SurfaceId,
            surface.Name,
            surface.Order);
        surface.MarkerCount = _surfaceDocuments[index].Markers.Count;
        surface.EditState = surface.MarkerCount > 0 ? SurfaceEditState.Edited : SurfaceEditState.Empty;
    }

    private void FlushCurrentSurfaceToModel() => FlushLoadedSurfaceToModel();

    private void ApplySurfaceParams(int index, SurfaceListItemViewModel item, SurfaceParamsResult result)
    {
        var doc = _surfaceDocuments[index];
        doc.SurfaceId = result.SurfaceId;
        doc.Name = result.Name;
        doc.Order = result.Order;
        doc.BoardWidth = result.BoardWidth;
        doc.BoardHeight = result.BoardHeight;

        item.SurfaceId = result.SurfaceId;
        item.Name = result.Name;
        item.Order = result.Order;

        if (ProductRoot is not null)
            ReorderSurfacesCollection(ProductRoot.Surfaces);
    }

    private static SurfaceLayoutDocument CreateSurfaceDocument(SurfaceParamsResult result) =>
        new()
        {
            SurfaceId = result.SurfaceId,
            Name = result.Name,
            Order = result.Order,
            BoardWidth = result.BoardWidth,
            BoardHeight = result.BoardHeight,
        };

    private static int ComputeInsertIndex(ObservableCollection<SurfaceListItemViewModel> surfaces, int order)
    {
        var insertAt = 0;
        while (insertAt < surfaces.Count && surfaces[insertAt].Order <= order)
            insertAt++;
        return insertAt;
    }

    private static SurfaceListItemViewModel CreateSurfaceListItem(SurfaceParamsResult result) =>
        new(result.SurfaceId, result.Name, result.Order);

    private void ReorderSurfacesCollection(ObservableCollection<SurfaceListItemViewModel> surfaces)
    {
        var ordered = surfaces.OrderBy(s => s.Order).ThenBy(s => s.SurfaceId, StringComparer.OrdinalIgnoreCase).ToList();
        surfaces.Clear();
        foreach (var item in ordered)
            surfaces.Add(item);

        _surfaceDocuments.Sort((a, b) =>
        {
            var c = a.Order.CompareTo(b.Order);
            return c != 0 ? c : string.Compare(a.SurfaceId, b.SurfaceId, StringComparison.OrdinalIgnoreCase);
        });
    }

    private void NormalizeSurfaceOrders()
    {
        if (ProductRoot is null)
            return;

        var surfaces = ProductRoot.Surfaces.OrderBy(s => s.Order).ThenBy(s => s.SurfaceId, StringComparer.OrdinalIgnoreCase).ToList();
        for (var i = 0; i < surfaces.Count; i++)
        {
            var order = i + 1;
            surfaces[i].Order = order;
            var docIndex = GetSurfaceIndex(surfaces[i]);
            if (docIndex >= 0)
                _surfaceDocuments[docIndex].Order = order;
        }

        ReorderSurfacesCollection(ProductRoot.Surfaces);
    }

    private int GetSurfaceIndex(SurfaceListItemViewModel item)
    {
        if (ProductRoot is null)
            return -1;

        return ProductRoot.Surfaces.IndexOf(item);
    }

    private IReadOnlyCollection<string> GetExistingSurfaceIds() =>
        ProductRoot?.Surfaces.Select(s => s.SurfaceId).ToList() ?? [];

    private void SelectSurfaceInTree(SurfaceListItemViewModel item)
    {
        SelectedSurface = item;
    }

    private void ClearBoardEditor()
    {
        _suppressDirty = true;
        CurrentSurfaceEditor.ClearBoard();
        _suppressDirty = false;
    }

    private ProductTemplateDocument BuildProductDocument()
    {
        return new ProductTemplateDocument
        {
            ProductId = ProductId.Trim(),
            DisplayName = string.IsNullOrWhiteSpace(DisplayName) ? ProductId.Trim() : DisplayName.Trim(),
            Revision = string.IsNullOrWhiteSpace(Revision) ? null : Revision.Trim(),
            SurfaceCount = _surfaceDocuments.Count,
            Surfaces = _surfaceDocuments.OrderBy(s => s.Order).ThenBy(s => s.SurfaceId).ToList(),
        };
    }

    private void RefreshTreeEditStates()
    {
        if (ProductRoot is null)
            return;

        for (var i = 0; i < ProductRoot.Surfaces.Count && i < _surfaceDocuments.Count; i++)
        {
            ProductRoot.Surfaces[i].MarkerCount = _surfaceDocuments[i].Markers.Count;
            ProductRoot.Surfaces[i].EditState = _surfaceDocuments[i].Markers.Count > 0
                ? SurfaceEditState.Valid
                : SurfaceEditState.Empty;
        }
    }

    private void MarkDirty()
    {
        if (_suppressDirty)
            return;

        IsDirty = true;
        if (_loadedSurface is not null)
            _loadedSurface.EditState = SurfaceEditState.Edited;
        else if (SelectedSurface is not null)
            SelectedSurface.EditState = SurfaceEditState.Edited;
    }

    private void RefreshLocalizedUi()
    {
        OnPropertyChanged(nameof(WindowTitle));
        if (!HasProduct)
        {
            StatusMessage = Loc.Get("S.Template.StatusInitial");
            return;
        }

        if (_loadedSurface is not null)
            StatusMessage = Loc.Format("S.Template.StatusEditing", _loadedSurface.Name, _loadedSurface.MarkerCount);
        else if (ProductRoot is not null)
            StatusMessage = Loc.Format("S.Template.StatusSelectSurface", ProductRoot.TreeHeader, ProductRoot.Surfaces.Count);
    }

    private static bool ConfirmDiscardChanges() =>
        ConfirmTips.ShowDialog(Loc.Get("S.Template.ConfirmDiscard"));

    private void RunWithTreeSelectionSuppressed(Action action)
    {
        _suppressTreeSelectionHandling = true;
        try
        {
            action();
        }
        finally
        {
            _suppressTreeSelectionHandling = false;
        }
    }

    private bool RunWithTreeSelectionSuppressed(Func<bool> func)
    {
        _suppressTreeSelectionHandling = true;
        try
        {
            return func();
        }
        finally
        {
            _suppressTreeSelectionHandling = false;
        }
    }
}
