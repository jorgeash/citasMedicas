namespace Clinical.Domain.Entities;

public class AtencionSintoma
{
    public long AtencionSintomaID { get; set; }
    public long AtencionID { get; set; }
    public int SintomaID { get; set; }
    public string Severidad { get; set; } = string.Empty;
    public int? DuracionDias { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
    public Sintoma Sintoma { get; set; } = null!;
}
