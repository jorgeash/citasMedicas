namespace Clinical.Domain.Entities;

public class PrescripcionDetalle
{
    public long PrescripcionDetalleID { get; set; }
    public long PrescripcionID { get; set; }
    public int MedicamentoID { get; set; }
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public string Via { get; set; } = string.Empty;
    public int? DuracionDias { get; set; }
    public int? Cantidad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Prescripcion Prescripcion { get; set; } = null!;
    public Medicamento Medicamento { get; set; } = null!;
}
