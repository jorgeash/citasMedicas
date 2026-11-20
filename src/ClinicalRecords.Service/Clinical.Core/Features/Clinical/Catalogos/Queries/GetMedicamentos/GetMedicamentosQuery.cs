using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Catalogos.Queries.GetMedicamentos;

public class GetMedicamentosQuery : RequestParameters, IRequest<PagedDto<List<MedicamentoDto>>>
{
    public bool? Activo { get; set; }
}

public class GetMedicamentosQueryHandler : IRequestHandler<GetMedicamentosQuery, PagedDto<List<MedicamentoDto>>>
{
    private readonly IMedicamentoRepository _repository;

    public GetMedicamentosQueryHandler(IMedicamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<MedicamentoDto>>> Handle(GetMedicamentosQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Medicamento, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Medicamento>(request.Filter);

        if (request.Activo.HasValue)
        {
            var activo = request.Activo.Value;
            filter = filter is null
                ? m => m.Activo == activo
                : Filter.AndAlso(filter, m => m.Activo == activo);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<Medicamento>(request.Buscar, "Codigo", "Nombre", "Presentacion", "Concentracion");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Medicamento>(request.OrderBy)
            : (Func<IQueryable<Medicamento>, IOrderedQueryable<Medicamento>>)(q => q.OrderBy(m => m.Nombre));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<MedicamentoDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToDto).ToList());
    }
}
