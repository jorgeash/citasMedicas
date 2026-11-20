namespace Clinical.Domain.Entities;

public class AtencionProcedimiento
{
    public long AtencionProcedimientoID { get; set; }
    public long AtencionID { get; set; }
    public int ProcedimientoID { get; set; }
    public DateTime FechaRealizacion { get; set; }
    public int Cantidad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
    public Procedimiento Procedimiento { get; set; } = null!;
}
