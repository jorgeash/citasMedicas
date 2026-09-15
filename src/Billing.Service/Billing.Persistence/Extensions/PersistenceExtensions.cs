using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using Billing.Persistence.Context;
using Billing.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BillingDbContext>());

        services.AddScoped<IServicioRepository, ServicioRepository>();
        services.AddScoped<ITarifaRepository, TarifaRepository>();

        return services;
    }
}
