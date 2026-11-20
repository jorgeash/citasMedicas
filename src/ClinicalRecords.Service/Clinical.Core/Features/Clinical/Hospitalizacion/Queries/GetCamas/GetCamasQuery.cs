using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Hospitalizacion.Queries.GetCamas;

public class GetCamasQuery : RequestParameters, IRequest<PagedDto<List<CamaDto>>>
{
    public int? HabitacionID { get; set; }
    public string? Estado { get; set; }
    public bool? Activa { get; set; }
}

public class GetCamasQueryHandler : IRequestHandler<GetCamasQuery, PagedDto<List<CamaDto>>>
{
    private readonly ICamaRepository _repository;

    public GetCamasQueryHandler(ICamaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<CamaDto>>> Handle(GetCamasQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Cama, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Cama>(request.Filter);

        if (request.HabitacionID.HasValue)
        {
            var habitacionID = request.HabitacionID.Value;
            filter = filter is null
                ? c => c.HabitacionID == habitacionID
                : Filter.AndAlso(filter, c => c.HabitacionID == habitacionID);
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estado = request.Estado.Trim().ToUpperInvariant();
            filter = filter is null
                ? c => c.Estado == estado
                : Filter.AndAlso(filter, c => c.Estado == estado);
        }

        if (request.Activa.HasValue)
        {
            var activa = request.Activa.Value;
            filter = filter is null
                ? c => c.Activa == activa
                : Filter.AndAlso(filter, c => c.Activa == activa);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<Cama>(request.Buscar, "Codigo", "Estado");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Cama>(request.OrderBy)
            : (Func<IQueryable<Cama>, IOrderedQueryable<Cama>>)(q => q.OrderBy(c => c.Codigo));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<CamaDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToDto).ToList());
    }
}
