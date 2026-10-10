using Shared.Kernel.Entities;

namespace Addresses.Domain.Entities;

/// <summary>Pais del catastro de direcciones (tabla Addresses_Country).</summary>
public class Country : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<State> States { get; set; } = new List<State>();
    public ICollection<City> Cities { get; set; } = new List<City>();
}