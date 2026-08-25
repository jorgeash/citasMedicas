namespace Domain.Billing.Entities;

public class Paciente
{
    public long PacienteID { get; set; }

    public string CodigoPaciente { get; set; } = string.Empty;

    public string TipoDocumento { get; set; } = string.Empty;

    public string NumeroDocumento { get; set; } = string.Empty;

    public string Nombres { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }

    public string Sexo { get; set; } = string.Empty;

    public string? EstadoCivil { get; set; }

    public string? Telefono { get; set; }

    public string? TelefonoSecundario { get; set; }

    public string? Email { get; set; }

    public string? Direccion { get; set; }

    public string? Ciudad { get; set; }

    public string? Pais { get; set; }

    public string? Ocupacion { get; set; }

    public string? TipoSangre { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }
}