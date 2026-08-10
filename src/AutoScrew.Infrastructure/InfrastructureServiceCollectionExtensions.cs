using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Activity;
using AutoScrew.Infrastructure.Audit;
using AutoScrew.Infrastructure.Authentication;
using AutoScrew.Infrastructure.Background;
using AutoScrew.Infrastructure.Files;
using AutoScrew.Infrastructure.Hardware;
using AutoScrew.Infrastructure.Host;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.Mes;
using AutoScrew.Infrastructure.Persistence;
using AutoScrew.Infrastructure.ProcessLibrary;
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
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.SectionName));
        services.AddSingleton<IPostConfigureOptions<MimsAuthenticationOptions>, MimsAuthenticationOptionsPostConfigure>();
        services.AddSingleton<MimsMySqlAuthenticationService>();
        services.AddSingleton<IMimsAuthenticationService>(sp => sp.GetRequiredService<MimsMySqlAuthenticationService>());
        services.AddSingleton<ConfigurationAccountsAuthenticationService>();
        services.AddSingleton<FallbackMimsAuthenticationService>();

        services.AddSingleton<SessionCurrentUser>();
        services.AddSingleton<ICurrentUser>(sp => sp.GetRequiredService<SessionCurrentUser>());
        services.AddSingleton<ITemplateLayoutLoader, TemplateLayoutJsonLoader>();
        services.AddSingleton<IHostIdentity, CachedHostIdentity>();
        services.AddSingleton<ISnWorkArchiveSync, SnWorkArchiveSync>();
        services.AddSingleton<LanShareAccess>();
        services.AddSingleton<ProcessLibraryStore>();
        services.AddSingleton<IProcessLibraryService, ProcessLibraryService>();
        services.AddSingleton<IStationProcessStateStore, JsonStationProcessStateStore>();
        services.AddSingleton<IProcessChangeoverService, ProcessChangeoverService>();
        services.AddSingleton<ICurveArchive, LocalCurveArchive>();
        services.AddSingleton<ILockSessionRepository, EfLockSessionRepository>();
        services.AddSingleton<ILockHistoryQuery, EfLockHistoryQuery>();
        services.AddSingleton<IOutboundMesQueue, EfOutboundMesQueue>();
        services.AddSingleton<JsonlUserAuditStore>();
        services.AddSingleton<IOperationActivityLogService, OperationActivityLogService>();
        services.AddSingleton<UserAuditService>();
        services.AddSingleton<IUserAuditService>(sp => sp.GetRequiredService<UserAuditService>());
        services.AddHostedService<UserAuditBackgroundService>();

        var appOpts = configuration.GetSection(AutoScrewAppOptions.SectionName).Get<AutoScrewAppOptions>() ?? new AutoScrewAppOptions();
        services.Configure<SimulationOptions>(configuration.GetSection(SimulationOptions.SectionName));
        services.AddStationDeviceServices(configuration);
        services.AddSingleton<LocalJsonControllerParameterPresetStore>();
        services.AddSingleton<IControllerParameterPresetService, ControllerParameterPresetService>();
        services.AddSingleton<LocalJsonControllerSequencePresetStore>();
        services.AddSingleton<IControllerSequencePresetService, ControllerSequencePresetService>();
        services.AddSingleton<LocalJsonControllerSourceConfigStore>();
        services.AddSingleton<IControllerSourceConfigService, ControllerSourceConfigService>();
        services.AddSingleton<IControllerTraceService, IemdSdControllerTraceService>();

        services.AddSingleton<SimulatedLockStationHardware>();
        if (!appOpts.UseSimulatedHardware)
            services.AddSingleton<ILockStationHardware, IemdSdLockStationHardware>();
        else
            services.AddSingleton<ILockStationHardware>(sp => sp.GetRequiredService<SimulatedLockStationHardware>());

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

        services.AddHttpClient("mes")
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt)));

        // ProductKey 使用自建 SocketsHttpHandler（证书策略随 mes-settings），不走此命名客户端。
        services.AddHttpClient("mes-product-key");

        services.AddSingleton<LocalJsonRecipeStore>();
        services.AddSingleton<LocalRecipeMesClient>();
        services.AddSingleton<LocalJsonMesSettingsStore>();
        services.AddSingleton<IMesSettingsService, MesSettingsService>();
        services.AddSingleton<ConfigurableMesClient>();
        services.AddSingleton<IMesClient>(sp => sp.GetRequiredService<ConfigurableMesClient>());

        services.AddSingleton<ProductTemplateLocalStore>();
        services.AddSingleton<IProductTemplateLocalStore>(sp => sp.GetRequiredService<ProductTemplateLocalStore>());
        services.AddSingleton<IProductTemplateSyncRepository, EfProductTemplateSyncRepository>();
        services.AddSingleton<IMesTemplatePackageClient, MesTemplatePackageClient>();
        services.AddSingleton<IMesTemplateUploadService, MesTemplateUploadService>();
        services.AddSingleton<IMesTemplateCatalogClient, MesTemplateCatalogClient>();

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
        var sp = scope.ServiceProvider;
        var factory = sp.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var db = factory.CreateDbContext();
        db.Database.Migrate();

        sp.GetRequiredService<ProductTemplateLocalStore>().SeedFromSamplesIfEmpty();
    }
}
