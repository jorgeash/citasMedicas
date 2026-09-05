using Core.DTOs.Billing;
using Domain.Billing.Entities;
using MediatR;
using Domain.Interfaces;

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
    private readonly IRepository<Servicio> _repository;

    public GetServicioByIdQueryHandler(IRepository<Servicio> repository)
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
