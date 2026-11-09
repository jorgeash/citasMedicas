namespace Billing.Core.DTOs.Billing;

public class FacturaDto
{
    public int FacturaID { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int PacienteID { get; set; }
    public string? PacienteNombre { get; set; }
    public int? CitaID { get; set; }
    public int? AseguradoraID { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public decimal Saldo { get; set; }
    public string Moneda { get; set; } = "USD";
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<FacturaDetalleDto> Detalle { get; set; } = new();
}
