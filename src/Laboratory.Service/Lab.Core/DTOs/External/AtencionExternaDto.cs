namespace Lab.Core.DTOs.External;

public class AtencionExternaDto
{
    public long AtencionID { get; set; }
    public long PacienteID { get; set; }
    public DateTime FechaAtencion { get; set; }
    public string Estado { get; set; } = string.Empty;
}
