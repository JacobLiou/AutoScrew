using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Audit;
using AutoScrew.Infrastructure.Authentication;
using AutoScrew.Infrastructure.Background;
using AutoScrew.Infrastructure.Files;
using AutoScrew.Infrastructure.Hardware;
using AutoScrew.Infrastructure.Mes;
using AutoScrew.Infrastructure.Persistence;
using AutoScrew.Infrastructure.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace AutoScrew.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAutoScrewInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MimsAuthenticationOptions>(configuration.GetSection(MimsAuthenticationOptions.SectionName));
        services.AddSingleton<IPostConfigureOptions<MimsAuthenticationOptions>, MimsAuthenticationOptionsPostConfigure>();
        services.AddSingleton<MimsMySqlAuthenticationService>();

        services.AddSingleton<SessionCurrentUser>();
        services.AddSingleton<ICurrentUser>(sp => sp.GetRequiredService<SessionCurrentUser>());
        services.AddSingleton<ITemplateLayoutLoader, TemplateLayoutJsonLoader>();
        services.AddSingleton<ICurveArchive, LocalCurveArchive>();
        services.AddSingleton<ILockSessionRepository, EfLockSessionRepository>();
        services.AddSingleton<IOutboundMesQueue, EfOutboundMesQueue>();
        services.AddSingleton<JsonlUserAuditStore>();
        services.AddSingleton<UserAuditService>();
        services.AddSingleton<IUserAuditService>(sp => sp.GetRequiredService<UserAuditService>());
        services.AddHostedService<UserAuditBackgroundService>();

        var appOpts = configuration.GetSection(AutoScrewAppOptions.SectionName).Get<AutoScrewAppOptions>() ?? new AutoScrewAppOptions();
        services.AddStationDeviceServices(configuration);
        services.AddSingleton<LocalJsonControllerParameterPresetStore>();
        services.AddSingleton<IControllerParameterPresetService, ControllerParameterPresetService>();

        if (!appOpts.UseSimulatedHardware)
            services.AddSingleton<ILockStationHardware, IemdSdLockStationHardware>();
        else
            services.AddSingleton<ILockStationHardware, SimulatedLockStationHardware>();

        services.AddDbContextFactory<AppDbContext>((sp, builder) =>
        {
            var options = sp.GetRequiredService<IOptions<AutoScrewAppOptions>>().Value;
            var root = string.IsNullOrWhiteSpace(options.DataDirectory)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "data")
                : options.DataDirectory;
            Directory.CreateDirectory(root);
            var dbPath = Path.Combine(root, "autoscrew.db");
            builder.UseSqlite($"Data Source={dbPath}");
        });

        services.AddHttpClient("mes", (sp, client) =>
            {
                var baseUrl = sp.GetRequiredService<IOptions<AutoScrewAppOptions>>().Value.MesBaseUrl.Trim();
                if (!baseUrl.EndsWith('/'))
                    baseUrl += "/";
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt)));

        services.AddSingleton<IMesClient>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<AutoScrewAppOptions>>().Value;
            if (opt.UseMockMes)
                return new MockMesClient();

            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("mes");
            return new MesHttpClient(
                http,
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<MesHttpClient>>());
        });

        services.AddHostedService<OutboxMesRetryHostedService>();

        services.PostConfigure<AutoScrewAppOptions>(o =>
        {
            if (!string.IsNullOrWhiteSpace(o.TemplateDirectory) && !Path.IsPathRooted(o.TemplateDirectory))
                o.TemplateDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, o.TemplateDirectory));

            if (!string.IsNullOrWhiteSpace(o.DataDirectory) && !Path.IsPathRooted(o.DataDirectory))
                o.DataDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, o.DataDirectory));

            if (!string.IsNullOrWhiteSpace(o.OptionalNetworkArchiveRoot) && !Path.IsPathRooted(o.OptionalNetworkArchiveRoot))
                o.OptionalNetworkArchiveRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, o.OptionalNetworkArchiveRoot));

            if (!string.IsNullOrWhiteSpace(o.AuditDirectory) && !Path.IsPathRooted(o.AuditDirectory))
                o.AuditDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, o.AuditDirectory));
        });

        return services;
    }

    public static void InitializeAutoScrewDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var db = factory.CreateDbContext();
        db.Database.Migrate();
    }
}
