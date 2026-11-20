namespace Clinical.Domain.Entities;

public class Hospitalizacion
{
    public long HospitalizacionID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public long AtencionID { get; set; }
    public int CamaID { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEgreso { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ExpedienteClinico ExpedienteClinico { get; set; } = null!;
    public Atencion Atencion { get; set; } = null!;
    public Cama Cama { get; set; } = null!;
}
