namespace Lab.Core.DTOs.Lab;

public class ResultadoEstudioDto
{
    public int ResultadoEstudioID { get; set; }
    public int SolicitudEstudioID { get; set; }
    public int SolicitudEstudioDetalleID { get; set; }
    public string? NumeroSolicitud { get; set; }
    public long? PacienteID { get; set; }
    public DateTime FechaResultado { get; set; }
    public string Valor { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public string? RangoReferencia { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Interpretacion { get; set; }
    public string? ArchivoUrl { get; set; }
    public int? ValidadoPorMedicoID { get; set; }
    public DateTime? FechaValidacion { get; set; }
    public DateTime CreatedAt { get; set; }
}
