using Addresses.Core.DTOs;
using Addresses.Core.Interfaces.Repositories;
using MediatR;

namespace Addresses.Core.Features.Cities;

public class GetCityByIdQuery : IRequest<HttpResponse<CityDto>?>
{
    public int Id { get; set; }

    public GetCityByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, HttpResponse<CityDto>?>
{
    private readonly ICityRepository _cityRepository;

    public GetCityByIdQueryHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<HttpResponse<CityDto>?> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetOneByAsync(
            c => c.Id == request.Id,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: [c => c.Country]);

        if (city is null)
            return null;

        return new HttpResponse<CityDto>(new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            CountryId = city.CountryId,
            CountryName = city.Country?.Name ?? string.Empty,
            IsActive = city.IsActive
        });
    }
}