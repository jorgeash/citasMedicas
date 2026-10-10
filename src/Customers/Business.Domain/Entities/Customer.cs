using Shared.Kernel.Entities;

namespace Business.Domain.Entities;

/// <summary>
/// Cliente del sistema (tabla Business_Customer). Es la entidad central del microservicio:
/// su campo LocationId apunta a una fila del catalogo de direcciones del microservicio
/// de Addresses (Addresses_Position), por eso se resuelve por HTTP con la libreria REST.
/// </summary>
public class Customer : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public int CustomerTypeId { get; set; }
    public CustomerType CustomerType { get; set; } = null!;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    /// <summary>
    /// Referencia a la direccion del cliente (Id de Addresses_Position). No es una navegacion
    /// de EF Core porque la tabla vive en otra base de datos, en el microservicio de Addresses.
    /// </summary>
    public int LocationId { get; set; }
}