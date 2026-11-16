using System.Linq.Expressions;
using MediatR;
using Scheduling.Core.DTOs;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Shared.Kernel.Helpers;

namespace Scheduling.Core.Features.Scheduling.Citas.Queries.GetCitas;

public class GetCitasQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<CitaDto>>>>
{
    public long? PacienteID { get; set; }
    public int? EstadoCitaID { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class GetCitasQueryHandler : IRequestHandler<GetCitasQuery, HttpResponse<PagedDto<List<CitaDto>>>>
{
    private readonly ICitaRepository _citaRepository;

    public GetCitasQueryHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<HttpResponse<PagedDto<List<CitaDto>>>> Handle(
        GetCitasQuery request,
        CancellationToken cancellationToken)
    {
        var pacienteID = request.PacienteID;
        var estadoCitaID = request.EstadoCitaID;
        var desde = request.Desde;
        var hasta = request.Hasta;

        Expression<Func<Cita, bool>> filter = c =>
            (pacienteID == null || c.PacienteID == pacienteID.Value) &&
            (estadoCitaID == null || c.EstadoCitaID == estadoCitaID.Value) &&
            (desde == null || c.FechaHoraInicio >= desde.Value) &&
            (hasta == null || c.FechaHoraInicio <= hasta.Value);

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<Cita>(
                request.Buscar,
                "Motivo", "Observaciones",
                "Paciente.Nombres", "Paciente.Apellidos",
                "Medico.Nombres", "Medico.Apellidos");
            filter = Filter.AndAlso(filter, textoLibre);
        }

        var result = await _citaRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filter,
            Filter.ParseOrderBy<Cita>(request.OrderBy ?? string.Empty)
                ?? (query => query.OrderByDescending(c => c.FechaHoraInicio)),
            cancellationToken: cancellationToken,
            includes: new Expression<Func<Cita, object>>[]
            {
                c => c.Paciente!,
                c => c.Medico!,
                c => c.EstadoCita!
            });

        return new HttpResponse<PagedDto<List<CitaDto>>>(new PagedDto<List<CitaDto>>
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = result.Data.Select(SchedulingMapper.ToDto).ToList()
        });
    }
}
