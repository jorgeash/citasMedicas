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
            // Scheduling exige JWT (FallbackPolicy), así que Billing se
            // autentica como cuenta de servicio antes de cada llamada.
            var cuenta = configuration.GetSection("Services:Scheduling:ServiceAccount")
                .Get<ServiceAccountSettings>();

            if (cuenta is null || string.IsNullOrWhiteSpace(cuenta.Username) || string.IsNullOrWhiteSpace(cuenta.Password))
            {
                throw new InvalidOperationException(
                    "Falta Services:Scheduling:ServiceAccount (Username/Password) en appsettings.json. " +
                    "Scheduling.Api exige JWT y Billing necesita una cuenta de servicio para llamarlo.");
            }

            var identityBaseUrl = configuration["Identity:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "Falta Identity:BaseUrl en appsettings.json (lo usa Billing para obtener el token de servicio).");

            // HttpClient propio para Identity: sin este handler, para no
            // recursar (este handler llama a Identity, que a su vez no necesita token).
            services.AddSingleton(cuenta);
            services.AddHttpClient("identity", client =>
            {
                client.BaseAddress = new Uri(identityBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddTransient<ServiceTokenHandler>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return new ServiceTokenHandler(cuenta, factory.CreateClient("identity"));
            });

            services.AddHttpClient<ISchedulingClient, SchedulingClient>(client =>
            {
                client.BaseAddress = new Uri(schedulingBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler<ServiceTokenHandler>();
        }

        return services;
    }
}
