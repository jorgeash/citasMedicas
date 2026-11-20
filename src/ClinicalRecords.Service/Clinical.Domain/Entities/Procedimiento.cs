namespace Clinical.Domain.Entities;

public class Procedimiento
{
    public int ProcedimientoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AtencionProcedimiento> AtencionProcedimientos { get; set; } = new List<AtencionProcedimiento>();
}
