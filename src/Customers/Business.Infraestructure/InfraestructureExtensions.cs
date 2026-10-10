using Business.Core.Configs;
using Business.Core.Interfaces.Services;
using Business.Infraestructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;
using Polly;

namespace Business.Infraestructure;

public static class InfraestructureExtensions
{
    /// <summary>
    /// Todo el registro HTTP del microservicio vive aqui: la libreria NugetPackage_Rest
    /// solo se conoce desde esta capa. En el microservicio original esta integracion se
    /// hacia con la libreria T-Rest (ITRest/TRest); aqui se reemplaza por IRest/RestBuilder.
    /// </summary>
    public static IServiceCollection AddInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Enlaza "RestSettings" -> IOptions<RequestSettings>
        services.AddRequestLogging(configuration);

        // 2. Registra IRest como typed client de IHttpClientFactory (pool de handlers,
        //    sin agotar sockets) con la politica de resiliencia estandar de Polly:
        //    reintentos con backoff, timeout total y circuit breaker.
        services.AddHttpClient<IRest, RestBuilder>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("X-Origen", "Customers");
        })
        .AddStandardResilienceHandler(options =>
        {
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
            options.Retry.MaxRetryAttempts = 2;
            options.Retry.Delay = TimeSpan.FromMilliseconds(500);
            options.Retry.BackoffType = DelayBackoffType.Exponential;
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.MinimumThroughput = 10;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
        });

        // 3. Registra el servicio que consume el microservicio de Addresses con IRest
        services.AddScoped<ILocationService, LocationService>();

        return services;
    }
}