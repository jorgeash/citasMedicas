using System.Net;
using Business.Core.Configs;
using Business.Core.DTOs;
using Business.Core.DTOs.External;
using Business.Core.Exceptions;
using Business.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Interfaces.IServices;
using Serilog;

namespace Business.Infraestructure.Services;

/// <summary>
/// Cliente del microservicio de direcciones (Addresses) construido sobre la libreria
/// NugetPackage_Rest. Sustituye a la clase AddressService del microservicio original,
/// que usaba la libreria T-Rest.
///
/// Cadenas de la libreria REST usadas aqui:
///   _rest.Get.WithoutAuth().WithUri(baseUrl, endpoint).DeserializeWithAsync&lt;T&gt;()
/// </summary>
internal class LocationService : ILocationService
{
    private readonly IRest _rest;
    private readonly ApiAddressesOptions _options;

    public LocationService(IRest rest, IOptions<ApiAddressesOptions> options)
    {
        _rest = rest;
        _options = options.Value;
    }

    /// <summary>
    /// GET {AddressesBaseUrl}/api/positions/{id}. Devuelve null cuando el catalogo responde
    /// 404 (direccion inexistente) y propaga un DomainException si el servicio no responde.
    /// </summary>
    public async Task<LocationDto?> ObtenerLocationAsync(int locationId, CancellationToken cancellationToken = default)
    {
        if (locationId <= 0)
            return null;

        try
        {
            var response = await _rest.Get
                .WithoutAuth()
                .WithUri(_options.AddressesBaseUrl, $"/api/positions/{locationId}")
                .DeserializeWithAsync<HttpResponse<LocationDto>>(cancellationToken);

            return response?.Data;
        }
        catch (ApiException ex) when (ex.StatusCode == (int)HttpStatusCode.NotFound)
        {
            // Patron de la libreria: el 404 se traduce a "no existe la direccion".
            Log.ForContext("LocationId", locationId)
               .Information("El microservicio de Addresses responde 404: la direccion {LocationId} no existe.", locationId);

            return null;
        }
        catch (ApiException ex)
        {
            Log.ForContext("LocationId", locationId).ForContext("Reason", ex.Reason).Error(ex, "Fallo al consultar la direccion en Addresses.");

            throw new DomainException(Errores.ADDRESSES_NO_DISPONIBLE,
                $"El microservicio de direcciones no esta disponible: {ex.Message}");
        }
    }

    /// <summary>
    /// GET {AddressesBaseUrl}/api/positions?pageNumber=1&amp;pageSize=... usando WithQuery de la
    /// libreria REST, que arma la query string escapando cada valor.
    /// </summary>
    public async Task<IReadOnlyList<LocationDto>> ListarLocationsAsync(int pageSize = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _rest.Get
                .WithoutAuth()
                .WithQuery(new Dictionary<string, string>
                {
                    ["pageNumber"] = "1",
                    ["pageSize"] = pageSize.ToString(),
                    ["orderBy"] = "Id"
                })
                .WithUri(_options.AddressesBaseUrl, "/api/positions")
                .DeserializeWithAsync<HttpResponse<PagedDto<List<LocationDto>>>>(cancellationToken);

            return response?.Data?.Data ?? new List<LocationDto>();
        }
        catch (ApiException ex)
        {
            Log.ForContext("Reason", ex.Reason).Error(ex, "Fallo al listar el catalogo de direcciones de Addresses.");

            throw new DomainException(Errores.ADDRESSES_NO_DISPONIBLE,
                $"El microservicio de direcciones no esta disponible: {ex.Message}");
        }
    }
}