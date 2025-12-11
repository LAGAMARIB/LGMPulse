using LGMPulse.AppServices.Interfaces;
using LGMPulse.AppServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LGMPulse.AppServices;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IGrupoService, GrupoService>();

        return services;
    }
}
