using System.Linq.Expressions;
using Lab.Core.DTOs;
using Lab.Core.DTOs.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Lab.Core.Features.Lab.SolicitudesEstudio.Queries.GetSolicitudesEstudio;

public class GetSolicitudesEstudioQuery : RequestParameters, IRequest<PagedDto<List<SolicitudEstudioDto>>>
{
    public long? AtencionID { get; set; }
    public long? PacienteID { get; set; }
    public int? MedicoID { get; set; }
    public string? Estado { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class GetSolicitudesEstudioQueryHandler : IRequestHandler<GetSolicitudesEstudioQuery, PagedDto<List<SolicitudEstudioDto>>>
{
    private readonly ISolicitudEstudioRepository _repository;

    public GetSolicitudesEstudioQueryHandler(ISolicitudEstudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<SolicitudEstudioDto>>> Handle(
        GetSolicitudesEstudioQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<SolicitudEstudio, bool>>? filtro = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<SolicitudEstudio>(request.Filter);

        if (request.AtencionID.HasValue)
        {
            var atencionID = request.AtencionID.Value;
            filtro = filtro is null
                ? s => s.AtencionID == atencionID
                : Filter.AndAlso(filtro, s => s.AtencionID == atencionID);
        }

        if (request.PacienteID.HasValue)
        {
            var pacienteID = request.PacienteID.Value;
            filtro = filtro is null
                ? s => s.PacienteID == pacienteID
                : Filter.AndAlso(filtro, s => s.PacienteID == pacienteID);
        }

        if (request.MedicoID.HasValue)
        {
            var medicoID = request.MedicoID.Value;
            filtro = filtro is null
                ? s => s.MedicoID == medicoID
                : Filter.AndAlso(filtro, s => s.MedicoID == medicoID);
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estado = request.Estado.Trim().ToUpperInvariant();
            filtro = filtro is null
                ? s => s.Estado == estado
                : Filter.AndAlso(filtro, s => s.Estado == estado);
        }

        if (request.Desde.HasValue)
        {
            var desde = request.Desde.Value.Date;
            filtro = filtro is null
                ? s => s.FechaSolicitud >= desde
                : Filter.AndAlso(filtro, s => s.FechaSolicitud >= desde);
        }

        if (request.Hasta.HasValue)
        {
            var hasta = request.Hasta.Value.Date.AddDays(1);
            filtro = filtro is null
                ? s => s.FechaSolicitud < hasta
                : Filter.AndAlso(filtro, s => s.FechaSolicitud < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<SolicitudEstudio>(request.Buscar, "NumeroSolicitud", "Observaciones");
            filtro = filtro is null ? textoLibre : Filter.AndAlso(filtro, textoLibre);
        }

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filtro,
            Filter.ParseOrderBy<SolicitudEstudio>(request.OrderBy ?? string.Empty),
            cancellationToken: cancellationToken,
            includes: new Expression<Func<SolicitudEstudio, object>>[]
            {
                s => s.Detalle,
                s => s.Resultados
            });

        return new PagedDto<List<SolicitudEstudioDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(LabMapper.ToDto).ToList());
    }
}
