namespace Clinical.Domain.Entities;

public class Alergia
{
    public int AlergiaID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public string Sustancia { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty;
    public string? Reaccion { get; set; }
    public DateOnly? FechaDeteccion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExpedienteClinico ExpedienteClinico { get; set; } = null!;
}
