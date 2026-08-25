using Core.DTOs.Billing;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Features.Billing.Servicios.Queries.GetServicioById;

public class GetServicioByIdQuery : IRequest<ServicioDto?>
{
    public int ServicioID { get; set; }

    public GetServicioByIdQuery(int servicioID)
    {
        ServicioID = servicioID;
    }
}

public class GetServicioByIdQueryHandler : IRequestHandler<GetServicioByIdQuery, ServicioDto?>
{
    private readonly IServicioRepository _servicioRepository;

    public GetServicioByIdQueryHandler(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
    }

    public async Task<ServicioDto?> Handle(GetServicioByIdQuery request, CancellationToken cancellationToken)
    {
        var servicio = await _servicioRepository.GetByIdAsync(request.ServicioID);

        if (servicio is null)
            return null;

        return new ServicioDto
        {
            ServicioID = servicio.ServicioID,
            Codigo = servicio.Codigo,
            Nombre = servicio.Nombre,
            TipoServicio = servicio.TipoServicio,
            Descripcion = servicio.Descripcion,
            Activo = servicio.Activo
        };
    }
}
