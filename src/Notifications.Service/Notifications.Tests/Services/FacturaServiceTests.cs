using System.Net;
using Notifications.Core.Configs;
using Notifications.Core.Exceptions;
using Notifications.Infraestructure.Services;
using Notifications.Tests.Fakes;
using NugetPackage_Rest.Exceptions;

namespace Notifications.Tests.Services;

public class FacturaServiceTests
{
    private static DownstreamOptions Options() => new() { BillingBaseUrl = "http://billing" };

    [Fact]
    public async Task ConsultarFacturaPagadaAsync_LlamaABillingYDeserializaLaFactura()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"data\":{\"facturaId\":42,\"paciente\":\"Ana Ruiz\",\"email\":\"ana@clinica.test\",\"monto\":120.50,\"moneda\":\"USD\",\"estado\":\"PAGADA\",\"transaccionId\":\"TX-9\"}}")
            };
        });

        var service = new FacturaService(TestRestFactory.Create(handler), TestRestFactory.CreateOptions(Options()));

        var factura = await service.ConsultarFacturaPagadaAsync(42);

        Assert.Equal(42, factura.FacturaId);
        Assert.Equal("Ana Ruiz", factura.Paciente);
        Assert.Equal("ana@clinica.test", factura.Email);
        Assert.Equal(120.50m, factura.Monto);
        Assert.Equal("PAGADA", factura.Estado);
        Assert.Equal("TX-9", factura.TransaccionId);
        Assert.Equal(HttpMethod.Get, enviado!.Method);
        Assert.Equal("http://billing/api/Cobros/42/detalle", enviado.RequestUri!.ToString());
    }

    [Fact]
    public async Task ConsultarFacturaPagadaAsync_ConNotFound_LanzaDomainExceptionDeFacturaNoEncontrada()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"error\":\"no existe\"}")
        });

        var service = new FacturaService(TestRestFactory.Create(handler), TestRestFactory.CreateOptions(Options()));

        var exception = await Assert.ThrowsAsync<DomainException>(() => service.ConsultarFacturaPagadaAsync(99));

        Assert.Equal(Errores.FACTURA_NO_ENCONTRADA, exception.Codigo);
    }

    [Fact]
    public async Task ConsultarFacturaPagadaAsync_SinData_EnElSobre_LanzaDomainException()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"succeeded\":true,\"data\":null}")
        });

        var service = new FacturaService(TestRestFactory.Create(handler), TestRestFactory.CreateOptions(Options()));

        var exception = await Assert.ThrowsAsync<DomainException>(() => service.ConsultarFacturaPagadaAsync(7));

        Assert.Equal(Errores.FACTURA_NO_ENCONTRADA, exception.Codigo);
    }

    [Fact]
    public async Task ConsultarFacturaPagadaAsync_SinRed_PropagaApiExceptionConMotivoNetwork()
    {
        var handler = new FakeHttpMessageHandler(new HttpRequestException("Connection refused"));

        var service = new FacturaService(TestRestFactory.Create(handler), TestRestFactory.CreateOptions(Options()));

        var exception = await Assert.ThrowsAsync<ApiException>(() => service.ConsultarFacturaPagadaAsync(1));

        Assert.Equal(ApiFailureReason.Network, exception.Reason);
    }
}
