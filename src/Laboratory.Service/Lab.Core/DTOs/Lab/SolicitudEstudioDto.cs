namespace Lab.Core.DTOs.Lab;

public class SolicitudEstudioDto
{
    public int SolicitudEstudioID { get; set; }
    public string NumeroSolicitud { get; set; } = string.Empty;
    public long AtencionID { get; set; }
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<SolicitudEstudioDetalleDto> Detalle { get; set; } = new();
    public List<ResultadoEstudioDto> Resultados { get; set; } = new();
}
