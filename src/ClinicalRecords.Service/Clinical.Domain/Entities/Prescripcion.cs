namespace Clinical.Domain.Entities;

public class Prescripcion
{
    public long PrescripcionID { get; set; }
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaPrescripcion { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
    public ICollection<PrescripcionDetalle> Detalle { get; set; } = new List<PrescripcionDetalle>();
}
