using Core.DTOs.Billing;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Features.Billing.Servicios.Queries.GetServicios;

public class GetServiciosQuery : IRequest<IEnumerable<ServicioDto>>
{
}

public class GetServiciosQueryHandler : IRequestHandler<GetServiciosQuery, IEnumerable<ServicioDto>>
{
    private readonly IServicioRepository _servicioRepository;

    public GetServiciosQueryHandler(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
    }

    public async Task<IEnumerable<ServicioDto>> Handle(GetServiciosQuery request, CancellationToken cancellationToken)
    {
        var servicios = await _servicioRepository.GetAllAsync();

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
