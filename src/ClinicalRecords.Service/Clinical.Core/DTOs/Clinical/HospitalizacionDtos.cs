namespace Clinical.Core.DTOs.Clinical;

public class HabitacionDto
{
    public int HabitacionID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Piso { get; set; }
    public int? AreaID { get; set; }
    public bool Activa { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CamaDto
{
    public int CamaID { get; set; }
    public int HabitacionID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Activa { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HospitalizacionDto
{
    public long HospitalizacionID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public long AtencionID { get; set; }
    public int CamaID { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEgreso { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
