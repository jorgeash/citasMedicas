using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Persistence.Context;
using Scheduling.Persistence.Repositories;

namespace Scheduling.Persistence.Extensions;

public static class PersistenceExtensions
{
    /// <summary>
    /// Registra el contexto apuntando a ClinicaDB. Las tablas ya existen,
    /// por lo que este servicio no aplica migraciones.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SchedulingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<SchedulingDbContext>());

        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<ICitaRepository, CitaRepository>();
        services.AddScoped<IAseguradoraRepository, AseguradoraRepository>();
        services.AddScoped<IMedicoRepository, MedicoRepository>();
        services.AddScoped<IEstadoCitaRepository, EstadoCitaRepository>();

        return services;
    }
}
