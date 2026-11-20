namespace Lab.Domain.Entities;

public class Estudio
{
    public int EstudioID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool RequiereAyuno { get; set; }
    public bool Activo { get; set; } = true;
    public decimal? Precio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
