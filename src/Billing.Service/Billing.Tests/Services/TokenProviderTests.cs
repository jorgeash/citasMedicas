using System.Net;
using Billing.Core.Configs;
using Billing.Core.Exceptions;
using Billing.Infraestructure.Services;
using Billing.Tests.Fakes;

namespace Billing.Tests.Services;

public class TokenProviderTests
{
    private static PasarelaOptions Options() => new()
    {
        BaseUrl = "https://pasarela.example",
        ClientId = "clinica-demo",
        ClientSecret = "secreto"
    };

    [Fact]
    public async Task GetTokenAsync_PideTokenConFormUrlEncodedYDeserializaElAccessToken()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"access_token\":\"abc123\",\"expires_in\":3600}")
            };
        });

        var provider = new TokenProvider(TestRestFactory.CreateScopeFactory(handler), TestRestFactory.CreateOptions(Options()));

        var token = await provider.GetTokenAsync();

        Assert.Equal("abc123", token);
        Assert.Equal("https://pasarela.example/oauth/token", enviado!.RequestUri!.ToString());
        Assert.Equal("application/x-www-form-urlencoded", enviado.Content!.Headers.ContentType!.MediaType);
        var body = await enviado.Content.ReadAsStringAsync();
        Assert.Contains("grant_type=client_credentials", body);
        Assert.Contains("client_id=clinica-demo", body);
    }

    [Fact]
    public async Task GetTokenAsync_CacheaElTokenYNoRepiteLaLlamada()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"access_token\":\"abc123\",\"expires_in\":3600}")
        });

        var provider = new TokenProvider(TestRestFactory.CreateScopeFactory(handler), TestRestFactory.CreateOptions(Options()));

        var primero = await provider.GetTokenAsync();
        var segundo = await provider.GetTokenAsync();

        Assert.Equal(primero, segundo);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task GetTokenAsync_SinAccessToken_LanzaDomainExceptionDeAutenticacion()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"access_token\":null,\"expires_in\":3600}")
        });

        var provider = new TokenProvider(TestRestFactory.CreateScopeFactory(handler), TestRestFactory.CreateOptions(Options()));

        var exception = await Assert.ThrowsAsync<DomainException>(() => provider.GetTokenAsync());
        Assert.Equal(Errores.AUTENTICACION_FALLIDA, exception.Codigo);
    }
}