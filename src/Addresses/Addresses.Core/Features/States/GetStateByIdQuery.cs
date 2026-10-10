using Addresses.Core.DTOs;
using Addresses.Core.Interfaces.Repositories;
using MediatR;

namespace Addresses.Core.Features.States;

public class GetStateByIdQuery : IRequest<HttpResponse<StateDto>?>
{
    public int Id { get; set; }

    public GetStateByIdQuery(int id)
    {
        Id = id;
    }
}

public class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, HttpResponse<StateDto>?>
{
    private readonly IStateRepository _stateRepository;

    public GetStateByIdQueryHandler(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<HttpResponse<StateDto>?> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
    {
        var state = await _stateRepository.GetOneByAsync(
            s => s.Id == request.Id,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: [s => s.Country]);

        if (state is null)
            return null;

        return new HttpResponse<StateDto>(new StateDto
        {
            Id = state.Id,
            Name = state.Name,
            CountryId = state.CountryId,
            CountryName = state.Country?.Name ?? string.Empty,
            IsActive = state.IsActive
        });
    }
}