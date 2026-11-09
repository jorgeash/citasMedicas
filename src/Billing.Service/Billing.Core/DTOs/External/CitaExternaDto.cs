namespace Billing.Core.DTOs.External;

/// <summary>Datos mínimos de la cita que expone Scheduling.</summary>
public class CitaExternaDto
{
    public int CitaID { get; set; }
    public int PacienteID { get; set; }
    public DateTime Fecha { get; set; }
}
