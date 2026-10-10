using System.Net;
using Business.Core.Configs;
using Business.Core.Exceptions;
using Business.Infraestructure.Services;
using Business.Tests.Fakes;

namespace Business.Tests.Services;

/// <summary>
/// Verifica la integracion con la libreria NugetPackage_Rest: URL construida, metodo,
/// deserializacion del sobre y traduccion del 404 a "direccion inexistente".
/// </summary>
public class LocationServiceTests
{
    [Fact]
    public async Task ObtenerLocationAsync_DireccionExistente_ConsultaElEndpointCorrecto()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                  "succeeded": true,
                  "data": {
                    "Id": 1,
                    "Address": "Av. Amazonas N34-1234, Edificio Clinica Norte",
                    "ZipCode": "EC010101",
                    "CityId": 1,
                    "CityName": "Quito",
                    "CountryId": 1,
                    "CountryName": "Ecuador",
                    "Latitude": -0.1807,
                    "Longitude": -78.4678,
                    "CustomerId": null,
                    "IsActive": true
                  }
                }
                """)
            };
        });

        var service = new LocationService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new ApiAddressesOptions { AddressesBaseUrl = "http://localhost:5310" }));

        var location = await service.ObtenerLocationAsync(1);

        Assert.NotNull(location);
        Assert.Equal(1, location!.Id);
        Assert.Equal("Av. Amazonas N34-1234, Edificio Clinica Norte", location.Address);
        Assert.Equal("Quito", location.CityName);
        Assert.Equal(HttpMethod.Get, enviado!.Method);
        Assert.Equal("http://localhost:5310/api/positions/1", enviado.RequestUri!.ToString());
    }

    [Fact]
    public async Task ObtenerLocationAsync_NotFound_DevuelveNull()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"succeeded\":false,\"errorMessage\":\"no existe\"}")
        });

        var service = new LocationService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new ApiAddressesOptions { AddressesBaseUrl = "http://localhost:5310" }));

        var location = await service.ObtenerLocationAsync(999);

        Assert.Null(location);
    }

    [Fact]
    public async Task ObtenerLocationAsync_ErrorDelServidor_LanzaDomainException()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("boom")
        });

        var service = new LocationService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new ApiAddressesOptions { AddressesBaseUrl = "http://localhost:5310" }));

        var ex = await Assert.ThrowsAsync<DomainException>(() => service.ObtenerLocationAsync(1));

        Assert.Equal(Errores.ADDRESSES_NO_DISPONIBLE, ex.Codigo);
    }

    [Fact]
    public async Task ObtenerLocationAsync_LocationIdInvalido_NoLlamaAlMicroservicio()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}")
        });

        var service = new LocationService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new ApiAddressesOptions { AddressesBaseUrl = "http://localhost:5310" }));

        var location = await service.ObtenerLocationAsync(0);

        Assert.Null(location);
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task ListarLocationsAsync_EnvíaQueryStringConWithQuery()
    {
        HttpRequestMessage? enviado = null;
        var handler = new FakeHttpMessageHandler(req =>
        {
            enviado = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                { "succeeded": true, "data": { "totalRecords": 4, "currentPage": 1, "pageSize": 50,
                  "data": [ { "Id": 1, "Address": "Av. Amazonas N34-1234", "CityName": "Quito" } ] } }
                """)
            };
        });

        var service = new LocationService(
            TestRestFactory.Create(handler),
            TestRestFactory.CreateOptions(new ApiAddressesOptions { AddressesBaseUrl = "http://localhost:5310" }));

        var locations = await service.ListarLocationsAsync(50);

        Assert.Single(locations);
        Assert.Equal(1, locations[0].Id);
        Assert.Equal("http://localhost:5310/api/positions?pageNumber=1&pageSize=50&orderBy=Id", enviado!.RequestUri!.ToString());
    }
}