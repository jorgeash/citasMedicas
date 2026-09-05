using Core.DTOs.Billing;
using Domain.Billing.Entities;
using MediatR;
using Domain.Interfaces;

namespace Core.Features.Billing.Servicios.Queries.GetServicios;

public class GetServiciosQuery : IRequest<IEnumerable<ServicioDto>>
{
}

public class GetServiciosQueryHandler : IRequestHandler<GetServiciosQuery, IEnumerable<ServicioDto>>
{
    private readonly IRepository<Servicio> _repository;

    public GetServiciosQueryHandler(IRepository<Servicio> repository)
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
