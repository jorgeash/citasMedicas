using Shared.Kernel.Entities;

namespace Addresses.Domain.Entities;

/// <summary>Estado o provincia dentro de un pais (tabla Addresses_State).</summary>
public class State : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
}