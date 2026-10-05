using System.Net;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using Serilog;

namespace NugetPackage_Rest.LiveTests;

/// <summary>
/// Anexo de la guía: verificación contra un servidor HTTP real (no mockeado). Cada test hace
/// tráfico TCP de verdad y comprueba tanto lo que llegó al servidor como lo que devolvió la
/// librería. Ejecutar con: dotnet test src\NugetPackage_Rest.LiveTests
/// </summary>
public class LiveVerificationTests : IDisposable
{
    private readonly CapturingSink _sink = new();
    private readonly HttpClient _httpClient;

    public LiveVerificationTests()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(_sink)
            .CreateLogger();

        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        Log.CloseAndFlush();
    }

    private RestBuilder CreateRest(RequestSettings? settings = null) =>
        new(_httpClient, Options.Create(settings ?? new RequestSettings { EnableRequestLogs = false }));

    private class ComprobanteDto
    {
        [Newtonsoft.Json.JsonProperty("comprobanteId")]
        public string? ComprobanteId { get; set; }

        [Newtonsoft.Json.JsonProperty("estado")]
        public string? Estado { get; set; }
    }

    private class PagoDto
    {
        [Newtonsoft.Json.JsonProperty("facturaId")]
        public int FacturaId { get; set; }

        [Newtonsoft.Json.JsonProperty("monto")]
        public decimal Monto { get; set; }
    }

    [Fact]
    public async Task Get_ConQueryYHeaders_LlegaCompletoAlServidorYDeserializaLaRespuesta()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros/verificar", "{\"comprobanteId\":\"CB-9001\",\"estado\":\"aprobado\"}");

        var dto = await CreateRest().Get
            .WithoutAuth()
            .WithHeaders(new Dictionary<string, string> { ["X-Correlation-Id"] = "corr-123" })
            .WithQuery(new Dictionary<string, string> { ["canal"] = "web", ["codigo"] = "A 1" })
            .WithUri(server.BaseUri, "/api/Cobros/verificar")
            .DeserializeWithAsync<ComprobanteDto>();

        Assert.Equal("CB-9001", dto.ComprobanteId);
        Assert.Equal("aprobado", dto.Estado);

        var recibido = Assert.Single(server.Requests);
        Assert.Equal("GET", recibido.Method);
        Assert.Equal("/api/Cobros/verificar?canal=web&codigo=A%201", recibido.PathAndQuery);
        Assert.Equal("corr-123", recibido.Header("X-Correlation-Id"));
    }

    [Fact]
    public async Task Post_ConBearerYJson_EnvíaAuthorizationYElBodySerializado()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros", "{\"comprobanteId\":\"CB-777\",\"estado\":\"procesado\"}", HttpStatusCode.Created);

        var dto = await CreateRest().Post
            .WithBearer("token-abc")
            .WithUri(server.BaseUri, "/api/Cobros")
            .WithBody(new PagoDto { FacturaId = 77, Monto = 125.50m })
            .DeserializeWithAsync<ComprobanteDto>();

        Assert.Equal("CB-777", dto.ComprobanteId);

        var recibido = Assert.Single(server.Requests);
        Assert.Equal("POST", recibido.Method);
        Assert.Equal("Bearer token-abc", recibido.Header("Authorization"));
        Assert.Equal("application/json; charset=utf-8", recibido.Header("Content-Type"));
        Assert.Equal("{\"facturaId\":77,\"monto\":125.50}", recibido.Body);
    }

    [Fact]
    public async Task Post_ConFormUrlEncoded_EnvíaElContentTypeCorrespondiente()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros/token", "{\"estado\":\"ok\"}");

        await CreateRest().Post
            .WithBasic("usuario01", "secreto")
            .WithUri(server.BaseUri, "/api/Cobros/token")
            .WithFormUrlEncoded(new Dictionary<string, string> { ["usuario"] = "usuario01", ["clave"] = "secreto" })
            .GetContentAsStringAsync();

        var recibido = Assert.Single(server.Requests);
        Assert.Equal("application/x-www-form-urlencoded", recibido.Header("Content-Type"));
        Assert.Equal("usuario=usuario01&clave=secreto", recibido.Body);
        Assert.StartsWith("Basic ", recibido.Header("Authorization"));
    }

    [Fact]
    public async Task Post_ConFormData_EnvíaMultipartConElArchivo()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros/comprobante", "{\"estado\":\"archivado\"}");

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("contenido-pdf"), "archivo", "comprobante.pdf");

        await CreateRest().Post
            .WithBearer("token-abc")
            .WithUri(server.BaseUri, "/api/Cobros/comprobante")
            .WithFormData(form)
            .GetContentAsStringAsync();

        var recibido = Assert.Single(server.Requests);
        Assert.StartsWith("multipart/form-data", recibido.Header("Content-Type"));
        Assert.Contains("filename=comprobante.pdf", recibido.Body);
        Assert.Contains("contenido-pdf", recibido.Body);
    }

    [Fact]
    public async Task Get_ConRespuesta404_LanzaApiExceptionConStatusCodeYBody()
    {
        using var server = new StubHttpServer();

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            CreateRest().Get.WithoutAuth().WithUri(server.BaseUri, "/api/Cobros/inexistente").GetContentAsStringAsync());

        Assert.Equal(ApiFailureReason.HttpError, exception.Reason);
        Assert.Equal(404, exception.StatusCode);
        Assert.Contains("ruta no encontrada", exception.ResponseBody);
        Assert.Equal(HttpMethod.Get, exception.RequestMethod);
        Assert.Contains("/api/Cobros/inexistente", exception.RequestUri!.ToString());
    }

    [Fact]
    public async Task Get_QueExcedeElTimeout_LanzaApiExceptionConMotivoTimeout()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros/lento", "{\"estado\":\"tarde\"}", HttpStatusCode.OK, TimeSpan.FromSeconds(5));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            CreateRest().Get.WithoutAuth().WithUri(server.BaseUri, "/api/Cobros/lento").GetContentAsStringAsync());

        Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
        Assert.Null(exception.StatusCode);
    }

    [Fact]
    public async Task Get_ElServidorCortaLaConexion_LanzaApiExceptionConMotivoNetwork()
    {
        using var server = new StubHttpServer()
            .MapAbort("/api/Cobros/caida");

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            CreateRest().Get.WithoutAuth().WithUri(server.BaseUri, "/api/Cobros/caida").GetContentAsStringAsync());

        Assert.Equal(ApiFailureReason.Network, exception.Reason);
        Assert.Contains("network error", exception.Message);
    }

    [Fact]
    public async Task ConEnableRequestLogs_LaPeticionQuedaRegistradaEnSerilog()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros/log", "{\"estado\":\"ok\"}");
        _sink.Reset();

        await CreateRest(new RequestSettings { EnableRequestLogs = true })
            .Get.WithoutAuth()
            .WithQuery(new Dictionary<string, string> { ["debug"] = "1" })
            .WithUri(server.BaseUri, "/api/Cobros/log")
            .GetContentAsStringAsync();

        Assert.True(_sink.Contains("/api/Cobros/log"), "El log debe mencionar el endpoint llamado.");
    }

    [Fact]
    public async Task ConCancellationTokenDelConsumidor_LanzaApiExceptionYNoEsperaAlServidor()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros/lento", "{\"estado\":\"tarde\"}", HttpStatusCode.OK, TimeSpan.FromSeconds(5));
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            CreateRest().Get.WithoutAuth().WithUri(server.BaseUri, "/api/Cobros/lento").GetContentAsStringAsync(cts.Token));

        Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
        Assert.Contains("cancelled by the caller", exception.Message);
    }

    [Fact]
    public async Task Put_Patch_Delete_LleganConElVerboYElBodyEsperados()
    {
        using var server = new StubHttpServer()
            .Map("/api/Cobros/1", "{\"estado\":\"actualizado\"}")
            .Map("/api/Cobros/1/estado", "{\"estado\":\"anulado\"}");

        var rest = CreateRest();

        await rest.Put.WithoutAuth().WithUri(server.BaseUri, "/api/Cobros/1")
            .WithBody(new PagoDto { FacturaId = 1, Monto = 10m }).GetContentAsStringAsync();

        await rest.Patch.WithoutAuth().WithUri(server.BaseUri, "/api/Cobros/1/estado")
            .WithBody(new { estado = "anulado" }).GetContentAsStringAsync();

        await rest.Delete.WithoutAuth().WithUri(server.BaseUri, "/api/Cobros/1")
            .WithBody(new { motivo = "duplicado" }).GetContentAsStringAsync();

        Assert.Equal(new[] { "PUT", "PATCH", "DELETE" }, server.Requests.Select(r => r.Method).ToArray());
        Assert.Equal("{\"facturaId\":1,\"monto\":10.0}", server.Requests[0].Body);
        Assert.Equal("{\"motivo\":\"duplicado\"}", server.Requests[2].Body);
    }
}