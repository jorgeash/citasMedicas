namespace Clinical.Core.DTOs.External;

public class CitaExternaDto
{
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public DateTime Fecha { get; set; }
    public string EstadoCodigo { get; set; } = string.Empty;
}
