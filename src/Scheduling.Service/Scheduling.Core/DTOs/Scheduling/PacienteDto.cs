namespace Scheduling.Core.DTOs.Scheduling;

public class PacienteDto
{
    public long PacienteID { get; set; }
    public string CodigoPaciente { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Nombre => $"{Nombres} {Apellidos}".Trim();
    public DateOnly FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public int? AseguradoraID { get; set; }
    public bool Activo { get; set; }
}
