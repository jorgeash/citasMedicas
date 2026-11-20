using Lab.Core.Interfaces.Repositories;
using Lab.Persistence.Context;
using Lab.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LabDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<LabDbContext>());

        services.AddScoped<IEstudioRepository, EstudioRepository>();
        services.AddScoped<ISolicitudEstudioRepository, SolicitudEstudioRepository>();
        services.AddScoped<ISolicitudEstudioDetalleRepository, SolicitudEstudioDetalleRepository>();
        services.AddScoped<IResultadoEstudioRepository, ResultadoEstudioRepository>();

        return services;
    }
}
