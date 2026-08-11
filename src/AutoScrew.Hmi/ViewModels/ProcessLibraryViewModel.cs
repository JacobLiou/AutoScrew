using System.Collections.ObjectModel;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Dialog;
using AutoScrew.Hmi.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class ProcessLibraryViewModel : ObservableObject
{
    private readonly IProcessLibraryService _library;
    private readonly ISnackbarService _snackbar;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;

    public ObservableCollection<string> ProductPns { get; } = [];
    public ObservableCollection<ProcessLibrarySlotRow> Slots { get; } = [];
    public ObservableCollection<ProcessLibrarySequenceRow> Sequences { get; } = [];

    [ObservableProperty]
    private string _productPn = string.Empty;

    [ObservableProperty]
    private string _processRootPath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _deviceStatusText = string.Empty;

    [ObservableProperty]
    private ProcessLibrarySlotRow? _selectedSlot;

    [ObservableProperty]
    private ProcessLibrarySequenceRow? _selectedSequence;

    [ObservableProperty]
    private bool _isBusy;

    /// <summary>上传顺序 Excel 时使用的顺序 ID（默认 1）。</summary>
    [ObservableProperty]
    private string _uploadSequenceIdText = "1";

    public ProcessLibraryViewModel(
        IProcessLibraryService library,
        ISnackbarService snackbar,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _library = library;
        _snackbar = snackbar;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
    }

    public bool HasProductPn => !string.IsNullOrWhiteSpace(ProductPn);

    public async Task OnAppearingAsync()
    {
        ProcessRootPath = _library.ProcessRootPath;
        DeviceStatusText = _library.IsDeviceAvailable
            ? Loc.Get("S.ProcessLibrary.DeviceOnline")
            : Loc.Get("S.ProcessLibrary.DeviceOffline");
        await RefreshProductListAsync().ConfigureAwait(true);
        // ComboBox 刷新 ItemsSource 后可能清空 Text；始终同步下方列表，避免残留上一 PN。
        await RefreshProductContentAsync().ConfigureAwait(true);
    }

    partial void OnProductPnChanged(string value)
    {
        OnPropertyChanged(nameof(HasProductPn));
        if (string.IsNullOrWhiteSpace(value))
            ClearProductContent(Loc.Get("S.ProcessLibrary.SelectProductHint"));
    }

    private void ClearProductContent(string? status = null)
    {
        Slots.Clear();
        Sequences.Clear();
        SelectedSlot = null;
        SelectedSequence = null;
        if (status is not null)
            StatusMessage = status;
    }

    [RelayCommand]
    private async Task RefreshProductListAsync()
    {
        try
        {
            IsBusy = true;
            var previous = ProductPn?.Trim() ?? string.Empty;
            var list = await _library.ListProductPnsAsync().ConfigureAwait(true);
            ProductPns.Clear();
            foreach (var pn in list)
                ProductPns.Add(pn);
            ProcessRootPath = _library.ProcessRootPath;

            // 可编辑 ComboBox 清空 ItemsSource 后 Text 常被置空；尽量恢复仍存在的上一选择。
            if (!string.IsNullOrEmpty(previous)
                && ProductPns.Any(p => string.Equals(p, previous, StringComparison.OrdinalIgnoreCase)))
            {
                ProductPn = previous;
            }
            else if (string.IsNullOrWhiteSpace(ProductPn))
            {
                ClearProductContent(Loc.Get("S.ProcessLibrary.SelectProductHint"));
            }

            StatusMessage = Loc.Get("S.ProcessLibrary.StatusRefreshed");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task RefreshSlotsAsync() => RefreshProductContentAsync();

    [RelayCommand]
    private async Task RefreshProductContentAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn))
        {
            ClearProductContent(Loc.Get("S.ProcessLibrary.SelectProductHint"));
            return;
        }

        try
        {
            IsBusy = true;
            var product = await _library.GetProductAsync(ProductPn.Trim()).ConfigureAwait(true);
            Slots.Clear();
            Sequences.Clear();
            SelectedSlot = null;
            SelectedSequence = null;
            if (product is null)
            {
                StatusMessage = Loc.Get("S.ProcessLibrary.StatusProductEmpty");
                return;
            }

            foreach (var s in product.Slots)
                Slots.Add(new ProcessLibrarySlotRow(s));
            foreach (var s in product.Sequences)
                Sequences.Add(new ProcessLibrarySequenceRow(s));
            StatusMessage = string.Format(
                Loc.Get("S.ProcessLibrary.StatusProductLoaded"),
                product.Slots.Count,
                product.Sequences.Count);
        }
        catch (Exception ex)
        {
            ClearProductContent(ex.Message);
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UploadCardAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn))
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.NeedProductPn"), ControlAppearance.Caution);
            return;
        }

        var dialog = new OpenFileDialog
        {
            Filter = "Process card (*.txt)|*.txt|All files (*.*)|*.*",
            RestoreDirectory = true,
            Multiselect = true,
        };
        if (dialog.ShowDialog() != true)
            return;

        try
        {
            IsBusy = true;
            var pn = ProductPn.Trim();
            var count = 0;
            foreach (var file in dialog.FileNames)
            {
                Audit("Configuration.ProcessLibraryUpload", $"product={pn};file={file}");
                await _library.UploadProcessCardAsync(pn, file).ConfigureAwait(true);
                count++;
            }

            EnsureProductInList(pn);
            await RefreshProductContentAsync().ConfigureAwait(true);
            StatusMessage = string.Format(Loc.Get("S.ProcessLibrary.StatusUploaded"), count);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UploadSequenceAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn))
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.NeedProductPn"), ControlAppearance.Caution);
            return;
        }

        var dialog = new OpenFileDialog
        {
            Filter = "Sequence JSON (*.json)|*.json|All files (*.*)|*.*",
            RestoreDirectory = true,
            Multiselect = true,
        };
        if (dialog.ShowDialog() != true)
            return;

        try
        {
            IsBusy = true;
            var pn = ProductPn.Trim();
            var count = 0;
            foreach (var file in dialog.FileNames)
            {
                Audit("Configuration.ProcessLibraryUploadSequence", $"product={pn};file={file}");
                await _library.UploadSequenceAsync(pn, file).ConfigureAwait(true);
                count++;
            }

            EnsureProductInList(pn);
            await RefreshProductContentAsync().ConfigureAwait(true);
            StatusMessage = string.Format(Loc.Get("S.ProcessLibrary.StatusSequenceUploaded"), count);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UploadSequenceExcelAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn))
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.NeedProductPn"), ControlAppearance.Caution);
            return;
        }

        if (!int.TryParse(UploadSequenceIdText?.Trim(), out var sequenceId) || sequenceId is < 1 or > 500)
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.InvalidSequenceId"), ControlAppearance.Caution);
            return;
        }

        var dialog = new OpenFileDialog
        {
            Filter = "Sequence Excel (*.xlsx)|*.xlsx|All files (*.*)|*.*",
            RestoreDirectory = true,
            Multiselect = false,
        };
        if (dialog.ShowDialog() != true)
            return;

        try
        {
            IsBusy = true;
            var pn = ProductPn.Trim();
            var file = dialog.FileName;
            Audit(
                "Configuration.ProcessLibraryUploadSequenceExcel",
                $"product={pn};sequenceId={sequenceId};file={file}");
            await _library.UploadSequenceExcelAsync(pn, file, sequenceId).ConfigureAwait(true);

            EnsureProductInList(pn);
            await RefreshProductContentAsync().ConfigureAwait(true);
            StatusMessage = string.Format(
                Loc.Get("S.ProcessLibrary.StatusSequenceExcelUploaded"),
                sequenceId);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RemoveSelectedSlotAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn) || SelectedSlot is null)
            return;

        if (!ConfirmTips.ShowDialog(
                string.Format(Loc.Get("S.ProcessLibrary.ConfirmDeleteBody"), SelectedSlot.SlotId, SelectedSlot.ScrewPn),
                System.Windows.Application.Current?.MainWindow,
                Loc.Get("S.ProcessLibrary.ConfirmDeleteTitle")))
            return;

        try
        {
            IsBusy = true;
            var pn = ProductPn.Trim();
            var slotId = SelectedSlot.SlotId;
            Audit("Configuration.ProcessLibraryDelete", $"product={pn};slot={slotId}");
            await _library.RemoveSlotAsync(pn, slotId).ConfigureAwait(true);
            await RefreshProductContentAsync().ConfigureAwait(true);
            StatusMessage = Loc.Get("S.ProcessLibrary.StatusDeleted");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RemoveSelectedSequenceAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn) || SelectedSequence is null)
            return;

        if (!ConfirmTips.ShowDialog(
                string.Format(
                    Loc.Get("S.ProcessLibrary.ConfirmDeleteSequenceBody"),
                    SelectedSequence.SequenceId,
                    SelectedSequence.DisplayName),
                System.Windows.Application.Current?.MainWindow,
                Loc.Get("S.ProcessLibrary.ConfirmDeleteSequenceTitle")))
            return;

        try
        {
            IsBusy = true;
            var pn = ProductPn.Trim();
            var id = SelectedSequence.SequenceId;
            Audit("Configuration.ProcessLibraryDeleteSequence", $"product={pn};sequenceId={id}");
            await _library.RemoveSequenceAsync(pn, id).ConfigureAwait(true);
            await RefreshProductContentAsync().ConfigureAwait(true);
            StatusMessage = Loc.Get("S.ProcessLibrary.StatusSequenceDeleted");
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeployProductAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn))
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.NeedProductPn"), ControlAppearance.Caution);
            return;
        }

        if (Slots.Count == 0)
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.StatusProductEmpty"), ControlAppearance.Caution);
            return;
        }

        if (!_library.IsDeviceAvailable)
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.DeviceOffline"), ControlAppearance.Caution);
            return;
        }

        var pn = ProductPn.Trim();
        if (!ConfirmTips.ShowDialog(
                string.Format(Loc.Get("S.ProcessLibrary.ConfirmDeployBody"), pn, Slots.Count),
                System.Windows.Application.Current?.MainWindow,
                Loc.Get("S.ProcessLibrary.ConfirmDeployTitle")))
            return;

        try
        {
            IsBusy = true;
            Audit("Configuration.ProcessLibraryDeploy", $"product={pn};slots={Slots.Count}");
            var result = await _library.DeployProductToDeviceAsync(pn).ConfigureAwait(true);
            if (result.Failures.Count > 0)
            {
                var fail = result.Failures[0];
                StatusMessage = string.Format(
                    Loc.Get("S.ProcessLibrary.StatusDeployPartial"),
                    result.WrittenSlotIds.Count,
                    fail.SlotId,
                    fail.Message);
                ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            }
            else
            {
                StatusMessage = string.Format(
                    Loc.Get("S.ProcessLibrary.StatusDeployOk"),
                    result.WrittenSlotIds.Count);
                ShowSnackbar(StatusMessage, ControlAppearance.Success);
            }

            Audit(
                "Configuration.ProcessLibraryDeployResult",
                $"product={pn};written={result.WrittenSlotIds.Count};failures={result.Failures.Count}",
                success: result.Failures.Count == 0);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
            Audit("Configuration.ProcessLibraryDeployResult", ex.Message, success: false);
        }
        finally
        {
            IsBusy = false;
            RefreshDeviceStatus();
        }
    }

    [RelayCommand]
    private async Task DeploySequencesAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn))
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.NeedProductPn"), ControlAppearance.Caution);
            return;
        }

        if (Sequences.Count == 0)
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.StatusSequenceEmpty"), ControlAppearance.Caution);
            return;
        }

        if (!_library.IsDeviceAvailable)
        {
            ShowSnackbar(Loc.Get("S.ProcessLibrary.DeviceOffline"), ControlAppearance.Caution);
            return;
        }

        var pn = ProductPn.Trim();
        if (!ConfirmTips.ShowDialog(
                string.Format(Loc.Get("S.ProcessLibrary.ConfirmDeploySequenceBody"), pn, Sequences.Count),
                System.Windows.Application.Current?.MainWindow,
                Loc.Get("S.ProcessLibrary.ConfirmDeploySequenceTitle")))
            return;

        try
        {
            IsBusy = true;
            Audit("Configuration.ProcessLibraryDeploySequences", $"product={pn};count={Sequences.Count}");
            var result = await _library.DeployProductSequencesToDeviceAsync(pn).ConfigureAwait(true);
            if (result.Failures.Count > 0)
            {
                var fail = result.Failures[0];
                StatusMessage = string.Format(
                    Loc.Get("S.ProcessLibrary.StatusDeploySequencePartial"),
                    result.WrittenSequenceIds.Count,
                    fail.SequenceId,
                    fail.Message);
                ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            }
            else
            {
                StatusMessage = string.Format(
                    Loc.Get("S.ProcessLibrary.StatusDeploySequenceOk"),
                    result.WrittenSequenceIds.Count);
                ShowSnackbar(StatusMessage, ControlAppearance.Success);
            }

            Audit(
                "Configuration.ProcessLibraryDeploySequencesResult",
                $"product={pn};written={result.WrittenSequenceIds.Count};failures={result.Failures.Count}",
                success: result.Failures.Count == 0);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(ex.Message, ControlAppearance.Danger);
            Audit("Configuration.ProcessLibraryDeploySequencesResult", ex.Message, success: false);
        }
        finally
        {
            IsBusy = false;
            RefreshDeviceStatus();
        }
    }

    private void EnsureProductInList(string pn)
    {
        if (!ProductPns.Contains(pn, StringComparer.OrdinalIgnoreCase))
            ProductPns.Add(pn);
    }

    private void RefreshDeviceStatus() =>
        DeviceStatusText = _library.IsDeviceAvailable
            ? Loc.Get("S.ProcessLibrary.DeviceOnline")
            : Loc.Get("S.ProcessLibrary.DeviceOffline");

    private void Audit(string action, string? detail, bool success = true) =>
        AuditHelper.Log(_audit, _appOptions, _user, AuditCategory.Configuration, action, target: null, detail: detail, success: success);

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbar.Show(Loc.Get("S.ProcessLibrary.Title"), message, appearance, null, TimeSpan.FromSeconds(4));
}

public sealed class ProcessLibrarySlotRow
{
    public ProcessLibrarySlotRow(ProcessLibrarySlotInfo info)
    {
        SlotId = info.SlotId;
        ScrewPn = info.ScrewPn;
        FileName = info.FileName;
        DisplayName = info.DisplayName;
    }

    public int SlotId { get; }
    public string ScrewPn { get; }
    public string FileName { get; }
    public string DisplayName { get; }
    public string SlotLabel => SlotId.ToString("D2");
}

public sealed class ProcessLibrarySequenceRow
{
    public ProcessLibrarySequenceRow(ProcessLibrarySequenceInfo info)
    {
        SequenceId = info.SequenceId;
        FileName = info.FileName;
        DisplayName = info.DisplayName;
    }

    public int SequenceId { get; }
    public string FileName { get; }
    public string DisplayName { get; }
    public string IdLabel => SequenceId.ToString("D2");
}
