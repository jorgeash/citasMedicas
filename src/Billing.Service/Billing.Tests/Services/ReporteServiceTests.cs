using System.Net;
using System.Text;
using Billing.Core.Configs;
using Billing.Infraestructure.Services;
using Billing.Tests.Fakes;

namespace Billing.Tests.Services;

public class ReporteServiceTests
{
    [Fact]
    public async Task DescargarComprobanteAsync_DevuelveLosBytesDelPdf()
    {
        var pdf = Encoding.UTF8.GetBytes("%PDF-1.4 comprobante");
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(pdf) };
        });

        var service = new ReporteService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new DownstreamOptions { ReportesBaseUrl = "http://reportes" }));

        var resultado = await service.DescargarComprobanteAsync(77);

        Assert.Equal(pdf, resultado);
        Assert.Equal("http://reportes/api/Reportes/facturas/77/comprobante", enviado!.RequestUri!.ToString());
    }

    [Fact]
    public async Task EnviarComprobanteAsync_EnviaMultipartConElArchivo()
    {
        HttpRequestMessage? enviado = null;
        string? body = null;
        // El body se lee dentro del handler: ReporteService descarta el MultipartFormDataContent
        // con "using" al terminar la llamada.
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            body = req.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        });

        var service = new ReporteService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new DownstreamOptions { ReportesBaseUrl = "http://reportes" }));

        await service.EnviarComprobanteAsync(77, Encoding.UTF8.GetBytes("%PDF-1.4"));

        var content = enviado!.Content!;
        Assert.Equal("multipart/form-data", content.Headers.ContentType!.MediaType);
        Assert.Contains("name=comprobante", body);
        Assert.Contains("filename=factura-77.pdf", body);
        Assert.Contains("%PDF-1.4", body);
        Assert.Contains("name=facturaId", body);
    }
}