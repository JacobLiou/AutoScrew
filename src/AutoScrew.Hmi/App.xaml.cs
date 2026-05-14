using System.Windows;
using AutoScrew.Application;
using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using AutoScrew.Hmi.ViewModels;
using AutoScrew.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AutoScrew.Hmi;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var builder = Host.CreateApplicationBuilder(e.Args);

        builder.Services.AddSerilog((_, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));

        builder.Services.Configure<AutoScrewAppOptions>(builder.Configuration.GetSection(AutoScrewAppOptions.SectionName));
        builder.Services.AddAutoScrewApplication();
        builder.Services.AddAutoScrewInfrastructure();
        builder.Services.AddSingleton<IUserAuthenticationService, AppAuthenticationService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginWindow>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainWindow>();

        _host = builder.Build();

        _host.Services.InitializeAutoScrewDatabase();
        await _host.StartAsync().ConfigureAwait(true);

        var login = _host.Services.GetRequiredService<LoginWindow>();
        var loginOk = login.ShowDialog() == true;
        if (!loginOk)
        {
            Shutdown();
            return;
        }

        _host.Services.GetRequiredService<MainWindow>().Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
            await _host.StopAsync().ConfigureAwait(false);

        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
