using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Expedientes.Queries.GetExpedientes;

public class GetExpedientesQuery : RequestParameters, IRequest<PagedDto<List<ExpedienteResumenDto>>>
{
    public long? PacienteID { get; set; }
    public bool? Activo { get; set; }
}

public class GetExpedientesQueryHandler : IRequestHandler<GetExpedientesQuery, PagedDto<List<ExpedienteResumenDto>>>
{
    private readonly IExpedienteClinicoRepository _repository;

    public GetExpedientesQueryHandler(IExpedienteClinicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<ExpedienteResumenDto>>> Handle(GetExpedientesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<ExpedienteClinico, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<ExpedienteClinico>(request.Filter);

        if (request.PacienteID.HasValue)
        {
            var pacienteID = request.PacienteID.Value;
            filter = filter is null
                ? e => e.PacienteID == pacienteID
                : Filter.AndAlso(filter, e => e.PacienteID == pacienteID);
        }

        if (request.Activo.HasValue)
        {
            var activo = request.Activo.Value;
            filter = filter is null
                ? e => e.Activo == activo
                : Filter.AndAlso(filter, e => e.Activo == activo);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
            filter = filter is null
                ? Filter.FreeText<ExpedienteClinico>(request.Buscar, "NumeroExpediente", "Observaciones")
                : Filter.AndAlso(filter, Filter.FreeText<ExpedienteClinico>(request.Buscar, "NumeroExpediente", "Observaciones"));

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<ExpedienteClinico>(request.OrderBy)
            : (Func<IQueryable<ExpedienteClinico>, IOrderedQueryable<ExpedienteClinico>>)(q => q.OrderByDescending(e => e.FechaApertura));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<ExpedienteResumenDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToResumenDto).ToList());
    }
}
