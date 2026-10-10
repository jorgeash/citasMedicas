using Addresses.Core.DTOs;
using Addresses.Core.Interfaces.Repositories;
using MediatR;

namespace Addresses.Core.Features.Countries;

public class GetCountryByIdQuery : IRequest<HttpResponse<CountryDto>?>
{
    public int Id { get; set; }

    public GetCountryByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, HttpResponse<CountryDto>?>
{
    private readonly ICountryRepository _countryRepository;

    public GetCountryByIdQueryHandler(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<HttpResponse<CountryDto>?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdAsync(request.Id);

        if (country is null)
            return null;

        return new HttpResponse<CountryDto>(new CountryDto
        {
            Id = country.Id,
            Name = country.Name,
            IsActive = country.IsActive
        });
    }
}