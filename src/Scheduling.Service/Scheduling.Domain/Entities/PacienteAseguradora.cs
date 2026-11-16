namespace Scheduling.Domain.Entities;

public class PacienteAseguradora
{
    public long PacienteAseguradoraID { get; set; }
    public long PacienteID { get; set; }
    public int AseguradoraID { get; set; }
    public string NumeroPoliza { get; set; } = string.Empty;
    public string? NumeroAfiliacion { get; set; }
    public string? TitularNombre { get; set; }
    public string? ParentescoTitular { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public decimal? CoberturaPorcentaje { get; set; }
    public bool Activo { get; set; }

    public Paciente? Paciente { get; set; }
    public Aseguradora? Aseguradora { get; set; }
}
