using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AutoScrew.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoScrewApplication(this IServiceCollection services)
    {
        services.AddOptions<AutoScrewAppOptions>().BindConfiguration(AutoScrewAppOptions.SectionName);
        services.AddSingleton<IRecipeProvisioningService, RecipeProvisioningService>();
        services.AddSingleton<OperatorSessionController>();
        return services;
    }
}
