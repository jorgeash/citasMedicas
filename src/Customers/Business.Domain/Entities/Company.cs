using Shared.Kernel.Entities;

namespace Business.Domain.Entities;

/// <summary>Empresa a la que pertenece un cliente (tabla Business_Company).</summary>
public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string MainEmail { get; set; } = string.Empty;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}