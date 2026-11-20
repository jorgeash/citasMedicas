namespace Clinical.Core.DTOs.Clinical;

public class DiagnosticoDto
{
    public int DiagnosticoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SintomaDto
{
    public int SintomaID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MedicamentoDto
{
    public int MedicamentoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Presentacion { get; set; }
    public string? Concentracion { get; set; }
    public string? ViaAdministracion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProcedimientoDto
{
    public int ProcedimientoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}
