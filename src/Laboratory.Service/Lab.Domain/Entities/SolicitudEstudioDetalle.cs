namespace Lab.Domain.Entities;

public class SolicitudEstudioDetalle
{
    public int SolicitudEstudioDetalleID { get; set; }
    public int SolicitudEstudioID { get; set; }
    public int EstudioID { get; set; }
    public string Prioridad { get; set; } = "NORMAL";
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public SolicitudEstudio? SolicitudEstudio { get; set; }
    public Estudio? Estudio { get; set; }
}
