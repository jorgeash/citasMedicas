using Addresses.Core.DTOs;
using Addresses.Core.Interfaces.Repositories;
using MediatR;

namespace Addresses.Core.Features.Positions;

public class GetPositionByIdQuery : IRequest<HttpResponse<PositionDto>?>
{
    public int Id { get; set; }

    public GetPositionByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, HttpResponse<PositionDto>?>
{
    private readonly IPositionRepository _positionRepository;

    public GetPositionByIdQueryHandler(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    /// <summary>
    /// Es el endpoint que consulta el microservicio de Customers con la libreria
    /// NugetPackage_Rest para validar el LocationId recibido al crear un cliente.
    /// Si la direccion no existe devuelve null y el controller responde 404.
    /// </summary>
    public async Task<HttpResponse<PositionDto>?> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
var position = await _positionRepository.GetOneByAsync(
            p => p.Id == request.Id,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: [p => p.City!, p => p.City!.Country!]);

        if (position is null)
            return null;

        return new HttpResponse<PositionDto>(new PositionDto
        {
            Id = position.Id,
            Address = position.Address,
            ZipCode = position.ZipCode,
            CityId = position.CityId,
            CityName = position.City?.Name ?? string.Empty,
            CountryId = position.City?.CountryId ?? 0,
            CountryName = position.City?.Country?.Name ?? string.Empty,
            Latitude = position.Latitude,
            Longitude = position.Longitude,
            CustomerId = position.CustomerId,
            IsActive = position.IsActive
        });
    }
}