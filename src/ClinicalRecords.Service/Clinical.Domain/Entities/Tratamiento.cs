namespace Clinical.Domain.Entities;

public class Tratamiento
{
    public long TratamientoID { get; set; }
    public long AtencionID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Atencion Atencion { get; set; } = null!;
    public ICollection<TratamientoDetalle> Detalle { get; set; } = new List<TratamientoDetalle>();
}
