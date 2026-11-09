using System.Reflection;
using Billing.Core.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        try
        {
            assembly.GetTypes();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });
        }
        catch (ReflectionTypeLoadException ex)
        {
            Console.WriteLine("========== ERRORES DE CARGA ==========");

            foreach (var loaderException in ex.LoaderExceptions)
            {
                Console.WriteLine(loaderException?.ToString());
            }

            Console.WriteLine("======================================");

            throw;
        }

        return services;
    }

    /// <summary>Sobrecarga que además enlaza las clases de opciones (Patrón Options)
    /// con las secciones de appsettings.json de la capa API.</summary>
    public static IServiceCollection AddApplication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PasarelaOptions>(configuration.GetSection("Pasarela"));

        return services.AddApplication();
    }
}