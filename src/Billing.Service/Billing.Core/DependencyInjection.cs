using System.Reflection;
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
}