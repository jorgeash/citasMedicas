using Addresses.Core.DTOs;
using Addresses.Core.Exceptions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;

namespace Addresses.Core.Features.States;

public class CreateStateCommand : IRequest<HttpResponse<StateDto>>
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}

public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, HttpResponse<StateDto>>
{
    private readonly IStateRepository _stateRepository;
    private readonly ICountryRepository _countryRepository;

    public CreateStateCommandHandler(IStateRepository stateRepository, ICountryRepository countryRepository)
    {
        _stateRepository = stateRepository;
        _countryRepository = countryRepository;
    }

    public async Task<HttpResponse<StateDto>> Handle(CreateStateCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new DomainException(Errores.DIRECCION_INVALIDA, "El nombre del estado es obligatorio.");

        var country = await _countryRepository.GetByIdAsync(request.CountryId);
        if (country is null)
            throw new DomainException(Errores.PAIS_NO_ENCONTRADO,
                $"El pais con Id {request.CountryId} no existe en el catalogo de direcciones.");

        var state = await _stateRepository.AddAsync(new State
        {
            Name = request.Name.Trim(),
            CountryId = request.CountryId
        });

        return new HttpResponse<StateDto>(new StateDto
        {
            Id = state.Id,
            Name = state.Name,
            CountryId = state.CountryId,
            CountryName = country.Name,
            IsActive = state.IsActive
        });
    }
}