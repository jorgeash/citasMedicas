using Addresses.Core.DTOs;
using Addresses.Core.Exceptions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;

namespace Addresses.Core.Features.Countries;

public class CreateCountryCommand : IRequest<HttpResponse<CountryDto>>
{
    public string Name { get; set; } = string.Empty;
}

public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, HttpResponse<CountryDto>>
{
    private readonly ICountryRepository _countryRepository;

    public CreateCountryCommandHandler(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<HttpResponse<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new DomainException(Errores.PAIS_INVALIDO, "El nombre del pais es obligatorio.");

        var nombre = request.Name.Trim();
        var existente = await _countryRepository.GetOneByAsync(c => c.Name == nombre, cancellationToken: cancellationToken);

        if (existente is not null)
            throw new DomainException(Errores.PAIS_DUPLICADO, $"El pais '{nombre}' ya esta registrado.");

        var country = await _countryRepository.AddAsync(new Country { Name = nombre });

        return new HttpResponse<CountryDto>(new CountryDto
        {
            Id = country.Id,
            Name = country.Name,
            IsActive = country.IsActive
        });
    }
}