using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using MediatR;

namespace Billing.Core.Features.Billing.Facturas.Queries.GetFacturaById;

public class GetFacturaByIdQuery : IRequest<FacturaDto?>
{
    public int FacturaID { get; set; }

    public GetFacturaByIdQuery(int facturaId)
    {
        FacturaID = facturaId;
    }
}

public class GetFacturaByIdQueryHandler : IRequestHandler<GetFacturaByIdQuery, FacturaDto?>
{
    private readonly IFacturaRepository _repository;

    public GetFacturaByIdQueryHandler(IFacturaRepository repository)
    {
        _repository = repository;
    }

    public async Task<FacturaDto?> Handle(GetFacturaByIdQuery request, CancellationToken cancellationToken)
    {
        var factura = await _repository.GetOneByAsync(
            f => f.FacturaID == request.FacturaID,
            true,
            false,
            cancellationToken,
            f => f.Detalle);

        return factura is null ? null : FacturaMapper.ToDto(factura);
    }
}
