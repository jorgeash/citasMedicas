namespace Billing.Core.DTOs.Billing;

/// <summary>Versión liviana de la factura para la bandeja del frontend
/// (no trae el detalle).</summary>
public class FacturaResumenDto
{
    public int FacturaID { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int PacienteID { get; set; }
    public string? PacienteNombre { get; set; }
    public int? CitaID { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal Total { get; set; }
    public decimal Saldo { get; set; }
    public string Moneda { get; set; } = "USD";
    public string Estado { get; set; } = string.Empty;
}
