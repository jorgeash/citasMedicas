using System.Net;
using Billing.Core.Configs;
using Billing.Core.DTOs.External;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces.Services;
using Billing.Infraestructure.Services;
using Billing.Tests.Fakes;
using NugetPackage_Rest.Exceptions;

namespace Billing.Tests.Services;

public class PasarelaGatewayTests
{
    private sealed class StubTokenProvider : ITokenProvider
    {
        public Task<string> GetTokenAsync(CancellationToken cancellationToken = default) => Task.FromResult("jwt-123");
    }

    private static PasarelaGateway CreateGateway(FakeHttpMessageHandler handler, int maxReintentos = 2) =>
        new(TestRestFactory.Create(handler),
            new StubTokenProvider(),
            TestRestFactory.CreateOptions(new PasarelaOptions
            {
                BaseUrl = "https://pasarela.example",
                MaxReintentos = maxReintentos
            }));

    private static CobroRequest Request() => new() { Referencia = "FAC-77", Monto = 25.50m, Moneda = "USD" };

    [Fact]
    public async Task CobrarAsync_EnviaBearerYBodyJsonYDeserializaLaRespuesta()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"estado\":\"APROBADO\",\"transaccionId\":\"TX-9\"}")
            };
        });

        var response = await CreateGateway(handler).CobrarAsync(Request());

        Assert.Equal("APROBADO", response.Estado);
        Assert.Equal("TX-9", response.TransaccionId);
        Assert.Equal("https://pasarela.example/v1/cobros", enviado!.RequestUri!.ToString());
        Assert.Equal("Bearer", enviado.Headers.Authorization!.Scheme);
        Assert.Equal("jwt-123", enviado.Headers.Authorization.Parameter);
        var body = await enviado.Content!.ReadAsStringAsync();
        Assert.Contains("\"referencia\":\"FAC-77\"", body);
        Assert.Contains("\"moneda\":\"USD\"", body);
    }

    [Fact]
    public async Task CobrarAsync_FallaDeRed_ConsultaElCobroAntesDeReintentarYNoRepiteSiYaSeAplico()
    {
        var intentos = 0;
        var handler = new FakeHttpMessageHandler(req =>
        {
            intentos++;
            if (req.Method == HttpMethod.Post && intentos == 1)
                throw new HttpRequestException("Conexión rechazada");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"estado\":\"APROBADO\",\"transaccionId\":\"TX-9\"}")
            };
        });

        var response = await CreateGateway(handler, maxReintentos: 2).CobrarAsync(Request());

        Assert.Equal("APROBADO", response.Estado);
        // 1 POST fallido + 1 GET de consulta que confirma que el cobro sí se aplicó.
        Assert.Equal(2, handler.Requests.Count);
        Assert.Equal(HttpMethod.Get, handler.Requests[1].Method);
        Assert.Equal("https://pasarela.example/v1/cobros/FAC-77", handler.Requests[1].RequestUri!.ToString());
    }

    [Fact]
    public async Task CobrarAsync_SinRespuestaUtil_LanzaDomainExceptionDePasarelaNoDisponible()
    {
        // Todos los POST fallan por timeout y las consultas nunca confirman el cobro:
        // tras agotar los reintentos debe traduci��se a un error de negocio.
        var handler = new FakeHttpMessageHandler(req =>
        {
            if (req.Method == HttpMethod.Post)
                throw new TaskCanceledException("Timed out");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"estado\":\"PENDIENTE\"}")
            };
        });

        var gateway = CreateGateway(handler, maxReintentos: 2);

        var exception = await Assert.ThrowsAsync<DomainException>(() => gateway.CobrarAsync(Request()));
        Assert.Equal(Errores.PASARELA_NO_DISPONIBLE, exception.Codigo);
        Assert.Contains("FAC-77", exception.Message);
        // 3 POST (1 inicial + 2 reintentos) y 2 consultas entre reintentos.
        Assert.Equal(5, handler.Requests.Count);
    }

    [Fact]
    public async Task CobrarAsync_TarjetaRechazada_NoReintentaYPropagaElApiException()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"estado\":\"RECHAZADO\",\"mensajes\":[\"Tarjeta rechazada\"]}")
        });

        var exception = await Assert.ThrowsAsync<ApiException>(
            () => CreateGateway(handler).CobrarAsync(Request()));

        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(400, exception.StatusCode);
        Assert.Contains("Tarjeta rechazada", exception.ResponseBody);
        // Un HttpError no es transitorio: exactamente una llamada, sin reintentos.
        Assert.Single(handler.Requests);
    }
}