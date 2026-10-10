using Addresses.Core.DTOs;
using Addresses.Core.Extensions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Addresses.Core.Features.States;

public class GetStatesQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<StateDto>>>>
{
}

public class GetStatesQueryHandler : IRequestHandler<GetStatesQuery, HttpResponse<PagedDto<List<StateDto>>>>
{
    private readonly IStateRepository _stateRepository;

    public GetStatesQueryHandler(IStateRepository stateRepository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<HttpResponse<PagedDto<List<StateDto>>>> Handle(GetStatesQuery request, CancellationToken cancellationToken)
    {
        var result = await _stateRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<State>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken,
            includes: [s => s.Country]);

        return new HttpResponse<PagedDto<List<StateDto>>>(result.ToPagedDto(s => new StateDto
        {
            Id = s.Id,
            Name = s.Name,
            CountryId = s.CountryId,
            CountryName = s.Country?.Name ?? string.Empty,
            IsActive = s.IsActive
        }));
    }
}