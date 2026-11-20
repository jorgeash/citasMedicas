namespace Clinical.Core.DTOs.Clinical;

public class AtencionDto
{
    public long AtencionID { get; set; }
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public int? ExpedienteClinicoID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaAtencion { get; set; }
    public string? Motivo { get; set; }
    public string? Antecedente { get; set; }
    public string? ExamenFisico { get; set; }
    public string? Conclusion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AtencionResumenDto
{
    public long AtencionID { get; set; }
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaAtencion { get; set; }
    public string? Motivo { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class AtencionDetalleDto : AtencionDto
{
    public List<SignosVitalesDto> SignosVitales { get; set; } = new();
    public List<NotaMedicaDto> NotasMedicas { get; set; } = new();
    public List<AtencionDiagnosticoDto> Diagnosticos { get; set; } = new();
    public List<AtencionSintomaDto> Sintomas { get; set; } = new();
    public List<PrescripcionDto> Prescripciones { get; set; } = new();
    public List<TratamientoDto> Tratamientos { get; set; } = new();
    public List<IndicacionMedicaDto> IndicacionesMedicas { get; set; } = new();
    public List<ReferenciaMedicaDto> ReferenciasMedicas { get; set; } = new();
    public List<AtencionProcedimientoDto> Procedimientos { get; set; } = new();
    public List<ArchivoClinicoDto> ArchivosClinicos { get; set; } = new();
}

public class SignosVitalesDto
{
    public int SignosVitalesID { get; set; }
    public long AtencionID { get; set; }
    public DateTime FechaMedicion { get; set; }
    public decimal? Temperatura { get; set; }
    public int? FrecuenciaCardiaca { get; set; }
    public int? FrecuenciaRespiratoria { get; set; }
    public int? PresionSistolica { get; set; }
    public int? PresionDiastolica { get; set; }
    public decimal? SaturacionOxigeno { get; set; }
    public decimal? PesoKg { get; set; }
    public decimal? TallaCm { get; set; }
    public decimal? IMC { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotaMedicaDto
{
    public long NotaMedicaID { get; set; }
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaNota { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AtencionDiagnosticoDto
{
    public long AtencionDiagnosticoID { get; set; }
    public long AtencionID { get; set; }
    public int DiagnosticoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AtencionSintomaDto
{
    public long AtencionSintomaID { get; set; }
    public long AtencionID { get; set; }
    public int SintomaID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty;
    public int? DuracionDias { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class IndicacionMedicaDto
{
    public long IndicacionMedicaID { get; set; }
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activa { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReferenciaMedicaDto
{
    public long ReferenciaMedicaID { get; set; }
    public long AtencionID { get; set; }
    public int EspecialidadID { get; set; }
    public int? MedicoDestinoID { get; set; }
    public DateTime FechaReferencia { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AtencionProcedimientoDto
{
    public long AtencionProcedimientoID { get; set; }
    public long AtencionID { get; set; }
    public int ProcedimientoID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public int Cantidad { get; set; }
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ArchivoClinicoDto
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
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}
