namespace Clinical.Domain.Entities;

public class AntecedenteMedico
{
    public int AntecedenteMedicoID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateOnly? Fecha { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExpedienteClinico ExpedienteClinico { get; set; } = null!;
}
