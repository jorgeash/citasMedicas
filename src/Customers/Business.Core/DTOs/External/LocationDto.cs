namespace Business.Core.DTOs.External;

/// <summary>
/// Direccion tal como la expone el microservicio de Addresses (tabla Addresses_Position).
/// Es el dato que este microservicio obtiene con la libreria NugetPackage_Rest.
/// </summary>
public class LocationDto
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