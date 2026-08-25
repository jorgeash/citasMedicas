using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Billing.Servicios.Queries.GetServicios;

public class GetServiciosQueryHandler
    : IRequestHandler<GetServiciosQuery, IEnumerable<Servicio>>
{
    private readonly IServicioRepository _repository;

    public GetServiciosQueryHandler(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Servicio>> Handle(
        GetServiciosQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync();
    }
}