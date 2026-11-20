namespace Clinical.Domain.Entities;

public class ArchivoClinico
{
    public long ArchivoClinicoID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public long? AtencionID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? TamanoBytes { get; set; }
    public DateTime FechaCarga { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ExpedienteClinico ExpedienteClinico { get; set; } = null!;
    public Atencion? Atencion { get; set; }
}
