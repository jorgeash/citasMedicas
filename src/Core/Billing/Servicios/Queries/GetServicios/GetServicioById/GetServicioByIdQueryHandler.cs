using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Billing.Servicios.Queries.GetServicioById;

public class GetServicioByIdQueryHandler
    : IRequestHandler<GetServicioByIdQuery, Servicio?>
{
    private readonly IServicioRepository _repository;

    public GetServicioByIdQueryHandler(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Servicio?> Handle(
        GetServicioByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}