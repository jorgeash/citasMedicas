namespace Clinical.Domain.Entities;

public class TratamientoDetalle
{
    public long TratamientoDetalleID { get; set; }
    public long TratamientoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Frecuencia { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tratamiento Tratamiento { get; set; } = null!;
}
