using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;

namespace Billing.Core.Features.Billing.Servicios.Queries.GetServicioById;

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
    private readonly IServicioRepository _repository;

    public GetServicioByIdQueryHandler(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<ServicioDto?> Handle(GetServicioByIdQuery request, CancellationToken cancellationToken)
    {
        var servicio = await _repository.GetByIdAsync(request.ServicioID);

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
