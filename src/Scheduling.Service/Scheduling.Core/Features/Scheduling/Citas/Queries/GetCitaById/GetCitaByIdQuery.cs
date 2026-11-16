using System.Linq.Expressions;
using MediatR;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;

namespace Scheduling.Core.Features.Scheduling.Citas.Queries.GetCitaById;

public class GetCitaByIdQuery : IRequest<CitaDto?>
{
    public long CitaID { get; set; }

    public GetCitaByIdQuery(long citaID)
    {
        CitaID = citaID;
    }
}

public class GetCitaByIdQueryHandler : IRequestHandler<GetCitaByIdQuery, CitaDto?>
{
    private readonly ICitaRepository _citaRepository;

    public GetCitaByIdQueryHandler(ICitaRepository citaRepository)
    {
        _citaRepository = citaRepository;
    }

    public async Task<CitaDto?> Handle(GetCitaByIdQuery request, CancellationToken cancellationToken)
    {
        var cita = await _citaRepository.GetOneByAsync(
            c => c.CitaID == request.CitaID,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<Cita, object>>[]
            {
                c => c.Paciente!,
                c => c.Medico!,
                c => c.EstadoCita!
            });

        return cita is null ? null : SchedulingMapper.ToDto(cita);
    }
}
