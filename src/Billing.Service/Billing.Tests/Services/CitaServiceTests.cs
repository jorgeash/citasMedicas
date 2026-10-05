using System.Net;
using Billing.Core.Configs;
using Billing.Infraestructure.Services;
using Billing.Tests.Fakes;

namespace Billing.Tests.Services;

public class CitaServiceTests
{
    [Fact]
    public async Task EstaDisponibleAsync_CuposMayoresACero_DevuelveTrueYQueryCorrecta()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\":3}")
            };
        });

        var service = new CitaService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new DownstreamOptions { CitasBaseUrl = "http://citas" }));

        var disponible = await service.EstaDisponibleAsync(12, new DateTime(2026, 3, 15));

        Assert.True(disponible);
        Assert.Equal(HttpMethod.Get, enviado!.Method);
        Assert.Equal("http://citas/api/Citas/disponibilidad?servicioId=12&fecha=2026-03-15", enviado.RequestUri!.ToString());
    }

    [Fact]
    public async Task EstaDisponibleAsync_SinCupos_DevuelveFalse()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"data\":0}")
        });

        var service = new CitaService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new DownstreamOptions { CitasBaseUrl = "http://citas" }));

        var disponible = await service.EstaDisponibleAsync(12, new DateTime(2026, 3, 15));

        Assert.False(disponible);
    }
}