using MediatR;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;

namespace Scheduling.Core.Features.Scheduling.Pacientes.Queries.GetPacienteById;

public class GetPacienteByIdQuery : IRequest<PacienteDto?>
{
    public long PacienteID { get; set; }

    public GetPacienteByIdQuery(long pacienteID)
    {
        PacienteID = pacienteID;
    }
}

public class GetPacienteByIdQueryHandler : IRequestHandler<GetPacienteByIdQuery, PacienteDto?>
{
    private readonly IPacienteRepository _pacienteRepository;

    public GetPacienteByIdQueryHandler(IPacienteRepository pacienteRepository)
    {
        _pacienteRepository = pacienteRepository;
    }

    public async Task<PacienteDto?> Handle(GetPacienteByIdQuery request, CancellationToken cancellationToken)
    {
        var paciente = await _pacienteRepository.GetOneByAsync(
            p => p.PacienteID == request.PacienteID,
            cancellationToken: cancellationToken,
            includes: new System.Linq.Expressions.Expression<Func<Paciente, object>>[] { p => p.Seguros });

        return paciente is null ? null : SchedulingMapper.ToDto(paciente);
    }
}
