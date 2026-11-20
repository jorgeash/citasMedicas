namespace Lab.Core.DTOs.Lab;

public class EstudioDto
{
    public int EstudioID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool RequiereAyuno { get; set; }
    public bool Activo { get; set; }
    public decimal? Precio { get; set; }
    public DateTime CreatedAt { get; set; }
}
