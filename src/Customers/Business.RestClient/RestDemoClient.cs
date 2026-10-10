using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Interfaces.IServices;

namespace Business.RestClient;

/// <summary>
/// Cliente de la actividad. Todas las operaciones HTTP se hacen con los métodos de la
/// libreria NugetPackage_Rest (IRest): nunca con HttpClient ni WebRequest.
///
///   IRest.Get  -> GET    (sin body)
///   IRest.Post -> POST   (con WithBody)
/// </summary>
public class RestDemoClient
{
    private readonly IRest _rest;
    private readonly string _addressesBaseUrl;
    private readonly string _customersBaseUrl;

    public RestDemoClient(IRest rest, string addressesBaseUrl, string customersBaseUrl)
    {
        _rest = rest;
        _addressesBaseUrl = addressesBaseUrl.TrimEnd('/');
        _customersBaseUrl = customersBaseUrl.TrimEnd('/');
    }

    /// <summary>
    /// GET /api/positions del microservicio de Addresses, con parametros de query Armados
    /// por WithQuery. Devuelve el catalogo de direcciones (datos de prueba).
    /// </summary>
    public async Task<List<LocationContract>> ListarDireccionesAsync(int pageSize, CancellationToken cancellationToken = default)
    {
        var response = await _rest.Get
            .WithoutAuth()
            .WithQuery(new Dictionary<string, string>
            {
                ["pageNumber"] = "1",
                ["pageSize"] = pageSize.ToString(),
                ["orderBy"] = "Id"
            })
            .WithUri(_addressesBaseUrl, "/api/positions")
            .DeserializeWithAsync<ApiResponse<PagedData<LocationContract>>>(cancellationToken);

        return response?.Data?.Data ?? new List<LocationContract>();
    }

    /// <summary>GET /api/positions/{id}: una direccion concreta del catalogo.</summary>
    public async Task<LocationContract?> ObtenerDireccionAsync(int locationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _rest.Get
                .WithoutAuth()
                .WithUri(_addressesBaseUrl, $"/api/positions/{locationId}")
                .DeserializeWithAsync<ApiResponse<LocationContract>>(cancellationToken);

            return response?.Data;
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }

    /// <summary>
    /// POST /api/customers del microservicio de clientes. Este es el método usado para crear
    /// los clientes de la actividad, con IRest.Post.WithBody(...).DeserializeWithAsync&lt;T&gt;().
    /// </summary>
    public async Task<CustomerContract> CrearClienteAsync(NewCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _rest.Post
            .WithoutAuth()
            .WithUri(_customersBaseUrl, "/api/customers")
            .WithBody(request)
            .DeserializeWithAsync<ApiResponse<CustomerContract>>(cancellationToken);

        return response.Data;
    }

    /// <summary>GET /api/customers/{id}: relee el cliente recien creado (verificacion).</summary>
    public async Task<CustomerContract?> ObtenerClienteAsync(int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _rest.Get
                .WithoutAuth()
                .WithUri(_customersBaseUrl, $"/api/customers/{customerId}")
                .DeserializeWithAsync<ApiResponse<CustomerContract>>(cancellationToken);

            return response?.Data;
        }
        catch (ApiException ex) when (ex.StatusCode == 404)
        {
            return null;
        }
    }
}