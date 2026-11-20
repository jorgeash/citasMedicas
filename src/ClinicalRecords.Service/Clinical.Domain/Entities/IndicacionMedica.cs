namespace Clinical.Domain.Entities;

public class IndicacionMedica
{
    public long IndicacionMedicaID { get; set; }
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
}
