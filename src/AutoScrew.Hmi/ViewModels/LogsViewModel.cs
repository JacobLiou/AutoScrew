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
    private readonly ISnackbarService _snackbarService;

    [ObservableProperty]
    private ObservableCollection<string> _logFiles = [];

    [ObservableProperty]
    private string? _selectedLogFile;

    [ObservableProperty]
    private string _selectedLogPreview = string.Empty;

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

        var files = Directory.EnumerateFiles(dir, "*.log", SearchOption.TopDirectoryOnly)
            .Concat(Directory.EnumerateFiles(dir, "*.txt", SearchOption.TopDirectoryOnly))
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .ToList();

        LogFiles = new ObservableCollection<string>(files);
        SelectedLogFile = LogFiles.FirstOrDefault();
        UpdatePreview();
    }

    partial void OnSelectedLogFileChanged(string? value) => UpdatePreview();

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
                "日志",
                "打开日志目录失败，请检查系统权限。",
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
                "日志",
                "打开日志文件失败。",
                ControlAppearance.Caution,
                new SymbolIcon(SymbolRegular.Warning24),
                TimeSpan.FromSeconds(4)
            );
        }
    }

    private static string GetLogsDirectory() => Path.Combine(AppContext.BaseDirectory, "Logs");
}

