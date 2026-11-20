namespace Clinical.Domain.Entities;

public class Sintoma
{
    public int SintomaID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AtencionSintoma> AtencionSintomas { get; set; } = new List<AtencionSintoma>();
}
