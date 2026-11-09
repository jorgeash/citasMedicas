namespace Billing.Core.DTOs.Billing;

public class FacturaDetalleDto
{
    public int FacturaDetalleID { get; set; }
    public int ServicioID { get; set; }
    public int TarifaID { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
