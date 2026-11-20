namespace Clinical.Domain.Entities;

public class Atencion
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ExpedienteClinico? ExpedienteClinico { get; set; }
    public ICollection<SignosVitales> SignosVitales { get; set; } = new List<SignosVitales>();
    public ICollection<NotaMedica> NotasMedicas { get; set; } = new List<NotaMedica>();
    public ICollection<AtencionDiagnostico> Diagnosticos { get; set; } = new List<AtencionDiagnostico>();
    public ICollection<AtencionSintoma> Sintomas { get; set; } = new List<AtencionSintoma>();
    public ICollection<Prescripcion> Prescripciones { get; set; } = new List<Prescripcion>();
    public ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();
    public ICollection<IndicacionMedica> IndicacionesMedicas { get; set; } = new List<IndicacionMedica>();
    public ICollection<ReferenciaMedica> ReferenciasMedicas { get; set; } = new List<ReferenciaMedica>();
    public ICollection<AtencionProcedimiento> Procedimientos { get; set; } = new List<AtencionProcedimiento>();
    public ICollection<ArchivoClinico> ArchivosClinicos { get; set; } = new List<ArchivoClinico>();
    public ICollection<Hospitalizacion> Hospitalizaciones { get; set; } = new List<Hospitalizacion>();
}
