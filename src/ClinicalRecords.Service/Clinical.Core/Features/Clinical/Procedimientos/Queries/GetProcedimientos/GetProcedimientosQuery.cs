using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Procedimientos.Queries.GetProcedimientos;

public class GetProcedimientosQuery : RequestParameters, IRequest<PagedDto<List<ProcedimientoDto>>>
{
    public bool? Activo { get; set; }
}

public class GetProcedimientosQueryHandler : IRequestHandler<GetProcedimientosQuery, PagedDto<List<ProcedimientoDto>>>
{
    private readonly IProcedimientoRepository _repository;

    public GetProcedimientosQueryHandler(IProcedimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<ProcedimientoDto>>> Handle(GetProcedimientosQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Procedimiento, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Procedimiento>(request.Filter);

        if (request.Activo.HasValue)
        {
            var activo = request.Activo.Value;
            filter = filter is null
                ? p => p.Activo == activo
                : Filter.AndAlso(filter, p => p.Activo == activo);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<Procedimiento>(request.Buscar, "Codigo", "Nombre", "Descripcion");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Procedimiento>(request.OrderBy)
            : (Func<IQueryable<Procedimiento>, IOrderedQueryable<Procedimiento>>)(q => q.OrderBy(p => p.Nombre));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<ProcedimientoDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToDto).ToList());
    }
}
