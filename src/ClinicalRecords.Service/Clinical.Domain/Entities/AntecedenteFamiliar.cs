namespace Clinical.Domain.Entities;

public class AntecedenteFamiliar
{
    public int AntecedenteFamiliarID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public string Parentesco { get; set; } = string.Empty;
    public string Enfermedad { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExpedienteClinico ExpedienteClinico { get; set; } = null!;
}
