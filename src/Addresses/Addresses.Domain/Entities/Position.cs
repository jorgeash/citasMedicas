using Shared.Kernel.Entities;

namespace Addresses.Domain.Entities;

/// <summary>
/// Direccion de un punto fisico: es la tabla "Address" del microservicio (Addresses_Position).
/// Es el catastro que consume el microservicio de Customers: cada cliente creado guarda
/// el Id de esta fila en su campo LocationId.
/// </summary>
public class Position : BaseEntity
{
    public string Address { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    /// <summary>
    /// Cliente del microservicio de Customers que ya adopto esta direccion. Es opcional
    /// porque el catastro se carga antes que los clientes (datos de prueba).
    /// </summary>
    public int? CustomerId { get; set; }
}