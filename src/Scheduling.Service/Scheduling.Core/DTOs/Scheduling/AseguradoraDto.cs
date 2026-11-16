namespace Scheduling.Core.DTOs.Scheduling;

public class AseguradoraDto
{
    public int AseguradoraID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? NumeroFiscal { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; }
}
