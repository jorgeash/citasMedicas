namespace Lab.Core.DTOs.Lab;

public class SolicitudEstudioDetalleDto
{
    public int SolicitudEstudioDetalleID { get; set; }
    public int SolicitudEstudioID { get; set; }
    public int EstudioID { get; set; }
    public string? CodigoEstudio { get; set; }
    public string? NombreEstudio { get; set; }
    public string Prioridad { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}
