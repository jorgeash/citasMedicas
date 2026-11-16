namespace Scheduling.Core.DTOs.Scheduling;

public class CitaDto
{
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public string? PacienteNombre { get; set; }
    public int MedicoID { get; set; }
    public string? MedicoNombre { get; set; }
    public int? EspecialidadID { get; set; }
    public int? ConsultorioID { get; set; }
    public int EstadoCitaID { get; set; }
    public string? EstadoCita { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public DateTime Fecha => FechaHoraInicio;
    public string? Motivo { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; }
}
