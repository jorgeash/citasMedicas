using Addresses.Core.Interfaces.Repositories;
using Addresses.Persistence.Context;
using Addresses.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Addresses.Persistence.Extensions;

public static class PersistenceExtensions
{
    /// <summary>
    /// Registra el DbContext contra la base ClinicaDB y los repositorios del microservicio.
    /// La cadena de conexion vive en appsettings.json, igual que en los demas servicios.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AddressesDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<AddressesDbContext>());

        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();

        return services;
    }
}