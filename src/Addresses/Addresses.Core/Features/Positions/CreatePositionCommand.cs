using Addresses.Core.DTOs;
using Addresses.Core.Exceptions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;

namespace Addresses.Core.Features.Positions;

public class CreatePositionCommand : IRequest<HttpResponse<PositionDto>>
{
    public string Address { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public int CityId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? CustomerId { get; set; }
}

public class CreatePositionCommandHandler : IRequestHandler<CreatePositionCommand, HttpResponse<PositionDto>>
{
    private readonly IPositionRepository _positionRepository;
    private readonly ICityRepository _cityRepository;

    public CreatePositionCommandHandler(IPositionRepository positionRepository, ICityRepository cityRepository)
    {
        _positionRepository = positionRepository;
        _cityRepository = cityRepository;
    }

    public async Task<HttpResponse<PositionDto>> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Address))
            throw new DomainException(Errores.DIRECCION_INVALIDA, "La direccion es obligatoria.");

        var city = await _cityRepository.GetOneByAsync(
            c => c.Id == request.CityId,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: [c => c.Country]);

        if (city is null)
            throw new DomainException(Errores.CIUDAD_INVALIDA,
                $"La ciudad con Id {request.CityId} no existe en el catalogo de direcciones.");

        var position = await _positionRepository.AddAsync(new Position
        {
            Address = request.Address.Trim(),
            ZipCode = request.ZipCode?.Trim() ?? string.Empty,
            CityId = request.CityId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CustomerId = request.CustomerId
        });

        return new HttpResponse<PositionDto>(new PositionDto
        {
            Id = position.Id,
            Address = position.Address,
            ZipCode = position.ZipCode,
            CityId = position.CityId,
            CityName = city.Name,
            CountryId = city.CountryId,
            CountryName = city.Country?.Name ?? string.Empty,
            Latitude = position.Latitude,
            Longitude = position.Longitude,
            CustomerId = position.CustomerId,
            IsActive = position.IsActive
        });
    }
}