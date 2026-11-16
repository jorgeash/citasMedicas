using System.Linq.Expressions;
using MediatR;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Exceptions;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;

namespace Scheduling.Core.Features.Scheduling.Citas.Commands.CancelarCita;

public class CancelarCitaCommand : IRequest<CitaDto>
{
    public long CitaID { get; set; }
}

public class CancelarCitaCommandHandler : IRequestHandler<CancelarCitaCommand, CitaDto>
{
    private readonly ICitaRepository _citaRepository;
    private readonly IEstadoCitaRepository _estadoCitaRepository;

    public CancelarCitaCommandHandler(
        ICitaRepository citaRepository,
        IEstadoCitaRepository estadoCitaRepository)
    {
        _citaRepository = citaRepository;
        _estadoCitaRepository = estadoCitaRepository;
    }

    public async Task<CitaDto> Handle(CancelarCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = await _citaRepository.GetOneByAsync(
            c => c.CitaID == request.CitaID,
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if (cita is null)
            throw new DomainException(Errores.CITA_NO_ENCONTRADA, $"No existe la cita {request.CitaID}.");

        var cancelada = await _estadoCitaRepository.GetOneByAsync(
            e => e.Codigo == "CANCELADA",
            cancellationToken: cancellationToken);

        if (cancelada is null)
            throw new DomainException(Errores.ESTADO_CITA_NO_ENCONTRADO, "No existe el estado CANCELADA.");

        if (cita.EstadoCitaID == cancelada.EstadoCitaID)
            throw new DomainException(Errores.CITA_YA_CANCELADA, "La cita ya está cancelada.");

        cita.EstadoCitaID = cancelada.EstadoCitaID;
        await _citaRepository.UpdateAsync(cita);

        var actualizada = await _citaRepository.GetOneByAsync(
            c => c.CitaID == request.CitaID,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<Cita, object>>[]
            {
                c => c.Paciente!,
                c => c.Medico!,
                c => c.EstadoCita!
            });

        return SchedulingMapper.ToDto(actualizada!);
    }
}
