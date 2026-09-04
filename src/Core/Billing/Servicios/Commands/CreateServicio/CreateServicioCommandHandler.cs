using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Billing.Servicios.Commands.CreateServicio;

public class CreateServicioCommandHandler
    : IRequestHandler<CreateServicioCommand, Servicio>
{
    private readonly IServicioRepository _repository;

    public CreateServicioCommandHandler(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Servicio> Handle(
        CreateServicioCommand request,
        CancellationToken cancellationToken)
    {
        var servicio = new Servicio
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            TipoServicio = request.TipoServicio,
            Descripcion = request.Descripcion,
            Activo = request.Activo
        };

        return await _repository.CreateAsync(servicio, cancellationToken);
    }
}