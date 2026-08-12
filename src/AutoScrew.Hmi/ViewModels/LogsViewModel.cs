using AutoScrew.Hmi.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class LogsViewModel : ObservableObject
{
    private const int PageSize = 20;

    private readonly ISnackbarService _snackbarService;
    private List<string> _allLogFiles = [];

    [ObservableProperty]
    private ObservableCollection<string> _logFiles = [];

    [ObservableProperty]
    private string? _selectedLogFile;

    [ObservableProperty]
    private string _selectedLogPreview = string.Empty;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private string _pageInfo = string.Empty;

    [ObservableProperty]
    private bool _hasPreviousPage;

    [ObservableProperty]
    private bool _hasNextPage;

    public LogsViewModel(ISnackbarService snackbarService)
    {
        _snackbarService = snackbarService;
        Refresh();
    }

    [RelayCommand]
    private void Refresh()
    {
        var dir = GetLogsDirectory();
        Directory.CreateDirectory(dir);

        _allLogFiles = Directory.EnumerateFiles(dir, "*.log", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(dir, "*.txt", SearchOption.TopDirectoryOnly))
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .ToList();

        var targetPage = CurrentPage;
        if (_allLogFiles.Count == 0)
        {
            TotalPages = 1;
            SetCurrentPage(1, preserveSelection: false);
            return;
        }

        TotalPages = Math.Max(1, (int)Math.Ceiling(_allLogFiles.Count / (double)PageSize));
        targetPage = Math.Clamp(targetPage, 1, TotalPages);
        SetCurrentPage(targetPage, preserveSelection: true);
    }

    partial void OnSelectedLogFileChanged(string? value) => UpdatePreview();

    partial void OnCurrentPageChanged(int value) => UpdatePagingState();

    partial void OnTotalPagesChanged(int value) => UpdatePagingState();

    private void SetCurrentPage(int page, bool preserveSelection)
    {
        CurrentPage = page;

        var previousSelection = preserveSelection ? SelectedLogFile : null;
        var pageFiles = _allLogFiles.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        LogFiles = new ObservableCollection<string>(pageFiles);

        if (pageFiles.Count == 0)
        {
            SelectedLogFile = null;
            SelectedLogPreview = string.Empty;
            UpdatePagingState();
            return;
        }

        if (!string.IsNullOrWhiteSpace(previousSelection) && pageFiles.Contains(previousSelection))
        {
            SelectedLogFile = previousSelection;
        }
        else
        {
            SelectedLogFile = pageFiles[0];
        }

        UpdatePagingState();
    }

    private void UpdatePagingState()
    {
        HasPreviousPage = CurrentPage > 1;
        HasNextPage = CurrentPage < TotalPages;
        PageInfo = TotalPages <= 1
            ? Loc.Format("S.Logs.PageInfoSingle", _allLogFiles.Count)
            : Loc.Format("S.Logs.PageInfo", CurrentPage, TotalPages, _allLogFiles.Count);
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (!HasPreviousPage)
            return;

        SetCurrentPage(CurrentPage - 1, preserveSelection: true);
    }

    [RelayCommand]
    private void NextPage()
    {
        if (!HasNextPage)
            return;

        SetCurrentPage(CurrentPage + 1, preserveSelection: true);
    }

    [RelayCommand]
    private void FirstPage()
    {
        if (CurrentPage <= 1)
            return;

        SetCurrentPage(1, preserveSelection: true);
    }

    [RelayCommand]
    private void LastPage()
    {
        if (CurrentPage >= TotalPages)
            return;

        SetCurrentPage(TotalPages, preserveSelection: true);
    }

    private void UpdatePreview()
    {
        if (string.IsNullOrWhiteSpace(SelectedLogFile) || !File.Exists(SelectedLogFile))
        {
            SelectedLogPreview = string.Empty;
            return;
        }

        try
        {
            using var fs = new FileStream(SelectedLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            var text = sr.ReadToEnd();
            SelectedLogPreview = text.Length <= 8000 ? text : text[..8000];
        }
        catch
        {
            SelectedLogPreview = string.Empty;
        }
    }

    [RelayCommand]
    private void OpenLogsFolder()
    {
        try
        {
            Process.Start("explorer.exe", GetLogsDirectory());
        }
        catch
        {
            _snackbarService.Show(
                Loc.Get("S.Logs.SnackbarTitle"),
                Loc.Get("S.Logs.OpenFolderFailed"),
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24),
                TimeSpan.FromSeconds(4)
            );
        }
    }

    [RelayCommand]
    private void OpenSelected()
    {
        if (string.IsNullOrWhiteSpace(SelectedLogFile) || !File.Exists(SelectedLogFile))
            return;

        try
        {
            Process.Start(new ProcessStartInfo(SelectedLogFile) { UseShellExecute = true });
        }
        catch
        {
            _snackbarService.Show(
                Loc.Get("S.Logs.SnackbarTitle"),
                Loc.Get("S.Logs.OpenFileFailed"),
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24),
                TimeSpan.FromSeconds(4)
            );
        }
    }

    private static string GetLogsDirectory() => Path.Combine(AppContext.BaseDirectory, "Logs");
}

