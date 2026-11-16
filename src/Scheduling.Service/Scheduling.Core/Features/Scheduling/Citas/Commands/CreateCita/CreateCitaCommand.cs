using MediatR;
using Scheduling.Core.Exceptions;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;

namespace Scheduling.Core.Features.Scheduling.Citas.Commands.CreateCita;

public class CreateCitaCommand : IRequest<long>
{
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public int? EspecialidadID { get; set; }
    public int? ConsultorioID { get; set; }
    public int? EstadoCitaID { get; set; }
    public DateTime FechaHoraInicio { get; set; }
    public DateTime? FechaHoraFin { get; set; }
    public string? Motivo { get; set; }
    public string? Observaciones { get; set; }
    public int? UsuarioCreacionID { get; set; }
}

public class CreateCitaCommandHandler : IRequestHandler<CreateCitaCommand, long>
{
    private readonly ICitaRepository _citaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IEstadoCitaRepository _estadoCitaRepository;

    public CreateCitaCommandHandler(
        ICitaRepository citaRepository,
        IPacienteRepository pacienteRepository,
        IMedicoRepository medicoRepository,
        IEstadoCitaRepository estadoCitaRepository)
    {
        _citaRepository = citaRepository;
        _pacienteRepository = pacienteRepository;
        _medicoRepository = medicoRepository;
        _estadoCitaRepository = estadoCitaRepository;
    }

    public async Task<long> Handle(CreateCitaCommand request, CancellationToken cancellationToken)
    {
        if (request.FechaHoraInicio == default)
            throw new DomainException(Errores.DATOS_INVALIDOS, "La fecha y hora de inicio son obligatorias.");

        if (request.FechaHoraFin.HasValue && request.FechaHoraFin.Value < request.FechaHoraInicio)
            throw new DomainException(Errores.DATOS_INVALIDOS, "La fecha de fin no puede ser anterior a la de inicio.");

        var paciente = await _pacienteRepository.GetOneByAsync(
            p => p.PacienteID == request.PacienteID,
            cancellationToken: cancellationToken);

        if (paciente is null)
            throw new DomainException(Errores.PACIENTE_NO_ENCONTRADO, $"No existe el paciente {request.PacienteID}.");

        var medico = await _medicoRepository.GetOneByAsync(
            m => m.MedicoID == request.MedicoID,
            cancellationToken: cancellationToken);

        if (medico is null)
            throw new DomainException(Errores.MEDICO_NO_ENCONTRADO, $"No existe el médico {request.MedicoID}.");

        var estado = request.EstadoCitaID.HasValue
            ? await _estadoCitaRepository.GetOneByAsync(
                e => e.EstadoCitaID == request.EstadoCitaID.Value,
                cancellationToken: cancellationToken)
            : await _estadoCitaRepository.GetOneByAsync(
                e => e.Codigo == "PROGRAMADA",
                cancellationToken: cancellationToken);

        if (estado is null)
            throw new DomainException(Errores.ESTADO_CITA_NO_ENCONTRADO, "No existe el estado de cita solicitado.");

        var cita = new Cita
        {
            PacienteID = request.PacienteID,
            MedicoID = request.MedicoID,
            EspecialidadID = request.EspecialidadID,
            ConsultorioID = request.ConsultorioID,
            EstadoCitaID = estado.EstadoCitaID,
            FechaHoraInicio = request.FechaHoraInicio,
            FechaHoraFin = request.FechaHoraFin,
            Motivo = request.Motivo,
            Observaciones = request.Observaciones,
            UsuarioCreacionID = request.UsuarioCreacionID,
            FechaCreacion = DateTime.UtcNow
        };

        var creada = await _citaRepository.AddAsync(cita);
        return creada.CitaID;
    }
}
