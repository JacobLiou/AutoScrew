using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Hardware;

public static class IemdSdServiceCollectionExtensions
{
    public static IServiceCollection AddStationDeviceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IemdSdOptions>(configuration.GetSection(IemdSdOptions.SectionName));
        services.AddSingleton<LocalJsonStationDeviceStore>();
        services.AddSingleton<IemdSdClientFactory>();
        services.AddSingleton<StationDeviceManager>();
        services.AddSingleton<IStationDeviceService>(sp => sp.GetRequiredService<StationDeviceManager>());
        return services;
    }
}
