namespace Lab.Domain.Entities;

public class SolicitudEstudio
{
    public int SolicitudEstudioID { get; set; }
    public string NumeroSolicitud { get; set; } = string.Empty;
    public long AtencionID { get; set; }
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "PENDIENTE";
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<SolicitudEstudioDetalle> Detalle { get; set; } = new List<SolicitudEstudioDetalle>();
    public ICollection<ResultadoEstudio> Resultados { get; set; } = new List<ResultadoEstudio>();
}
