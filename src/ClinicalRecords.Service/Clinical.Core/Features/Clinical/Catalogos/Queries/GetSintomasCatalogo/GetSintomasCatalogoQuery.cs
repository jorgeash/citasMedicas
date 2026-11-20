using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Catalogos.Queries.GetSintomasCatalogo;

public class GetSintomasCatalogoQuery : RequestParameters, IRequest<PagedDto<List<SintomaDto>>>
{
    public bool? Activo { get; set; }
}

public class GetSintomasCatalogoQueryHandler : IRequestHandler<GetSintomasCatalogoQuery, PagedDto<List<SintomaDto>>>
{
    private readonly ISintomaRepository _repository;

    public GetSintomasCatalogoQueryHandler(ISintomaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<SintomaDto>>> Handle(GetSintomasCatalogoQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Sintoma, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Sintoma>(request.Filter);

        if (request.Activo.HasValue)
        {
            var activo = request.Activo.Value;
            filter = filter is null
                ? s => s.Activo == activo
                : Filter.AndAlso(filter, s => s.Activo == activo);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<Sintoma>(request.Buscar, "Codigo", "Nombre", "Descripcion");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Sintoma>(request.OrderBy)
            : (Func<IQueryable<Sintoma>, IOrderedQueryable<Sintoma>>)(q => q.OrderBy(s => s.Nombre));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<SintomaDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToDto).ToList());
    }
}
