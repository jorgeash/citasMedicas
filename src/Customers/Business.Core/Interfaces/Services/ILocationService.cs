using Business.Core.DTOs.External;

namespace Business.Core.Interfaces.Services;

/// <summary>
/// Acceso al microservicio de direcciones. La implementacion real vive en la capa
/// Infraestructure y usa el cliente IRest de la libreria NugetPackage_Rest; aqui solo se
/// declara el contrato para que Core no dependa de la libreria.
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Devuelve la direccion (Addresses_Position) con el Id indicado, o null si el
    /// microservicio de direcciones responde 404. Es la validacion del LocationId al crear
    /// un cliente. Si el servicio no esta disponible lanza DomainException(ADDRESSES_NO_DISPONIBLE).
    /// </summary>
    Task<LocationDto?> ObtenerLocationAsync(int locationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista el catalogo de direcciones (datos de prueba del microservicio de Addresses).
    /// Si el servicio no esta disponible lanza DomainException(ADDRESSES_NO_DISPONIBLE).
    /// </summary>
    Task<IReadOnlyList<LocationDto>> ListarLocationsAsync(int pageSize = 50, CancellationToken cancellationToken = default);
}