namespace Clinical.Domain.Entities;

public class AtencionDiagnostico
{
    public long AtencionDiagnosticoID { get; set; }
    public long AtencionID { get; set; }
    public int DiagnosticoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
    public Diagnostico Diagnostico { get; set; } = null!;
}
