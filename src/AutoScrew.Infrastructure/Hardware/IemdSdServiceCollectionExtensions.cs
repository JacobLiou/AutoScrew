using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UDL.Delta.IemdSd;

namespace AutoScrew.Infrastructure.Hardware;

public static class IemdSdServiceCollectionExtensions
{
    public static IServiceCollection AddIemdSdDriver(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IemdSdOptions>(configuration.GetSection(IemdSdOptions.SectionName));

        services.AddSingleton<IIemdSdClient>(sp =>
        {
            var opt = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<IemdSdOptions>>().Value;
            var clientOpt = new IemdSdClientOptions
            {
                Host = opt.Host,
                Port = opt.Port,
                ToolIndex = opt.ToolIndex,
                TriggerMode = string.Equals(opt.TriggerMode, "Manual", StringComparison.OrdinalIgnoreCase)
                    ? TighteningTriggerMode.Manual
                    : TighteningTriggerMode.AutoDi,
                AutoLockOnInit = opt.AutoLockOnInit,
                SendUnlockAfterCycle = opt.SendUnlockAfterCycle,
                UseLegacyFinishRegister = opt.UseLegacyFinishRegister,
                CommandTimeoutMs = opt.CommandTimeoutMs,
            };
            return new IemdSdClient(clientOpt, sp.GetRequiredService<ILogger<IemdSdClient>>());
        });

        services.AddSingleton<ILockStationHardware, IemdSdLockStationHardware>();
        return services;
    }
}
