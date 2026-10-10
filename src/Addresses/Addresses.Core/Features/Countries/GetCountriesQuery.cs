using Addresses.Core.DTOs;
using Addresses.Core.Extensions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Addresses.Core.Features.Countries;

public class GetCountriesQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<CountryDto>>>>
{
}

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, HttpResponse<PagedDto<List<CountryDto>>>>
{
    private readonly ICountryRepository _countryRepository;

    public GetCountriesQueryHandler(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<HttpResponse<PagedDto<List<CountryDto>>>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _countryRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<Country>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedDto<List<CountryDto>>>(result.ToPagedDto(c => new CountryDto
        {
            Id = c.Id,
            Name = c.Name,
            IsActive = c.IsActive
        }));
    }
}