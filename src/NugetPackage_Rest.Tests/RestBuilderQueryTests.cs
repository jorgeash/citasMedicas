using System.Net;
using System.Net.Http;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Tests.Fakes;
using Xunit;

using Microsoft.Extensions.Options;

namespace NugetPackage_Rest.Tests;

public class RestBuilderQueryTests
{
    private static RestBuilder CreateRestBuilder(HttpMessageHandler handler) =>
        new(new HttpClient(handler), Options.Create(new RequestSettings { EnableRequestLogs = false }));

    [Fact]
    public async Task WithQuery_AgregaParametrosEscapadosALaUri()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        });

        var result = await CreateRestBuilder(handler)
            .Get.WithoutAuth()
            .WithQuery(new Dictionary<string, string> { ["sku"] = "A-1 2", ["cantidad"] = "3" })
            .WithUri("https://api.example.com", "/api/Inventario/disponibilidad")
            .GetContentAsStringAsync();

Assert.Equal("ok", result);
        // AbsoluteUri conserva el escapado; ToString() lo desescapa.
        Assert.Equal(
            "https://api.example.com/api/Inventario/disponibilidad?sku=A-1%202&cantidad=3",
            enviado!.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task WithQuery_SePuedeLlamarVariasVecesYCombinaConUnEndpointQueYaTraeQuery()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        });

        await CreateRestBuilder(handler)
            .Get.WithoutAuth()
            .WithQuery(new Dictionary<string, string> { ["a"] = "1" })
            .WithQuery(new Dictionary<string, string> { ["b"] = "2" })
            .WithUri("https://api.example.com", "/api/recurso?existente=si")
            .GetContentAsStringAsync();

        Assert.Equal("https://api.example.com/api/recurso?existente=si&a=1&b=2", enviado!.RequestUri!.ToString());
    }

    [Fact]
    public async Task WithQuery_SinParametros_NoAlteraLaUri()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        });

        await CreateRestBuilder(handler)
            .Get.WithoutAuth()
            .WithQuery(new Dictionary<string, string>())
            .WithUri("https://api.example.com", "/api/recurso")
            .GetContentAsStringAsync();

        Assert.Equal("https://api.example.com/api/recurso", enviado!.RequestUri!.ToString());
    }
}