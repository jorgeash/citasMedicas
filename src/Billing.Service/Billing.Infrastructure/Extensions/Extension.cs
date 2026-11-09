using Billing.Core.Services;
using Billing.Infrastructure.Services;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Infrastructure.Extensions;

public static class Extension
{
    /// <summary>Todo el registro HTTP del microservicio vive aquí: la librería
    /// NugetPackageRest se enlaza con la configuración y se registra como
    /// typed client de IHttpClientFactory.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Enlaza "RestSettings" → IOptions<RequestSettings>
        services.AddRequestLogging(configuration);

        // 2. Registra IRest como typed client de IHttpClientFactory
        services.AddHttpClient<IRest, RestBuilder>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30); // al vencer → ApiException(Reason = Timeout)
        });

        // 3. Registra los servicios que usan IRest
        services.AddScoped<IPagoService, PagoService>();

        // 4. Cliente de Scheduling: si no hay BaseUrl configurada se usa el stub
        //    para no bloquear Billing mientras ese microservicio no exista.
        var schedulingBaseUrl = configuration["Services:Scheduling:BaseUrl"];
        if (string.IsNullOrWhiteSpace(schedulingBaseUrl))
        {
            services.AddScoped<ISchedulingClient, StubSchedulingClient>();
        }
        else
        {
            services.AddHttpClient<ISchedulingClient, SchedulingClient>(client =>
            {
                client.BaseAddress = new Uri(schedulingBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            });
        }

        return services;
    }
}
