using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Atenciones.Queries.GetAtenciones;

public class GetAtencionesQuery : RequestParameters, IRequest<PagedDto<List<AtencionResumenDto>>>
{
    public long? PacienteID { get; set; }
    public int? MedicoID { get; set; }
    public string? Estado { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class GetAtencionesQueryHandler : IRequestHandler<GetAtencionesQuery, PagedDto<List<AtencionResumenDto>>>
{
    private readonly IAtencionRepository _repository;

    public GetAtencionesQueryHandler(IAtencionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<AtencionResumenDto>>> Handle(GetAtencionesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Atencion, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Atencion>(request.Filter);

        if (request.PacienteID.HasValue)
        {
            var pacienteID = request.PacienteID.Value;
            filter = filter is null
                ? a => a.PacienteID == pacienteID
                : Filter.AndAlso(filter, a => a.PacienteID == pacienteID);
        }

        if (request.MedicoID.HasValue)
        {
            var medicoID = request.MedicoID.Value;
            filter = filter is null
                ? a => a.MedicoID == medicoID
                : Filter.AndAlso(filter, a => a.MedicoID == medicoID);
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estado = request.Estado.Trim().ToUpperInvariant();
            filter = filter is null
                ? a => a.Estado == estado
                : Filter.AndAlso(filter, a => a.Estado == estado);
        }

        if (request.Desde.HasValue)
        {
            var desde = request.Desde.Value;
            filter = filter is null
                ? a => a.FechaAtencion >= desde
                : Filter.AndAlso(filter, a => a.FechaAtencion >= desde);
        }

        if (request.Hasta.HasValue)
        {
            var hasta = request.Hasta.Value.Date.AddDays(1);
            filter = filter is null
                ? a => a.FechaAtencion < hasta
                : Filter.AndAlso(filter, a => a.FechaAtencion < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<Atencion>(request.Buscar, "Motivo", "Conclusion", "Estado");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<Atencion>(request.OrderBy)
            : (Func<IQueryable<Atencion>, IOrderedQueryable<Atencion>>)(q => q.OrderByDescending(a => a.FechaAtencion));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<AtencionResumenDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToResumenDto).ToList());
    }
}
