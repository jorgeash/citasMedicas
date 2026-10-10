using Addresses.Core.DTOs;
using Addresses.Core.Exceptions;
using Addresses.Core.Interfaces.Repositories;
using MediatR;

namespace Addresses.Core.Features.Positions;

public class UpdatePositionCommand : IRequest<HttpResponse<string>>
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int CityId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? CustomerId { get; set; }
}

public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, HttpResponse<string>>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ICityRepository _cityRepository;

    public UpdatePositionCommandHandler(IPositionRepository positionRepository, ICityRepository cityRepository)
    {
        _positionRepository = positionRepository;
        _cityRepository = cityRepository;
    }

    public async Task<HttpResponse<string>> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var position = await _positionRepository.GetByIdAsync(request.Id);

        if (position is null)
            throw new DomainException(Errores.POSICION_NO_ENCONTRADA,
                $"La direccion con Id {request.Id} no existe en el catalogo.");

        var city = await _cityRepository.GetByIdAsync(request.CityId);
        if (city is null)
            throw new DomainException(Errores.CIUDAD_INVALIDA,
                $"La ciudad con Id {request.CityId} no existe en el catalogo de direcciones.");

        position.Address = request.Address?.Trim() ?? position.Address;
        position.ZipCode = request.ZipCode?.Trim() ?? position.ZipCode;
        position.CityId = request.CityId;
        position.Latitude = request.Latitude;
        position.Longitude = request.Longitude;
        position.CustomerId = request.CustomerId;

        await _positionRepository.UpdateAsync(position);

        return new HttpResponse<string>($"Direccion {request.Id} actualizada correctamente.");
    }
}