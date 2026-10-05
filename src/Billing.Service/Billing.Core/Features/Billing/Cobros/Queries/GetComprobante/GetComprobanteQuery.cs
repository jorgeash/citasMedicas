using Billing.Core.Exceptions;
using Billing.Core.Interfaces.Services;
using MediatR;

namespace Billing.Core.Features.Billing.Cobros.Queries.GetComprobante;

/// <summary>
/// Descarga el comprobante en PDF de una factura desde el microservicio de Reportes.
/// </summary>
public class GetComprobanteQuery : IRequest<byte[]>
{
    public int FacturaId { get; set; }

    public GetComprobanteQuery(int facturaId)
    {
        FacturaId = facturaId;
    }
}

public class GetComprobanteQueryHandler : IRequestHandler<GetComprobanteQuery, byte[]>
{
    private readonly IReporteService _reporteService;

    public GetComprobanteQueryHandler(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    public async Task<byte[]> Handle(GetComprobanteQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var comprobante = await _reporteService.DescargarComprobanteAsync(request.FacturaId, cancellationToken);

            if (comprobante.Length == 0)
                throw new DomainException(Errores.REPORTE_NO_GENERADO, $"El comprobante de la factura {request.FacturaId} llegó vacío.");

            return comprobante;
        }
        catch (Exception ex) when (ex is not DomainException)
        {
            throw new DomainException(Errores.REPORTE_NO_GENERADO,
                $"No fue posible generar el comprobante de la factura {request.FacturaId}: {ex.Message}");
        }
    }
}