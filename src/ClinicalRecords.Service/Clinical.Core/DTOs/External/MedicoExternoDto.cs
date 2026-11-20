namespace Clinical.Core.DTOs.External;

public class MedicoExternoDto
{
    public int MedicoID { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
