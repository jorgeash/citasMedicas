using Addresses.Core.DTOs;
using Addresses.Core.Extensions;
using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Addresses.Core.Features.Positions;

public class GetPositionsQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<PositionDto>>>>
{
}

public class GetPositionsQueryHandler : IRequestHandler<GetPositionsQuery, HttpResponse<PagedDto<List<PositionDto>>>>
{
    private readonly IPositionRepository _positionRepository;

    public GetPositionsQueryHandler(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<HttpResponse<PagedDto<List<PositionDto>>>> Handle(GetPositionsQuery request, CancellationToken cancellationToken)
    {
        var result = await _positionRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<Position>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken,
            includes: [p => p.City!, p => p.City!.Country!]);

        return new HttpResponse<PagedDto<List<PositionDto>>>(result.ToPagedDto(p => new PositionDto
        {
            Id = p.Id,
            Address = p.Address,
            ZipCode = p.ZipCode,
            CityId = p.CityId,
            CityName = p.City?.Name ?? string.Empty,
            CountryId = p.City?.CountryId ?? 0,
            CountryName = p.City?.Country?.Name ?? string.Empty,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            CustomerId = p.CustomerId,
            IsActive = p.IsActive
        }));
    }
}