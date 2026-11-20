namespace Clinical.Core.DTOs.Clinical;

public class ExpedienteDto
{
    public int ExpedienteClinicoID { get; set; }
    public long PacienteID { get; set; }
    public string NumeroExpediente { get; set; } = string.Empty;
    public DateOnly FechaApertura { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ExpedienteResumenDto
{
    public int ExpedienteClinicoID { get; set; }
    public long PacienteID { get; set; }
    public string NumeroExpediente { get; set; } = string.Empty;
    public DateOnly FechaApertura { get; set; }
    public bool Activo { get; set; }
}

public class ExpedienteDetalleDto : ExpedienteDto
{
    public List<AntecedenteMedicoDto> AntecedentesMedicos { get; set; } = new();
    public List<AntecedenteFamiliarDto> AntecedentesFamiliares { get; set; } = new();
    public List<AlergiaDto> Alergias { get; set; } = new();
    public List<HabitoDto> Habitos { get; set; } = new();
}

public class AntecedenteMedicoDto
{
    public int AntecedenteMedicoID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateOnly? Fecha { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AntecedenteFamiliarDto
{
    public int AntecedenteFamiliarID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public string Parentesco { get; set; } = string.Empty;
    public string Enfermedad { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AlergiaDto
{
    public int AlergiaID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public string Sustancia { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty;
    public string? Reaccion { get; set; }
    public DateOnly? FechaDeteccion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class HabitoDto
{
    public int HabitoID { get; set; }
    public int ExpedienteClinicoID { get; set; }
    public bool Fuma { get; set; }
    public bool ConsumeAlcohol { get; set; }
    public bool ActividadFisica { get; set; }
    public string? FrecuenciaActividad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}
