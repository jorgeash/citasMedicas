namespace Clinical.Domain.Entities;

public class ExpedienteClinico
{
    public int ExpedienteClinicoID { get; set; }
    public long PacienteID { get; set; }
    public string NumeroExpediente { get; set; } = string.Empty;
    public DateOnly FechaApertura { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<AntecedenteMedico> AntecedentesMedicos { get; set; } = new List<AntecedenteMedico>();
    public ICollection<AntecedenteFamiliar> AntecedentesFamiliares { get; set; } = new List<AntecedenteFamiliar>();
    public ICollection<Alergia> Alergias { get; set; } = new List<Alergia>();
    public ICollection<Habito> Habitos { get; set; } = new List<Habito>();
    public ICollection<Atencion> Atenciones { get; set; } = new List<Atencion>();
    public ICollection<ArchivoClinico> ArchivosClinicos { get; set; } = new List<ArchivoClinico>();
    public ICollection<Hospitalizacion> Hospitalizaciones { get; set; } = new List<Hospitalizacion>();
}
