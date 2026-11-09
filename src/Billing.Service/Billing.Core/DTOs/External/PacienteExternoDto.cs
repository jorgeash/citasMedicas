namespace Billing.Core.DTOs.External;

/// <summary>Datos del paciente que expone Scheduling. Billing solo los usa
/// para validar y guardar un snapshot en la factura.</summary>
public class PacienteExternoDto
{
    public int PacienteID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int? AseguradoraID { get; set; }
}
