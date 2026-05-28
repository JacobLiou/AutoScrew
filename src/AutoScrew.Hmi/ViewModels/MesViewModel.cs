using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AutoScrew.Hmi.ViewModels;

public sealed partial class MesViewModel : ObservableObject
{
    private readonly ISnackbarService _snackbarService;

    [ObservableProperty]
    private bool _isEnabled;

    [ObservableProperty]
    private string _baseUrl = string.Empty;

    public MesViewModel(ISnackbarService snackbarService, IConfiguration configuration)
    {
        _snackbarService = snackbarService;
        IsEnabled = bool.TryParse(configuration["Mes:Enabled"], out var enabled) && enabled;
        BaseUrl = configuration["Mes:BaseUrl"] ?? string.Empty;
    }

    [RelayCommand]
    private void TestConnection()
    {
        _snackbarService.Show(
            "MES",
            "当前版本仅提供页面与配置入口，MES 连接将在对接协议确定后接入。",
            ControlAppearance.Info,
            new SymbolIcon(SymbolRegular.Info24),
            TimeSpan.FromSeconds(4)
        );
    }
}

