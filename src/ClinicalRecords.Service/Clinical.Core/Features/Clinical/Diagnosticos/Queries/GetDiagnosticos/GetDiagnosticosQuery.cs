using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Diagnosticos.Queries.GetDiagnosticos;

public class GetDiagnosticosQuery : RequestParameters, IRequest<PagedDto<List<DiagnosticoDto>>>
{
    public bool? Activo { get; set; }
}

public class GetDiagnosticosQueryHandler : IRequestHandler<GetDiagnosticosQuery, PagedDto<List<DiagnosticoDto>>>
{
    private readonly IDiagnosticoRepository _repository;

    public GetDiagnosticosQueryHandler(IDiagnosticoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<DiagnosticoDto>>> Handle(GetDiagnosticosQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Diagnostico, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Diagnostico>(request.Filter);

        if (request.Activo.HasValue)
        {
            var activo = request.Activo.Value;
            filter = filter is null
                ? d => d.Activo == activo
                : Filter.AndAlso(filter, d => d.Activo == activo);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<Diagnostico>(request.Buscar, "Codigo", "Nombre", "Descripcion");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Diagnostico>(request.OrderBy)
            : (Func<IQueryable<Diagnostico>, IOrderedQueryable<Diagnostico>>)(q => q.OrderBy(d => d.Nombre));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<DiagnosticoDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToDto).ToList());
    }
}
