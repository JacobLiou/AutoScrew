using System.IO;
using System.Windows;
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

public sealed partial class LanFileMaintenanceViewModel : ObservableObject
{
    private readonly ILanPrivilegedFileService _lanFiles;
    private readonly ISnackbarService _snackbar;
    private readonly IUserAuditService _audit;
    private readonly IOptions<AutoScrewAppOptions> _appOptions;
    private readonly ICurrentUser _user;

    public LanFileMaintenanceViewModel(
        ILanPrivilegedFileService lanFiles,
        ISnackbarService snackbar,
        IUserAuditService audit,
        IOptions<AutoScrewAppOptions> appOptions,
        ICurrentUser user)
    {
        _lanFiles = lanFiles;
        _snackbar = snackbar;
        _audit = audit;
        _appOptions = appOptions;
        _user = user;
        ServiceAccountDisplay = _lanFiles.ServiceAccountUserName;
        RefreshLanRoot();
    }

    public string ServiceAccountDisplay { get; }

    [ObservableProperty]
    private string _lanRootDisplay = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SyncDirectoriesCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenExplorerCommand))]
    [NotifyCanExecuteChangedFor(nameof(LockSessionCommand))]
    private bool _isUnlocked;

    [ObservableProperty]
    private string _unlockStatusMessage = string.Empty;

    [ObservableProperty]
    private string _sourceDirectory = string.Empty;

    [ObservableProperty]
    private string _targetDirectory = string.Empty;

    [ObservableProperty]
    private string _explorerPath = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UnlockCommand))]
    [NotifyCanExecuteChangedFor(nameof(SyncDirectoriesCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenExplorerCommand))]
    private bool _isBusy;

    /// <summary>由页面 PasswordBox 注入，避免口令进绑定。</summary>
    public Func<string>? PasswordReader { get; set; }

    public Action? ClearPasswordField { get; set; }

    public void OnAppearing()
    {
        RefreshLanRoot();
        IsUnlocked = _lanFiles.IsUnlocked;
        if (!IsUnlocked)
            UnlockStatusMessage = Loc.Get("S.LanFile.UnlockHint");
        if (string.IsNullOrWhiteSpace(ExplorerPath))
        {
            var root = _lanFiles.ResolveLanRoot();
            if (!string.IsNullOrWhiteSpace(root))
                ExplorerPath = root;
        }
    }

    public void OnDisappearing()
    {
        _lanFiles.Lock();
        IsUnlocked = false;
        ClearPasswordField?.Invoke();
        UnlockStatusMessage = Loc.Get("S.LanFile.SessionLocked");
        AuditHelper.Log(
            _audit,
            _appOptions,
            _user,
            AuditCategory.System,
            "LanFile.Lock",
            detail: "leave-page");
    }

    [RelayCommand(CanExecute = nameof(CanUnlock))]
    private async Task UnlockAsync()
    {
        var password = PasswordReader?.Invoke() ?? string.Empty;
        IsBusy = true;
        UnlockStatusMessage = Loc.Get("S.LanFile.Unlocking");
        try
        {
            var result = await _lanFiles.TryUnlockAsync(password).ConfigureAwait(true);
            IsUnlocked = result.Success && _lanFiles.IsUnlocked;
            if (result.Success)
            {
                UnlockStatusMessage = Loc.Get("S.LanFile.Unlocked");
                StatusMessage = UnlockStatusMessage;
                if (string.IsNullOrWhiteSpace(ExplorerPath))
                {
                    var root = _lanFiles.ResolveLanRoot();
                    if (!string.IsNullOrWhiteSpace(root))
                        ExplorerPath = root;
                }
                ShowSnackbar(UnlockStatusMessage, ControlAppearance.Success);
                AuditHelper.Log(
                    _audit,
                    _appOptions,
                    _user,
                    AuditCategory.System,
                    "LanFile.Unlock",
                    target: LanRootDisplay,
                    success: true);
            }
            else
            {
                UnlockStatusMessage = result.ErrorMessage ?? Loc.Get("S.LanFile.UnlockFailed");
                ShowSnackbar(UnlockStatusMessage, ControlAppearance.Danger);
                AuditHelper.Log(
                    _audit,
                    _appOptions,
                    _user,
                    AuditCategory.System,
                    "LanFile.Unlock",
                    target: LanRootDisplay,
                    detail: result.ErrorMessage,
                    success: false);
            }
        }
        finally
        {
            ClearPasswordField?.Invoke();
            IsBusy = false;
        }
    }

    private bool CanUnlock() => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanUsePrivilegedOps))]
    private void LockSession()
    {
        _lanFiles.Lock();
        IsUnlocked = false;
        ClearPasswordField?.Invoke();
        UnlockStatusMessage = Loc.Get("S.LanFile.SessionLocked");
        StatusMessage = UnlockStatusMessage;
        ShowSnackbar(StatusMessage, ControlAppearance.Info);
        AuditHelper.Log(
            _audit,
            _appOptions,
            _user,
            AuditCategory.System,
            "LanFile.Lock",
            detail: "manual");
    }

    [RelayCommand]
    private void BrowseSource()
    {
        var path = PickFolder(SourceDirectory);
        if (path is not null)
            SourceDirectory = path;
    }

    [RelayCommand]
    private void BrowseTarget()
    {
        var path = PickFolder(TargetDirectory);
        if (path is not null)
            TargetDirectory = path;
    }

    [RelayCommand]
    private void BrowseExplorerPath()
    {
        var path = PickFolder(ExplorerPath);
        if (path is not null)
            ExplorerPath = path;
    }

    [RelayCommand(CanExecute = nameof(CanUsePrivilegedOps))]
    private async Task SyncDirectoriesAsync()
    {
        if (string.IsNullOrWhiteSpace(SourceDirectory) || string.IsNullOrWhiteSpace(TargetDirectory))
        {
            StatusMessage = Loc.Get("S.LanFile.NeedBothPaths");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

        var confirm = System.Windows.MessageBox.Show(
            Loc.Get("S.LanFile.SyncConfirmBody"),
            Loc.Get("S.LanFile.SyncConfirmTitle"),
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);
        if (confirm != System.Windows.MessageBoxResult.Yes)
            return;

        IsBusy = true;
        StatusMessage = Loc.Get("S.LanFile.Syncing");
        try
        {
            var result = await _lanFiles
                .MirrorDirectoryAsync(SourceDirectory.Trim(), TargetDirectory.Trim())
                .ConfigureAwait(true);

            if (result.Success)
            {
                StatusMessage = Loc.Format(
                    "S.LanFile.SyncOk",
                    result.FilesCopied,
                    result.FilesOverwritten,
                    result.DirectoriesCreated);
                ShowSnackbar(StatusMessage, ControlAppearance.Success);
            }
            else
            {
                var firstErr = result.Errors.Count > 0 ? result.Errors[0] : Loc.Get("S.LanFile.SyncFailed");
                StatusMessage = Loc.Format(
                    "S.LanFile.SyncPartial",
                    result.FilesCopied,
                    result.FilesOverwritten,
                    result.Errors.Count,
                    firstErr);
                ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            }

            AuditHelper.Log(
                _audit,
                _appOptions,
                _user,
                AuditCategory.System,
                "LanFile.Sync",
                target: $"{SourceDirectory} -> {TargetDirectory}",
                detail: StatusMessage,
                success: result.Success);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            ShowSnackbar(StatusMessage, ControlAppearance.Danger);
            AuditHelper.Log(
                _audit,
                _appOptions,
                _user,
                AuditCategory.System,
                "LanFile.Sync",
                target: $"{SourceDirectory} -> {TargetDirectory}",
                detail: ex.Message,
                success: false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanUsePrivilegedOps))]
    private void OpenExplorer()
    {
        var path = string.IsNullOrWhiteSpace(ExplorerPath) ? LanRootDisplay : ExplorerPath.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            StatusMessage = Loc.Get("S.LanFile.NeedExplorerPath");
            ShowSnackbar(StatusMessage, ControlAppearance.Caution);
            return;
        }

        var err = _lanFiles.OpenInExplorer(path);
        if (err is null)
        {
            StatusMessage = Loc.Format("S.LanFile.ExplorerOpened", path);
            ShowSnackbar(StatusMessage, ControlAppearance.Success);
            AuditHelper.Log(
                _audit,
                _appOptions,
                _user,
                AuditCategory.System,
                "LanFile.OpenExplorer",
                target: path,
                success: true);
        }
        else
        {
            StatusMessage = err;
            ShowSnackbar(StatusMessage, ControlAppearance.Danger);
            AuditHelper.Log(
                _audit,
                _appOptions,
                _user,
                AuditCategory.System,
                "LanFile.OpenExplorer",
                target: path,
                detail: err,
                success: false);
        }
    }

    private bool CanUsePrivilegedOps() => IsUnlocked && !IsBusy;

    private void RefreshLanRoot()
    {
        var root = _lanFiles.ResolveLanRoot();
        LanRootDisplay = string.IsNullOrWhiteSpace(root)
            ? Loc.Get("S.LanFile.LanRootEmpty")
            : root;
    }

    private static string? PickFolder(string? initial)
    {
        var dlg = new OpenFolderDialog();
        if (!string.IsNullOrWhiteSpace(initial) && Directory.Exists(initial))
            dlg.InitialDirectory = initial;
        return dlg.ShowDialog() == true ? dlg.FolderName : null;
    }

    private void ShowSnackbar(string message, ControlAppearance appearance) =>
        _snackbar.Show(
            Loc.Get("S.LanFile.Title"),
            message,
            appearance,
            new SymbolIcon(SymbolRegular.FolderOpen24),
            TimeSpan.FromSeconds(4));
}
