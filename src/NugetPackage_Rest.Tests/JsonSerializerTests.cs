using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Tests.Fakes;
using Xunit;

using Microsoft.Extensions.Options;

namespace NugetPackage_Rest.Tests;

/// <summary>
/// Ejercicio 3: el serializador de JSON es intercambiable. Con Newtonsoft (por defecto) los
/// DTOs usan [JsonProperty]; con System.Text.Json deben usar [JsonPropertyName].
/// </summary>
public class JsonSerializerTests
{
    public class NewtonsoftOrder
    {
        [Newtonsoft.Json.JsonProperty("order_id")]
        public int OrderId { get; set; }
    }

    public class SystemTextOrder
    {
        [JsonPropertyName("order_id")]
        public int OrderId { get; set; }
    }

    private static RestBuilder CreateRestBuilder(HttpMessageHandler handler, JsonSerializerChoice choice) =>
        new(new HttpClient(handler), Options.Create(new RequestSettings
        {
            EnableRequestLogs = false,
            JsonSerializer = choice
        }));

    [Fact]
    public void RequestSettings_PorDefecto_UsaNewtonsoft()
    {
        var settings = new RequestSettings();

        Assert.Equal(JsonSerializerChoice.Newtonsoft, settings.JsonSerializer);
        Assert.IsType<NugetPackage_Rest.Serializers.NewtonsoftJsonSerializer>(settings.Serializer);
    }

    [Fact]
    public async Task WithBody_ConNewtonsoft_RespetaJsonPropertyDelDto()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
        });

        await CreateRestBuilder(handler, JsonSerializerChoice.Newtonsoft)
            .Post.WithoutAuth()
            .WithUri("https://api.example.com", "/api/Ordenes")
            .WithBody(new NewtonsoftOrder { OrderId = 42 })
            .GetContentAsStringAsync();

        Assert.Equal("{\"order_id\":42}", await enviado!.Content!.ReadAsStringAsync());
    }

    [Fact]
    public async Task DeserializeWithAsync_ConNewtonsoft_LeeJsonProperty()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"order_id\":42}")
        });

        var result = await CreateRestBuilder(handler, JsonSerializerChoice.Newtonsoft)
            .Get.WithoutAuth()
            .WithUri("https://api.example.com", "/api/Ordenes/42")
            .DeserializeWithAsync<NewtonsoftOrder>();

        Assert.Equal(42, result.OrderId);
    }

    [Fact]
    public async Task ConSystemTextJson_SerializaYDeserializaConJsonPropertyName()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"order_id\":42}") };
        });

        var rest = CreateRestBuilder(handler, JsonSerializerChoice.SystemTextJson);

        var body = await rest.Post.WithoutAuth()
            .WithUri("https://api.example.com", "/api/Ordenes")
            .WithBody(new SystemTextOrder { OrderId = 42 })
            .GetContentAsStringAsync();

Assert.Equal("{\"order_id\":42}", body);
        Assert.Equal("{\"order_id\":42}", await enviado!.Content!.ReadAsStringAsync());

        var leido = await rest.Get.WithoutAuth()
            .WithUri("https://api.example.com", "/api/Ordenes/42")
            .DeserializeWithAsync<SystemTextOrder>();

        Assert.Equal(42, leido.OrderId);
    }
}