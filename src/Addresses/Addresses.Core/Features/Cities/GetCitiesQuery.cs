using Addresses.Core.DTOs;
using Addresses.Core.Extensions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Addresses.Core.Features.Cities;

public class GetCitiesQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<CityDto>>>>
{
}

public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, HttpResponse<PagedDto<List<CityDto>>>>
{
    private readonly ICityRepository _cityRepository;

    public GetCitiesQueryHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<HttpResponse<PagedDto<List<CityDto>>>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var result = await _cityRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<City>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken,
            includes: [c => c.Country]);

        return new HttpResponse<PagedDto<List<CityDto>>>(result.ToPagedDto(c => new CityDto
        {
            Id = c.Id,
            Name = c.Name,
            CountryId = c.CountryId,
            CountryName = c.Country?.Name ?? string.Empty,
            IsActive = c.IsActive
        }));
    }
}