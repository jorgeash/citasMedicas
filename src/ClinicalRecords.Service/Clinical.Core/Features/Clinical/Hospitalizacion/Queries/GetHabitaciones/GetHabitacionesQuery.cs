using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Hospitalizacion.Queries.GetHabitaciones;

public class GetHabitacionesQuery : RequestParameters, IRequest<PagedDto<List<HabitacionDto>>>
{
    public bool? Activa { get; set; }
}

public class GetHabitacionesQueryHandler : IRequestHandler<GetHabitacionesQuery, PagedDto<List<HabitacionDto>>>
{
    private readonly IHabitacionRepository _repository;

    public GetHabitacionesQueryHandler(IHabitacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<HabitacionDto>>> Handle(GetHabitacionesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Habitacion, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Habitacion>(request.Filter);

        if (request.Activa.HasValue)
        {
            var activa = request.Activa.Value;
            filter = filter is null
                ? h => h.Activa == activa
                : Filter.AndAlso(filter, h => h.Activa == activa);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<Habitacion>(request.Buscar, "Codigo", "Numero", "Tipo", "Piso");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Habitacion>(request.OrderBy)
            : (Func<IQueryable<Habitacion>, IOrderedQueryable<Habitacion>>)(q => q.OrderBy(h => h.Numero));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<HabitacionDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToDto).ToList());
    }
}
