using System.Net;
using Billing.Core.Configs;
using Billing.Infraestructure.Services;
using Billing.Tests.Fakes;

namespace Billing.Tests.Services;

public class PagoServiceTests
{
    [Fact]
    public async Task ProcesarPagoAsync_DevuelveIdentificadorDeTransaccion()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\":\"TX-0001\"}")
            };
        });

        var service = new PagoService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new DownstreamOptions { PagosBaseUrl = "http://pagos" }));

        var transaccionId = await service.ProcesarPagoAsync(77, 25.50m, "USD");

        Assert.Equal("TX-0001", transaccionId);
        Assert.Equal("http://pagos/api/Pagos/procesar", enviado!.RequestUri!.ToString());
        Assert.Equal(HttpMethod.Post, enviado.Method);
        Assert.Equal("application/json", enviado.Content!.Headers.ContentType!.MediaType);
        Assert.Contains("\"facturaId\":77", await enviado.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ProcesarPagoAsync_SinIdentificador_LanzaInvalidOperationException()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"data\":null}")
        });

        var service = new PagoService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new DownstreamOptions { PagosBaseUrl = "http://pagos" }));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ProcesarPagoAsync(77, 25.50m, "USD"));
    }

    [Fact]
    public async Task ProcesarPagoAsync_ErrorHttp_LanzaApiExceptionConCodigoYBody()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("{\"data\":\"no disponible\"}")
        });

        var service = new PagoService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new DownstreamOptions { PagosBaseUrl = "http://pagos" }));

        var exception = await Assert.ThrowsAsync<NugetPackage_Rest.Exceptions.ApiException>(
            () => service.ProcesarPagoAsync(77, 25.50m, "USD"));

        Assert.Equal(NugetPackage_Rest.Exceptions.ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(503, exception.StatusCode);
    }
}