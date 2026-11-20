namespace Clinical.Domain.Entities;

public class NotaMedica
{
    public long NotaMedicaID { get; set; }
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaNota { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
}
