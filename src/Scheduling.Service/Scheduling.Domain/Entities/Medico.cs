namespace Scheduling.Domain.Entities;

public class Medico
{
    public int MedicoID { get; set; }
    public int? UsuarioID { get; set; }
    public string CodigoMedico { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? NumeroLicencia { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; }
}
