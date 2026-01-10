using LGMPulse.Persistence.Interfaces;
using LGMPulse.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LGMPulse.Persistence;

public static class ServiceCollectionExt
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services)
    {
        services.AddScoped<IGrupoRepository, GrupoRepository>();
        services.AddScoped<IMovtoRepository, MovtoRepository>();
        services.AddScoped<ILocalUserRepository, LocalUserRepository>();
        services.AddScoped<IAgendaRepository, AgendaRepository>();

        return services;
    }
}
