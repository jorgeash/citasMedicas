using Addresses.Core.DTOs;
using Addresses.Core.Exceptions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;

namespace Addresses.Core.Features.Cities;

public class CreateCityCommand : IRequest<HttpResponse<CityDto>>
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}

public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, HttpResponse<CityDto>>
{
    private readonly ICityRepository _cityRepository;
    private readonly ICountryRepository _countryRepository;

    public CreateCityCommandHandler(ICityRepository cityRepository, ICountryRepository countryRepository)
    {
        _cityRepository = cityRepository;
        _countryRepository = countryRepository;
    }

    public async Task<HttpResponse<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new DomainException(Errores.CIUDAD_INVALIDA, "El nombre de la ciudad es obligatorio.");

        var country = await _countryRepository.GetByIdAsync(request.CountryId);
        if (country is null)
            throw new DomainException(Errores.PAIS_NO_ENCONTRADO,
                $"El pais con Id {request.CountryId} no existe en el catalogo de direcciones.");

        var ciudad = await _cityRepository.AddAsync(new City
        {
            Name = request.Name.Trim(),
            CountryId = request.CountryId
        });

        return new HttpResponse<CityDto>(new CityDto
        {
            Id = ciudad.Id,
            Name = ciudad.Name,
            CountryId = ciudad.CountryId,
            CountryName = country.Name,
            IsActive = ciudad.IsActive
        });
    }
}