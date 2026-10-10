using Shared.Kernel.Entities;

namespace Addresses.Domain.Entities;

/// <summary>
/// Ciudad del catastro (tabla Addresses_City). En el microservicio original la ciudad
/// referenciaba al pais y no al estado, por lo que se conserva esa relacion.
/// </summary>
public class City : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public ICollection<Position> Locations { get; set; } = new List<Position>();
}