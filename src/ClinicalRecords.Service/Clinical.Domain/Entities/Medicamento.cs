namespace Clinical.Domain.Entities;

public class Medicamento
{
    public int MedicamentoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Presentacion { get; set; }
    public string? Concentracion { get; set; }
    public string? ViaAdministracion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PrescripcionDetalle> PrescripcionDetalles { get; set; } = new List<PrescripcionDetalle>();
}
