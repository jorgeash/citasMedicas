namespace Business.Core.DTOs.Customers;

public class CompanyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sigla { get; set; } = string.Empty;
    public string MainEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}