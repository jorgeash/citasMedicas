namespace Business.Core.DTOs.Customers;

public class CustomerContactDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone1 { get; set; } = string.Empty;
    public string? Phone2 { get; set; }
    public int CustomerId { get; set; }
    public bool IsActive { get; set; }
}