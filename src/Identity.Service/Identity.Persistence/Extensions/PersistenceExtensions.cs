using Identity.Core.Interfaces.Repositories;
using Identity.Persistence.Context;
using Identity.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Persistence.Extensions;

public static class PersistenceExtensions
{
    /// <summary>
    /// Registra el contexto apuntando a ClinicaDB. Las tablas ya existen,
    /// por lo que este servicio no aplica migraciones.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPermisoRepository, PermisoRepository>();
        services.AddScoped<ISesionRepository, SesionRepository>();

        return services;
    }
}
