using AutoScrew.Hmi.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class LogsViewModel : ObservableObject
{
    private const int PreviewWindowBytes = 256 * 1024;
    private const int MaxPreviewChars = 12000;
    public sealed class LogFileListItem
    {
        public required string FullPath { get; init; }

        public required string FileName { get; init; }
    }

    private const int PageSize = 20;

    private readonly ISnackbarService _snackbarService;
    private List<string> _allLogFiles = [];

    [ObservableProperty]
    private ObservableCollection<LogFileListItem> _logFiles = [];

    [ObservableProperty]
    private LogFileListItem? _selectedLogFile;

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

    partial void OnSelectedLogFileChanged(LogFileListItem? value) => UpdatePreview();

    partial void OnCurrentPageChanged(int value) => UpdatePagingState();

    partial void OnTotalPagesChanged(int value) => UpdatePagingState();

    private void SetCurrentPage(int page, bool preserveSelection)
    {
        CurrentPage = page;

        var previousSelection = preserveSelection ? SelectedLogFile?.FullPath : null;
        var pageFiles = _allLogFiles.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        LogFiles = new ObservableCollection<LogFileListItem>(
            pageFiles.Select(path => new LogFileListItem
            {
                FullPath = path,
                FileName = Path.GetFileName(path)
            }));

        if (pageFiles.Count == 0)
        {
            SelectedLogFile = null;
            SelectedLogPreview = string.Empty;
            UpdatePagingState();
            return;
        }

        if (!string.IsNullOrWhiteSpace(previousSelection))
        {
            SelectedLogFile = LogFiles.FirstOrDefault(item => string.Equals(item.FullPath, previousSelection, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedLogFile is null)
        {
            SelectedLogFile = LogFiles[0];
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
        var selectedPath = SelectedLogFile?.FullPath;
        if (string.IsNullOrWhiteSpace(selectedPath) || !File.Exists(selectedPath))
        {
            SelectedLogPreview = string.Empty;
            return;
        }

        try
        {
            using var fs = new FileStream(selectedPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Large files are previewed via a fixed-size tail window to avoid loading full content into memory.
            var offset = Math.Max(0, fs.Length - PreviewWindowBytes);
            fs.Seek(offset, SeekOrigin.Begin);

            using var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var text = sr.ReadToEnd();
            if (text.Length > MaxPreviewChars)
                text = text[^MaxPreviewChars..];

            SelectedLogPreview = offset > 0
                ? "... (preview truncated, showing file tail)" + Environment.NewLine + text
                : text;
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
        var selectedPath = SelectedLogFile?.FullPath;
        if (string.IsNullOrWhiteSpace(selectedPath) || !File.Exists(selectedPath))
            return;

        try
        {
            Process.Start(new ProcessStartInfo(selectedPath) { UseShellExecute = true });
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

