namespace Scheduling.Core.DTOs.Scheduling;

public class MedicoDto
{
    public int MedicoID { get; set; }
    public string CodigoMedico { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Nombre => $"{Nombres} {Apellidos}".Trim();
    public string? NumeroLicencia { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; }
}
