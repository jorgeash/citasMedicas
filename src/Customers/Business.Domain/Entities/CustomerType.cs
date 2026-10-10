using Shared.Kernel.Entities;

namespace Business.Domain.Entities;

/// <summary>Tipo de cliente: empresa, persona natural o aseguradora (tabla Business_CustomerType).</summary>
public class CustomerType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}