using Billing.Core.DTOs.Billing;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Enums;
using MediatR;

namespace Billing.Core.Features.Billing.Facturas.Commands.AnularFactura;

public class AnularFacturaCommand : IRequest<FacturaDto>
{
    public int FacturaID { get; set; }
    public string? Motivo { get; set; }
}

public class AnularFacturaCommandHandler : IRequestHandler<AnularFacturaCommand, FacturaDto>
{
    private readonly IFacturaRepository _repository;

    public AnularFacturaCommandHandler(IFacturaRepository repository)
    {
        _repository = repository;
    }

    public async Task<FacturaDto> Handle(AnularFacturaCommand request, CancellationToken cancellationToken)
    {
        var factura = await _repository.GetOneByAsync(
            f => f.FacturaID == request.FacturaID,
            false,
            false,
            cancellationToken,
            f => f.Detalle);

        if (factura is null)
            throw new DomainException(Errores.FACTURA_NO_ENCONTRADA, $"No existe la factura {request.FacturaID}.");

        if (factura.Estado == EstadoFactura.PAGADA)
            throw new DomainException(Errores.FACTURA_YA_PAGADA, "No se puede anular una factura que ya fue pagada.");

        if (factura.Estado == EstadoFactura.ANULADA)
            throw new DomainException(Errores.FACTURA_YA_ANULADA, "La factura ya está anulada.");

        factura.Estado = EstadoFactura.ANULADA;
        factura.Saldo = 0m;
        factura.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Motivo))
        {
            var motivo = $"ANULADA: {request.Motivo.Trim()}";
            var observaciones = string.IsNullOrWhiteSpace(factura.Observaciones)
                ? motivo
                : $"{factura.Observaciones} | {motivo}";

            factura.Observaciones = observaciones.Length > 500
                ? observaciones[..500]
                : observaciones;
        }

        await _repository.UpdateAsync(factura);

        return FacturaMapper.ToDto(factura);
    }
}
