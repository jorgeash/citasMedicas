using Addresses.Core.DTOs;
using Addresses.Core.Exceptions;
using Addresses.Core.Interfaces.Repositories;
using MediatR;

namespace Addresses.Core.Features.Positions;

public class DeletePositionCommand : IRequest<HttpResponse<string>>
{
    public int Id { get; set; }
}

public class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand, HttpResponse<string>>
{
    private readonly IPositionRepository _positionRepository;

    public DeletePositionCommandHandler(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<HttpResponse<string>> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await _positionRepository.GetByIdAsync(request.Id);

        if (position is null)
            throw new DomainException(Errores.POSICION_NO_ENCONTRADA,
                $"La direccion con Id {request.Id} no existe en el catalogo.");

        // Baja logica: el catalastro es referenciado por clientes, no se borra fisicamente.
        position.IsActive = false;
        await _positionRepository.UpdateAsync(position);

        return new HttpResponse<string>($"Direccion {request.Id} dada de baja correctamente.");
    }
}