namespace Billing.Domain.Entities;

public class FacturaDetalle
{
    public int FacturaDetalleID { get; set; }
    public int FacturaID { get; set; }
    public int ServicioID { get; set; }
    public int TarifaID { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public Factura? Factura { get; set; }
    public Servicio? Servicio { get; set; }
    public Tarifa? Tarifa { get; set; }
}
