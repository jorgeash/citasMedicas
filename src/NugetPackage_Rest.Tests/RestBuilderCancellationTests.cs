using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NugetPackage_Rest.Builders;
using NugetPackage_Rest.Configs;
using NugetPackage_Rest.Exceptions;
using Xunit;

using Microsoft.Extensions.Options;
using NugetPackage_Rest.Tests.Models;

namespace NugetPackage_Rest.Tests;

/// <summary>
/// Ejercicio 1: los métodos terminales aceptan un CancellationToken. Si se cancela, la
/// librería lanza ApiException con Reason = Timeout, y el mensaje permite distinguir una
/// cancelación pedida por el consumidor de un timeout real del HttpClient.
/// </summary>
public class RestBuilderCancellationTests
{
    private sealed class NeverCompletingHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }

    private static RestBuilder CreateRestBuilder() =>
        new(new HttpClient(new NeverCompletingHandler()), Options.Create(new RequestSettings()));

    [Fact]
    public async Task GetContentAsStringAsync_CancellationTokenCancelado_LanzaApiExceptionConMotivoTimeout()
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            CreateRestBuilder().Get.WithoutAuth().WithUri("https://api.example.com", "/lento").GetContentAsStringAsync(cts.Token));

        Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
        Assert.Contains("cancelled by the caller", exception.Message);
    }

    [Fact]
    public async Task DeserializeWithAsync_CancellationTokenCancelado_LanzaApiExceptionConMotivoTimeout()
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            CreateRestBuilder().Get.WithoutAuth().WithUri("https://api.example.com", "/lento").DeserializeWithAsync<TestOrder>(cts.Token));

        Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
    }

    [Fact]
    public async Task GetContentAsByteArrayAsync_CancellationTokenCancelado_LanzaApiExceptionConMotivoTimeout()
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        var exception = await Assert.ThrowsAsync<ApiException>(() =>
            CreateRestBuilder().Get.WithoutAuth().WithUri("https://api.example.com", "/lento").GetContentAsByteArrayAsync(cts.Token));

        Assert.Equal(ApiFailureReason.Timeout, exception.Reason);
    }
}