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
    private bool _isBusy;

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

    public async Task OnAppearingAsync()
    {
        ProcessRootPath = _library.ProcessRootPath;
        DeviceStatusText = _library.IsDeviceAvailable
            ? Loc.Get("S.ProcessLibrary.DeviceOnline")
            : Loc.Get("S.ProcessLibrary.DeviceOffline");
        await RefreshProductListAsync().ConfigureAwait(true);
        if (!string.IsNullOrWhiteSpace(ProductPn))
            await RefreshSlotsAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task RefreshProductListAsync()
    {
        try
        {
            IsBusy = true;
            var list = await _library.ListProductPnsAsync().ConfigureAwait(true);
            ProductPns.Clear();
            foreach (var pn in list)
                ProductPns.Add(pn);
            ProcessRootPath = _library.ProcessRootPath;
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
    private async Task RefreshSlotsAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductPn))
        {
            Slots.Clear();
            return;
        }

        try
        {
            IsBusy = true;
            var product = await _library.GetProductAsync(ProductPn.Trim()).ConfigureAwait(true);
            Slots.Clear();
            if (product is null)
            {
                StatusMessage = Loc.Get("S.ProcessLibrary.StatusProductEmpty");
                return;
            }

            foreach (var s in product.Slots)
                Slots.Add(new ProcessLibrarySlotRow(s));
            StatusMessage = string.Format(
                Loc.Get("S.ProcessLibrary.StatusSlotsLoaded"),
                product.Slots.Count);
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

            if (!ProductPns.Contains(pn, StringComparer.OrdinalIgnoreCase))
                ProductPns.Add(pn);

            await RefreshSlotsAsync().ConfigureAwait(true);
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
            await RefreshSlotsAsync().ConfigureAwait(true);
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
            DeviceStatusText = _library.IsDeviceAvailable
                ? Loc.Get("S.ProcessLibrary.DeviceOnline")
                : Loc.Get("S.ProcessLibrary.DeviceOffline");
        }
    }

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
