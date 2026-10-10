namespace Business.Core.DTOs.Customers;

public class CustomerTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}