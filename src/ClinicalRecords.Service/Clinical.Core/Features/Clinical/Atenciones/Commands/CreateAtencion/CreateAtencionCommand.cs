using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Core.Services;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Atenciones.Commands.CreateAtencion;

public class CreateAtencionCommand : IRequest<AtencionDto>
{
    public long CitaID { get; set; }
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaAtencion { get; set; }
    public string? Motivo { get; set; }
    public string? Antecedente { get; set; }
    public string? ExamenFisico { get; set; }
    public string? Conclusion { get; set; }
}

public class CreateAtencionCommandHandler : IRequestHandler<CreateAtencionCommand, AtencionDto>
{
    private readonly IAtencionRepository _atenciones;
    private readonly IExpedienteClinicoRepository _expedientes;
    private readonly ISchedulingClient _schedulingClient;
    private readonly IIdentityClient _identityClient;

    public CreateAtencionCommandHandler(
        IAtencionRepository atenciones,
        IExpedienteClinicoRepository expedientes,
        ISchedulingClient schedulingClient,
        IIdentityClient identityClient)
    {
        _atenciones = atenciones;
        _expedientes = expedientes;
        _schedulingClient = schedulingClient;
        _identityClient = identityClient;
    }

    public async Task<AtencionDto> Handle(CreateAtencionCommand request, CancellationToken cancellationToken)
    {
        if (request.CitaID <= 0 || request.PacienteID <= 0 || request.MedicoID <= 0)
            throw new DomainException(Errores.DATOS_INVALIDOS, "Cita, paciente y médico son obligatorios.");

        var cita = await _schedulingClient.GetCitaAsync(request.CitaID, cancellationToken);
        if (cita is null)
            throw new DomainException(Errores.CITA_NO_ENCONTRADA, $"No existe la cita {request.CitaID}.");

        if (cita.PacienteID != request.PacienteID)
            throw new DomainException(
                Errores.CITA_PACIENTE_INCONSISTENTE,
                $"La cita {request.CitaID} pertenece al paciente {cita.PacienteID}, no al paciente {request.PacienteID}.");

        var medico = await _identityClient.GetMedicoAsync(request.MedicoID, cancellationToken);
        if (medico is not null && (medico.MedicoID != request.MedicoID || !medico.Activo))
            throw new DomainException(Errores.MEDICO_NO_VALIDO, $"El médico {request.MedicoID} no existe o no está activo.");

        var duplicada = await _atenciones.GetOneByAsync(
            a => a.CitaID == request.CitaID,
            cancellationToken: cancellationToken);

        if (duplicada is not null)
            throw new DomainException(Errores.ATENCION_CITA_DUPLICADA, $"Ya existe una atención para la cita {request.CitaID}.");

        var fechaAtencion = request.FechaAtencion == default ? DateTime.UtcNow : request.FechaAtencion;

        var expediente = await _expedientes.GetOneByAsync(
            e => e.PacienteID == request.PacienteID && e.Activo,
            cancellationToken: cancellationToken);

        if (expediente is null)
        {
            expediente = new ExpedienteClinico
            {
                PacienteID = request.PacienteID,
                NumeroExpediente = $"TMP-{Guid.NewGuid():N}".Substring(0, 20),
                FechaApertura = DateOnly.FromDateTime(fechaAtencion),
                Activo = true,
                CreatedAt = DateTime.UtcNow
            };

            expediente = await _expedientes.AddAsync(expediente);
            expediente.NumeroExpediente = $"EXP-{expediente.FechaApertura:yyyy}-{expediente.ExpedienteClinicoID:D6}";
            await _expedientes.UpdateAsync(expediente);
        }

        var atencion = new Atencion
        {
            CitaID = request.CitaID,
            PacienteID = request.PacienteID,
            ExpedienteClinicoID = expediente.ExpedienteClinicoID,
            MedicoID = request.MedicoID,
            FechaAtencion = fechaAtencion,
            Motivo = request.Motivo,
            Antecedente = request.Antecedente,
            ExamenFisico = request.ExamenFisico,
            Conclusion = request.Conclusion,
            Estado = "EN_CURSO",
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _atenciones.AddAsync(atencion);
        return ClinicalMapper.ToDto(creada);
    }
}
