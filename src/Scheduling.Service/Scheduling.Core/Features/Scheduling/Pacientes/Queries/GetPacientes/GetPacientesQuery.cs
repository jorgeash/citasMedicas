using System.Linq.Expressions;
using MediatR;
using Scheduling.Core.DTOs;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Shared.Kernel.Helpers;

namespace Scheduling.Core.Features.Scheduling.Pacientes.Queries.GetPacientes;

public class GetPacientesQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<PacienteDto>>>>
{
}

public class GetPacientesQueryHandler : IRequestHandler<GetPacientesQuery, HttpResponse<PagedDto<List<PacienteDto>>>>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetPacientesQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<HttpResponse<PagedDto<List<PacienteDto>>>> Handle(
        GetPacientesQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Paciente, bool>>? filtro =
            !string.IsNullOrWhiteSpace(request.Filter)
                ? Filter.FromStringExpression<Paciente>(request.Filter)
                : null;

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<Paciente>(
                request.Buscar,
                "CodigoPaciente", "Nombres", "Apellidos", "NumeroDocumento", "Telefono", "Email");
            filtro = filtro is null ? textoLibre : Filter.AndAlso(filtro, textoLibre);
        }

        var result = await _pacienteRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filtro,
            Filter.ParseOrderBy<Paciente>(request.OrderBy ?? string.Empty)
                ?? (query => query.OrderBy(p => p.Apellidos).ThenBy(p => p.Nombres)),
            cancellationToken: cancellationToken,
            includes: new System.Linq.Expressions.Expression<Func<Paciente, object>>[] { p => p.Seguros });

        return new HttpResponse<PagedDto<List<PacienteDto>>>(new PagedDto<List<PacienteDto>>
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = result.Data.Select(SchedulingMapper.ToDto).ToList()
        });
    }
}
