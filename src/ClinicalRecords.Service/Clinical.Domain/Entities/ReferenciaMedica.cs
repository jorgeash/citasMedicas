namespace Clinical.Domain.Entities;

public class ReferenciaMedica
{
    public long ReferenciaMedicaID { get; set; }
    public long AtencionID { get; set; }
    public int EspecialidadID { get; set; }
    public int? MedicoDestinoID { get; set; }
    public DateTime FechaReferencia { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
}
