using System.Linq.Expressions;
using Lab.Core.DTOs;
using Lab.Core.DTOs.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Lab.Core.Features.Lab.ResultadosEstudio.Queries.GetResultados;

public class GetResultadosQuery : RequestParameters, IRequest<PagedDto<List<ResultadoEstudioDto>>>
{
    public long? PacienteID { get; set; }
    public string? Estado { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class GetResultadosQueryHandler : IRequestHandler<GetResultadosQuery, PagedDto<List<ResultadoEstudioDto>>>
{
    private readonly IResultadoEstudioRepository _resultadoRepository;
    private readonly ISolicitudEstudioRepository _solicitudRepository;

    public GetResultadosQueryHandler(
        IResultadoEstudioRepository resultadoRepository,
        ISolicitudEstudioRepository solicitudRepository)
    {
        _resultadoRepository = resultadoRepository;
        _solicitudRepository = solicitudRepository;
    }

    public async Task<PagedDto<List<ResultadoEstudioDto>>> Handle(
        GetResultadosQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<ResultadoEstudio, bool>>? filtro = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<ResultadoEstudio>(request.Filter);

        if (request.PacienteID.HasValue)
        {
            var solicitudes = await _solicitudRepository.GetAllAsync();
            var ids = solicitudes
                .Where(s => s.PacienteID == request.PacienteID.Value)
                .Select(s => s.SolicitudEstudioID)
                .ToList();

            filtro = filtro is null
                ? r => ids.Contains(r.SolicitudEstudioID)
                : Filter.AndAlso(filtro, r => ids.Contains(r.SolicitudEstudioID));
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estado = request.Estado.Trim().ToUpperInvariant();
            filtro = filtro is null
                ? r => r.Estado == estado
                : Filter.AndAlso(filtro, r => r.Estado == estado);
        }

        if (request.Desde.HasValue)
        {
            var desde = request.Desde.Value.Date;
            filtro = filtro is null
                ? r => r.FechaResultado >= desde
                : Filter.AndAlso(filtro, r => r.FechaResultado >= desde);
        }

        if (request.Hasta.HasValue)
        {
            var hasta = request.Hasta.Value.Date.AddDays(1);
            filtro = filtro is null
                ? r => r.FechaResultado < hasta
                : Filter.AndAlso(filtro, r => r.FechaResultado < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<ResultadoEstudio>(
                request.Buscar,
                "Valor",
                "Unidad",
                "RangoReferencia",
                "Interpretacion");
            filtro = filtro is null ? textoLibre : Filter.AndAlso(filtro, textoLibre);
        }

        var result = await _resultadoRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filtro,
            Filter.ParseOrderBy<ResultadoEstudio>(request.OrderBy ?? "FechaResultado desc"),
            cancellationToken: cancellationToken,
            includes: new Expression<Func<ResultadoEstudio, object>>[]
            {
                r => r.SolicitudEstudio!
            });

        return new PagedDto<List<ResultadoEstudioDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(LabMapper.ToDto).ToList());
    }
}
