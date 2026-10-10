using Shared.Kernel.Entities;

namespace Business.Domain.Entities;

/// <summary>Persona de contacto de un cliente (tabla Business_CustomerContact).</summary>
public class CustomerContact : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone1 { get; set; } = string.Empty;
    public string? Phone2 { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}