using Billing.Core.Configs;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(
        this IServiceCollection services,
        IConfiguration configuration)
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

        services.Configure<DownstreamOptions>(configuration.GetSection("Downstream"));
        services.Configure<PasarelaOptions>(configuration.GetSection("Pasarela"));

        return services;
    }
}