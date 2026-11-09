using System.Net;
using System.Net.Http;
using Billing.Core.Configs;
using Billing.Infrastructure.Services;
using Billing.Tests.Fakes;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Interfaces.IServices;

namespace Billing.Tests.Tests;

public class PagoServiceTests
{
    private static IRest CreateRest(HttpClient client)
    {
        return new RestBuilder(client, Options.Create(new RequestSettings()));
    }

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
        var service = new PagoService(CreateRest(new HttpClient(handler)), Options.Create(new PasarelaOptions
        {
            BaseUrl = "https://pasarela"
        }));

        var transaccionId = await service.ProcesarPagoAsync(Guid.NewGuid(), 25.50m, "USD");

        Assert.Equal("TX-0001", transaccionId);
        Assert.Equal(HttpMethod.Post, enviado!.Method);
        Assert.Equal("https://pasarela/api/Pagos/procesar", enviado.RequestUri!.ToString());
    }

    [Fact]
    public async Task ProcesarPagoAsync_CuandoLaPasarelaNoDevuelveTransaccion_LanzaInvalidOperationException()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"data\":null}")
        });
        var service = new PagoService(CreateRest(new HttpClient(handler)), Options.Create(new PasarelaOptions
        {
            BaseUrl = "https://pasarela"
        }));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ProcesarPagoAsync(Guid.NewGuid(), 10m, "USD"));

        Assert.Contains("transacción", ex.Message);
    }

    [Fact]
    public async Task ProcesarPagoAsync_CuandoLaPasarelaRespondeError_HerdaApiExceptionConCodigoYBody()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("{\"error\":\"down\"}")
        });
        var service = new PagoService(CreateRest(new HttpClient(handler)), Options.Create(new PasarelaOptions
        {
            BaseUrl = "https://pasarela"
        }));

        var ex = await Assert.ThrowsAsync<NugetPackage_Rest.Exceptions.ApiException>(
            () => service.ProcesarPagoAsync(Guid.NewGuid(), 10m, "USD"));

        Assert.Equal(NugetPackage_Rest.Exceptions.ApiFailureReason.HttpError, ex.Reason);
        Assert.Equal(503, ex.StatusCode);
        Assert.Equal("{\"error\":\"down\"}", ex.ResponseBody);
    }
}
