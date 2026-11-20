namespace Clinical.Core.DTOs.Clinical;

public class PrescripcionDetalleDto
{
    public long PrescripcionDetalleID { get; set; }
    public long PrescripcionID { get; set; }
    public int MedicamentoID { get; set; }
    public string NombreMedicamento { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public string Via { get; set; } = string.Empty;
    public int? DuracionDias { get; set; }
    public int? Cantidad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PrescripcionDto
{
    public long PrescripcionID { get; set; }
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaPrescripcion { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PrescripcionDetalleDto> Detalle { get; set; } = new();
}

public class TratamientoDetalleDto
{
    public long TratamientoDetalleID { get; set; }
    public long TratamientoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Frecuencia { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TratamientoDto
{
    public long TratamientoID { get; set; }
    public long AtencionID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TratamientoDetalleDto> Detalle { get; set; } = new();
}
