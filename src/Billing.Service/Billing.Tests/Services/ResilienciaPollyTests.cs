using System.Net;
using Billing.Tests.Fakes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Extensions;
using NugetPackage_Rest.Interfaces.IServices;
using Polly;

namespace Billing.Tests.Services;

/// <summary>
/// Ejercicio 4 de la guía: la resiliencia se delega a Polly sobre el typed client.
/// Polly reintenta fallas transitorias (5xx, 408, 429, red) y NO reintenta una respuesta
/// 4xx definitiva. El bucle manual de PasarelaGateway sigue existiendo porque resuelve un
/// problema distinto: consultar si el cobro ya se aplicó antes de reenviarlo.
/// </summary>
public class ResilienciaPollyTests
{
    private class RespuestaDto
    {
        [Newtonsoft.Json.JsonProperty("data")]
        public string? Data { get; set; }
    }

    private static IRest CrearRest(Func<int, HttpResponseMessage> responder, out Func<int> intentos)
    {
        var contador = 0;
        intentos = () => contador;

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRequestLogging(ConfiguracionVacia());
        services.AddHttpClient<IRest, RestBuilder>(client => client.Timeout = TimeSpan.FromSeconds(10))
            .ConfigurePrimaryHttpMessageHandler(() => new FakeHttpMessageHandler(_ =>
            {
                contador++;
                return responder(contador);
            }))
            .AddStandardResilienceHandler(options =>
            {
                // Escenario controlado: los mismos valores que en AddInfraestructure,
                // con esperas cortas para que el test no tarde segundos.
                options.Retry.MaxRetryAttempts = 2;
                options.Retry.Delay = TimeSpan.FromMilliseconds(20);
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);
            });

        return services.BuildServiceProvider().GetRequiredService<IRest>();
    }

    [Fact]
    public async Task AnteUn503PollyReintentaYElLlamadorRecibeLaRespuestaExitosa()
    {
        var rest = CrearRest(intento => intento < 3
            ? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) { Content = new StringContent("{\"error\":\"no disponible\"}") }
            : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":\"TX-1\"}") },
            out var intentos);

        var dto = await rest.Get.WithoutAuth().WithUri("https://pasarela.example.com", "/v1/cobros")
            .DeserializeWithAsync<RespuestaDto>();

        Assert.Equal("TX-1", dto.Data);
        Assert.Equal(3, intentos());
    }

    [Fact]
    public async Task AgotadosLosReintentosPollyPropagaElErrorYLaLibreriaLoTraduceAApiException()
    {
        var rest = CrearRest(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("{\"error\":\"no disponible\"}")
        }, out var intentos);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            rest.Get.WithoutAuth().WithUri("https://pasarela.example.com", "/v1/cobros").GetContentAsStringAsync());

        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(503, exception.StatusCode);
        Assert.Equal(3, intentos());
    }

    [Fact]
    public async Task AnteUn404PollyNoReintentaPorqueLaRespuestaSeriaLaMisma()
    {
        var rest = CrearRest(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"error\":\"no existe\"}")
        }, out var intentos);

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            rest.Get.WithoutAuth().WithUri("https://pasarela.example.com", "/v1/cobros/99").GetContentAsStringAsync());

        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal(1, intentos());
    }

    [Fact]
    public async Task AnteUnaCaidaDeRedPollyReintentaYAlFinalDevuelveNetwork()
    {
        var rest = CrearRest(intento => intento < 3
            ? throw new HttpRequestException("conexión reiniciada")
            : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"data\":\"TX-2\"}") },
            out var intentos);

        var dto = await rest.Get.WithoutAuth().WithUri("https://pasarela.example.com", "/v1/cobros")
            .DeserializeWithAsync<RespuestaDto>();

        Assert.Equal("TX-2", dto.Data);
        Assert.Equal(3, intentos());
    }

    /// <summary>IConfiguration mínimo para enlazar "RestSettings" sin tocar appsettings.</summary>
    private static IConfiguration ConfiguracionVacia() =>
        new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build();
}