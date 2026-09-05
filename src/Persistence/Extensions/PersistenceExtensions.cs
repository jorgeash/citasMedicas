using Domain.Billing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Billing.Context;
using Domain.Interfaces;
using Persistence.Repositories;

namespace Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BillingDbContext>());

        services.AddScoped<IRepository<Servicio>, Repository<Servicio>>();
        services.AddScoped<IRepository<Tarifa>, Repository<Tarifa>>();

        return services;
    }
}
