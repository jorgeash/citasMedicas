using Business.Core.Interfaces.Repositories;
using Business.Persistence.Context;
using Business.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Persistence.Extensions;

public static class PersistenceExtensions
{
    /// <summary>
    /// Registra el DbContext contra la base ClinicaDB y los repositorios del microservicio.
    /// La cadena de conexion vive en appsettings.json, igual que en los demas servicios.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BusinessDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BusinessDbContext>());

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICustomerTypeRepository, CustomerTypeRepository>();
        services.AddScoped<ICustomerContactRepository, CustomerContactRepository>();

        return services;
    }
}