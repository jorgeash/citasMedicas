namespace Business.Core.DTOs.Customers;

/// <summary>
/// Proyeccion del cliente hacia el exterior. LocationId es la referencia al catalastro de
/// direcciones del microservicio de Addresses; Location trae el detalle de esa direccion,
/// obtenido por HTTP con la libreria NugetPackage_Rest.
/// </summary>
public class CustomerDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CustomerTypeId { get; set; }
    public string CustomerTypeName { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public LocationSummaryDto? Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Resumen de la direccion asociada, para no duplicar el contrato completo.</summary>
public class LocationSummaryDto
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}