using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AutoScrew.Application.Abstractions;
using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.Views.ControllerDevice;

public partial class ProcessLibrarySequencePickerDialog : Window, INotifyPropertyChanged
{
    private readonly IProcessLibraryService _library;
    private string _productPn = string.Empty;
    private SequenceRow? _selectedSequence;
    private bool _suppressProductReload;

    public ProcessLibrarySequencePickerDialog(IProcessLibraryService library)
    {
        _library = library;
        InitializeComponent();
        Title = Loc.Get("S.ControllerSeq.ImportProcessLibraryTitle");
        DataContext = this;
        Loaded += async (_, _) => await InitializeAsync().ConfigureAwait(true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> ProductPns { get; } = [];

    public ObservableCollection<SequenceRow> Sequences { get; } = [];

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
                _ = ReloadSequencesAsync();
        }
    }

    public SequenceRow? SelectedSequence
    {
        get => _selectedSequence;
        set
        {
            if (ReferenceEquals(_selectedSequence, value))
                return;
            _selectedSequence = value;
            OnPropertyChanged();
        }
    }

    public bool Confirmed { get; private set; }

    public string? ConfirmedProductPn { get; private set; }

    public int ConfirmedSequenceId { get; private set; }

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
                await ReloadSequencesAsync().ConfigureAwait(true);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async Task ReloadSequencesAsync()
    {
        Sequences.Clear();
        SelectedSequence = null;
        if (string.IsNullOrWhiteSpace(ProductPn))
            return;

        try
        {
            var product = await _library.GetProductAsync(ProductPn.Trim()).ConfigureAwait(true);
            if (product is null)
                return;

            foreach (var s in product.Sequences.OrderBy(x => x.SequenceId))
                Sequences.Add(new SequenceRow(s));

            if (Sequences.Count > 0)
                SelectedSequence = Sequences[0];
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ConfirmSelection()
    {
        if (SelectedSequence is null || string.IsNullOrWhiteSpace(ProductPn))
            return;

        ConfirmedProductPn = ProductPn.Trim();
        ConfirmedSequenceId = SelectedSequence.SequenceId;
        Confirmed = true;
        DialogResult = true;
        Close();
    }

    private void SequenceList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) => ConfirmSelection();

    private void SequenceList_OnKeyDown(object sender, KeyEventArgs e)
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

    public sealed class SequenceRow
    {
        public SequenceRow(ProcessLibrarySequenceInfo info)
        {
            SequenceId = info.SequenceId;
            DisplayName = info.DisplayName;
            FileName = info.FileName;
        }

        public int SequenceId { get; }
        public string DisplayName { get; }
        public string FileName { get; }
        public string IdLabel => SequenceId.ToString("D2");
    }
}
