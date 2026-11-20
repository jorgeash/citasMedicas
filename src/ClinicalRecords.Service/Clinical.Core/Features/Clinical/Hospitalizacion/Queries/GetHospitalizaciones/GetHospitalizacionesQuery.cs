using System.Linq.Expressions;
using Clinical.Core.DTOs;
using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Clinical.Core.Features.Clinical.Hospitalizacion.Queries.GetHospitalizaciones;

public class GetHospitalizacionesQuery : RequestParameters, IRequest<PagedDto<List<HospitalizacionDto>>>
{
    public int? ExpedienteClinicoID { get; set; }
    public string? Estado { get; set; }
    public bool? SoloActivas { get; set; }
}

public class GetHospitalizacionesQueryHandler : IRequestHandler<GetHospitalizacionesQuery, PagedDto<List<HospitalizacionDto>>>
{
    private readonly IHospitalizacionRepository _repository;

    public GetHospitalizacionesQueryHandler(IHospitalizacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<HospitalizacionDto>>> Handle(GetHospitalizacionesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<global::Clinical.Domain.Entities.Hospitalizacion, bool>>? filter = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<global::Clinical.Domain.Entities.Hospitalizacion>(request.Filter);

        if (request.ExpedienteClinicoID.HasValue)
        {
            var expedienteID = request.ExpedienteClinicoID.Value;
            filter = filter is null
                ? h => h.ExpedienteClinicoID == expedienteID
                : Filter.AndAlso(filter, h => h.ExpedienteClinicoID == expedienteID);
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estado = request.Estado.Trim().ToUpperInvariant();
            filter = filter is null
                ? h => h.Estado == estado
                : Filter.AndAlso(filter, h => h.Estado == estado);
        }

        if (request.SoloActivas == true)
            filter = filter is null
                ? h => h.Estado == "ACTIVA"
                : Filter.AndAlso(filter, h => h.Estado == "ACTIVA");

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var freeText = Filter.FreeText<global::Clinical.Domain.Entities.Hospitalizacion>(request.Buscar, "Motivo", "Diagnostico", "Estado");
            filter = filter is null ? freeText : Filter.AndAlso(filter, freeText);
        }

        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? Filter.ParseOrderBy<global::Clinical.Domain.Entities.Hospitalizacion>(request.OrderBy)
            : (Func<IQueryable<global::Clinical.Domain.Entities.Hospitalizacion>, IOrderedQueryable<global::Clinical.Domain.Entities.Hospitalizacion>>)(q => q.OrderByDescending(h => h.FechaIngreso));

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            orderBy,
            cancellationToken: cancellationToken);

        return new PagedDto<List<HospitalizacionDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(ClinicalMapper.ToDto).ToList());
    }
}
