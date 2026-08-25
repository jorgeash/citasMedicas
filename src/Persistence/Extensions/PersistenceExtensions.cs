using Domain.Billing.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Billing.Context;
using Persistence.Billing.Repositories;

namespace Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<IServicioRepository, ServicioRepository>();
        services.AddScoped<ITarifaRepository, TarifaRepository>();
        services.AddScoped<IPacienteRepository, PacienteRepository>();

        return services;
    }
}