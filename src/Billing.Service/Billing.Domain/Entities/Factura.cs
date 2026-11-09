using Billing.Domain.Enums;

namespace Billing.Domain.Entities;

public class Factura
{
    public int FacturaID { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int PacienteID { get; set; }
    public string? PacienteNombre { get; set; }
    public int? CitaID { get; set; }
    public int? AseguradoraID { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public decimal Saldo { get; set; }
    public string Moneda { get; set; } = "USD";
    public EstadoFactura Estado { get; set; } = EstadoFactura.PENDIENTE;
    public string? Observaciones { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<FacturaDetalle> Detalle { get; set; } = new List<FacturaDetalle>();
}
