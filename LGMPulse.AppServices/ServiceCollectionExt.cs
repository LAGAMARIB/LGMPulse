using LGMPulse.AppServices.Interfaces;
using LGMPulse.AppServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LGMPulse.AppServices;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IGrupoService, GrupoService>();
        services.AddScoped<IMovtoService, MovtoService>();
        services.AddScoped<IAgendaService, AgendaService>();

        return services;
    }
}
