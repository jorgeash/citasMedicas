using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;

namespace Billing.Core.Features.Billing.Servicios.Queries.GetServicios;

public class GetServiciosQuery : IRequest<IEnumerable<ServicioDto>>
{
}

public class GetServiciosQueryHandler : IRequestHandler<GetServiciosQuery, IEnumerable<ServicioDto>>
{
    private readonly IServicioRepository _repository;

    public GetServiciosQueryHandler(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ServicioDto>> Handle(GetServiciosQuery request, CancellationToken cancellationToken)
    {
        var servicios = await _repository.GetAllAsync();

        return servicios.Select(s => new ServicioDto
        {
            ServicioID = s.ServicioID,
            Codigo = s.Codigo,
            Nombre = s.Nombre,
            TipoServicio = s.TipoServicio,
            Descripcion = s.Descripcion,
            Activo = s.Activo
        });
    }
}
