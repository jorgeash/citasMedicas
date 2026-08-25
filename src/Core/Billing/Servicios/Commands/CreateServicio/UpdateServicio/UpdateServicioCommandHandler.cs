using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Billing.Servicios.Commands.UpdateServicio;

public class UpdateServicioCommandHandler
    : IRequestHandler<UpdateServicioCommand, Servicio?>
{
    private readonly IServicioRepository _repository;

    public UpdateServicioCommandHandler(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Servicio?> Handle(
        UpdateServicioCommand request,
        CancellationToken cancellationToken)
    {
        var servicio = await _repository.GetByIdAsync(request.ServicioID);

        if (servicio is null)
        {
            return null;
        }

        servicio.Codigo = request.Codigo;
        servicio.Nombre = request.Nombre;
        servicio.TipoServicio = request.TipoServicio;
        servicio.Descripcion = request.Descripcion;
        servicio.Activo = request.Activo;

        return await _repository.UpdateAsync(servicio);
    }
}