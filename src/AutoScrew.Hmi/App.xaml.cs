using System.IO;
using System.Windows;
using System.Windows.Threading;
using AutoScrew.Application;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.ViewModels;
using AutoScrew.Hmi.Views.Pages;
using AutoScrew.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.DependencyInjection;

namespace AutoScrew.Hmi;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            LogUnhandled(args.Exception, "UnobservedTask");
            args.SetObserved();
        };
    }

    /// <summary>显式 <c>MimsMySql</c>，或已配置 MIMS 连接（明文 / DPAPI）时使用 MySQL 认证。</summary>
    private static bool UseMimsMySqlAuthentication(IConfiguration configuration)
    {
        var mode = configuration["Authentication:Mode"] ?? "Development";
        if (string.Equals(mode, "MimsMySql", StringComparison.OrdinalIgnoreCase))
            return true;

        var mims = configuration.GetSection("Authentication:Mims");
        if (!string.IsNullOrWhiteSpace(mims["ConnectionString"]))
            return true;
        if (!string.IsNullOrWhiteSpace(mims["ConnectionStringDpapiBase64"]))
            return true;

        return false;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            await OnStartupCoreAsync(e).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            LogUnhandled(ex, "Startup");
            MessageBox.Show(
                ex.ToString(),
                "AutoScrew 启动失败",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
        }
    }

    private async Task OnStartupCoreAsync(StartupEventArgs e)
    {
        base.OnStartup(e);
        // 登录窗是唯一可见窗体时，若保持默认 OnLastWindowClose，关闭登录后会在 Show 主窗体前触发 Shutdown。
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        ApplicationThemeManager.Apply(ApplicationTheme.Light);

        var builder = Host.CreateApplicationBuilder(e.Args);

        builder.Services.AddSerilog((_, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));

        builder.Services.Configure<AutoScrewAppOptions>(builder.Configuration.GetSection(AutoScrewAppOptions.SectionName));
        builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(AuthenticationOptions.SectionName));
        builder.Services.AddAutoScrewApplication();
        builder.Services.AddAutoScrewInfrastructure(builder.Configuration);
        builder.Services.AddSingleton<AppAuthenticationService>();
        builder.Services.AddSingleton<IUserAuthenticationService>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            if (!UseMimsMySqlAuthentication(cfg))
                return sp.GetRequiredService<AppAuthenticationService>();

            if (cfg.GetValue<bool>("Authentication:FallbackToMockAccountsOnMimsFailure"))
                return sp.GetRequiredService<FallbackMimsAuthenticationService>();

            return sp.GetRequiredService<MimsMySqlAuthenticationService>();
        });
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginWindow>();
        builder.Services.AddNavigationViewPageProvider();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISnackbarService, SnackbarService>();
        builder.Services.AddSingleton<IContentDialogService, ContentDialogService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<SurfaceBoardEditorViewModel>();
        builder.Services.AddSingleton<ProductTemplateEditorViewModel>();
        builder.Services.AddSingleton<TemplateBoardViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<MesViewModel>();
        builder.Services.AddSingleton<LogsViewModel>();
        builder.Services.AddSingleton<DeviceConnectionViewModel>();
        builder.Services.AddSingleton<ControllerParameterViewModel>();
        builder.Services.AddSingleton<ControllerSequenceViewModel>();
        builder.Services.AddSingleton<ControllerSourceViewModel>();
        builder.Services.AddSingleton<ControllerDeploymentViewModel>();
        builder.Services.AddSingleton<ControllerWorkbenchViewModel>();
        builder.Services.AddSingleton<OperationNavPage>();
        builder.Services.AddSingleton<TemplateNavPage>();
        builder.Services.AddSingleton<SettingsPage>();
        builder.Services.AddSingleton<MesPage>();
        builder.Services.AddSingleton<LogsPage>();
        builder.Services.AddSingleton<ControllerWorkbenchPage>();
        builder.Services.AddSingleton<DeviceConnectionPage>();
        builder.Services.AddSingleton<MainShellViewModel>();
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<IAppSessionCoordinator, AppSessionCoordinator>();
        builder.Services.AddSingleton<LocalizationService>();

        _host = builder.Build();

        _host.Services.InitializeAutoScrewDatabase();
        await _host.StartAsync().ConfigureAwait(true);

        var localization = _host.Services.GetRequiredService<LocalizationService>();
        var appOptions = _host.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<AutoScrewAppOptions>>().Value;
        localization.Initialize(appOptions.UiCulture);
        Loc.Initialize(localization);

        AuditContext.Initialize(
            _host.Services.GetRequiredService<IUserAuditService>(),
            _host.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<AutoScrewAppOptions>>(),
            _host.Services.GetRequiredService<ICurrentUser>());

        var login = _host.Services.GetRequiredService<LoginWindow>();
        var loginOk = login.ShowDialog() == true;
        if (!loginOk)
        {
            Shutdown();
            return;
        }

        var main = _host.Services.GetRequiredService<MainWindow>();
        MainWindow = main;
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        main.Show();
    }

    /// <summary>同步关闭 host；避免 async OnExit + ConfigureAwait(false) 在线程池上调用 <c>base.OnExit</c> 触发跨线程异常。</summary>
    protected override void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
            _host.StopAsync().GetAwaiter().GetResult();

        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        LogUnhandled(e.Exception, "Dispatcher");
        MessageBox.Show(
            e.Exception.Message,
            "AutoScrew",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }

    private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            LogUnhandled(ex, "AppDomain");
    }

    private static void LogUnhandled(Exception ex, string source)
    {
        try
        {
            Log.Error(ex, "Unhandled exception ({Source})", source);
        }
        catch
        {
            // Serilog may not be configured yet.
        }

        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "crash.log");
            File.AppendAllText(path, $"{DateTimeOffset.Now:O} [{source}] {ex}{Environment.NewLine}");
        }
        catch
        {
            // ignore file logging failures
        }
    }
}

