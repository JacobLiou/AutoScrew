using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class ProcessLibrarySlotPickerDialog : Window, INotifyPropertyChanged
{
    private readonly IProcessLibraryService _library;
    private string _productPn = string.Empty;
    private SlotRow? _selectedSlot;
    private bool _suppressProductReload;

    public ProcessLibrarySlotPickerDialog(IProcessLibraryService library)
    {
        _library = library;
        InitializeComponent();
        Title = Loc.Get("S.ControllerParam.ImportProcessLibraryTitle");
        DataContext = this;
        Loaded += async (_, _) => await InitializeAsync().ConfigureAwait(true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> ProductPns { get; } = [];

    public ObservableCollection<SlotRow> Slots { get; } = [];

    public string ProductPn
    {
        get => _productPn;
        set
        {
            if (_productPn == value)
                return;
            _productPn = value ?? string.Empty;
            OnPropertyChanged();
            if (!_suppressProductReload)
                _ = ReloadSlotsAsync();
        }
    }

    public SlotRow? SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            if (ReferenceEquals(_selectedSlot, value))
                return;
            _selectedSlot = value;
            OnPropertyChanged();
        }
    }

    public bool Confirmed { get; private set; }

    public bool ImportAll { get; private set; }

    public string? ConfirmedProductPn { get; private set; }

    public IReadOnlyList<int> ConfirmedSlotIds { get; private set; } = [];

    private async Task InitializeAsync()
    {
        try
        {
            var list = await _library.ListProductPnsAsync().ConfigureAwait(true);
            ProductPns.Clear();
            foreach (var pn in list)
                ProductPns.Add(pn);

            if (ProductPns.Count > 0)
            {
                _suppressProductReload = true;
                ProductPn = ProductPns[0];
                _suppressProductReload = false;
                await ReloadSlotsAsync().ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async Task ReloadSlotsAsync()
    {
        Slots.Clear();
        SelectedSlot = null;
        if (string.IsNullOrWhiteSpace(ProductPn))
            return;

        try
        {
            var product = await _library.GetProductAsync(ProductPn.Trim()).ConfigureAwait(true);
            if (product is null)
                return;

            foreach (var s in product.Slots.OrderBy(x => x.SlotId))
                Slots.Add(new SlotRow(s));

            if (Slots.Count > 0)
                SelectedSlot = Slots[0];
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ConfirmSelection()
    {
        if (string.IsNullOrWhiteSpace(ProductPn) || Slots.Count == 0)
            return;

        var ids = SlotList.SelectedItems.OfType<SlotRow>().Select(s => s.SlotId).ToList();
        if (ids.Count == 0 && SelectedSlot is not null)
            ids.Add(SelectedSlot.SlotId);
        if (ids.Count == 0)
            return;

        ConfirmedProductPn = ProductPn.Trim();
        ConfirmedSlotIds = ids;
        ImportAll = false;
        Confirmed = true;
        DialogResult = true;
        Close();
    }

    private void ImportAll_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProductPn) || Slots.Count == 0)
            return;

        ConfirmedProductPn = ProductPn.Trim();
        ConfirmedSlotIds = [];
        ImportAll = true;
        Confirmed = true;
        DialogResult = true;
        Close();
    }

    private void SlotList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) => ConfirmSelection();

    private void SlotList_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            ConfirmSelection();
    }

    private void Select_Click(object sender, RoutedEventArgs e) => ConfirmSelection();

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        DialogResult = false;
        Close();
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public sealed class SlotRow
    {
        public SlotRow(ProcessLibrarySlotInfo info)
        {
            SlotId = info.SlotId;
            ScrewPn = info.ScrewPn;
            FileName = info.FileName;
            DeviceParameterId = info.DeviceParameterId > 0
                ? info.DeviceParameterId
                : info.SlotId + 1;
        }

        public int SlotId { get; }
        public int DeviceParameterId { get; }
        public string ScrewPn { get; }
        public string FileName { get; }
        public string SlotLabel => SlotId.ToString("D2");
        public string DeviceIdLabel => DeviceParameterId.ToString();
    }
}
