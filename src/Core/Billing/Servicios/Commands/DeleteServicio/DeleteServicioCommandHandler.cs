using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Billing.Servicios.Commands.DeleteServicio;

public class DeleteServicioCommandHandler
    : IRequestHandler<DeleteServicioCommand, bool>
{
    private readonly IServicioRepository _repository;

    public DeleteServicioCommandHandler(IServicioRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteServicioCommand request,
        CancellationToken cancellationToken)
    {
        var servicio = await _repository.GetByIdAsync(request.Id);

        if (servicio is null)
        {
            return false;
        }

        return await _repository.DeleteAsync(servicio);
    }
}