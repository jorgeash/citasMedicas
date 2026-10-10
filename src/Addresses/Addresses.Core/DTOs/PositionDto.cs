namespace Addresses.Core.DTOs;

/// <summary>
/// Direccion (posicion) del catastro. Este es el contrato que el microservicio de Customers
/// consume con la libreria NugetPackage_Rest: el Id de esta fila es el LocationId del cliente.
/// </summary>
public class PositionDto
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? CustomerId { get; set; }
    public bool IsActive { get; set; }
}