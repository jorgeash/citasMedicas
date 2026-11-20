namespace Lab.Domain.Entities;

public class ResultadoEstudio
{
    public int ResultadoEstudioID { get; set; }
    public int SolicitudEstudioID { get; set; }
    public int SolicitudEstudioDetalleID { get; set; }
    public DateTime FechaResultado { get; set; } = DateTime.UtcNow;
    public string Valor { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public string? RangoReferencia { get; set; }
    public string Estado { get; set; } = "NORMAL";
    public string? Interpretacion { get; set; }
    public string? ArchivoUrl { get; set; }
    public int? ValidadoPorMedicoID { get; set; }
    public DateTime? FechaValidacion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public SolicitudEstudio? SolicitudEstudio { get; set; }
    public SolicitudEstudioDetalle? SolicitudEstudioDetalle { get; set; }
}
