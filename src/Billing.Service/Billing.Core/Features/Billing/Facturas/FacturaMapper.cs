using Billing.Core.DTOs.Billing;
using Billing.Domain.Entities;

namespace Billing.Core.Features.Billing.Facturas;

internal static class FacturaMapper
{
    public static FacturaDto ToDto(Factura factura)
    {
        return new FacturaDto
        {
            FacturaID = factura.FacturaID,
            Numero = factura.Numero,
            PacienteID = factura.PacienteID,
            PacienteNombre = factura.PacienteNombre,
            CitaID = factura.CitaID,
            AseguradoraID = factura.AseguradoraID,
            FechaEmision = factura.FechaEmision,
            Subtotal = factura.Subtotal,
            Impuesto = factura.Impuesto,
            Total = factura.Total,
            Saldo = factura.Saldo,
            Moneda = factura.Moneda,
            Estado = factura.Estado.ToString(),
            Observaciones = factura.Observaciones,
            CreatedAt = factura.CreatedAt,
            Detalle = factura.Detalle.Select(ToDetalleDto).ToList()
        };
    }

    public static FacturaResumenDto ToResumenDto(Factura factura)
    {
        return new FacturaResumenDto
        {
            FacturaID = factura.FacturaID,
            Numero = factura.Numero,
            PacienteID = factura.PacienteID,
            PacienteNombre = factura.PacienteNombre,
            CitaID = factura.CitaID,
            FechaEmision = factura.FechaEmision,
            Total = factura.Total,
            Saldo = factura.Saldo,
            Moneda = factura.Moneda,
            Estado = factura.Estado.ToString()
        };
    }

    private static FacturaDetalleDto ToDetalleDto(FacturaDetalle detalle)
    {
        return new FacturaDetalleDto
        {
            FacturaDetalleID = detalle.FacturaDetalleID,
            ServicioID = detalle.ServicioID,
            TarifaID = detalle.TarifaID,
            Descripcion = detalle.Descripcion,
            Cantidad = detalle.Cantidad,
            PrecioUnitario = detalle.PrecioUnitario,
            Subtotal = detalle.Subtotal
        };
    }
}
