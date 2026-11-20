namespace Clinical.Domain.Entities;

public class Habito
{
    public int HabitoID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public bool Fuma { get; set; }
    public bool ConsumeAlcohol { get; set; }
    public bool ActividadFisica { get; set; }
    public string? FrecuenciaActividad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExpedienteClinico ExpedienteClinico { get; set; } = null!;
}
