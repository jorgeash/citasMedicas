using Lab.Core.DTOs.Lab;
using Lab.Core.Exceptions;
using Lab.Core.Features.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Core.Services;
using Lab.Domain.Entities;
using MediatR;

namespace Lab.Core.Features.Lab.SolicitudesEstudio.Commands.CreateSolicitudEstudio;

public class CreateSolicitudEstudioCommand : IRequest<SolicitudEstudioDto>
{
    public long AtencionID { get; set; }
    public long PacienteID { get; set; }
    public int MedicoID { get; set; }
    public string? Observaciones { get; set; }
    public List<CreateSolicitudEstudioItem> Detalle { get; set; } = new();
}

public class CreateSolicitudEstudioItem
{
    public int EstudioID { get; set; }
    public string Prioridad { get; set; } = "NORMAL";
    public string? Observaciones { get; set; }
}

public class CreateSolicitudEstudioCommandHandler : IRequestHandler<CreateSolicitudEstudioCommand, SolicitudEstudioDto>
{
    private readonly ISolicitudEstudioRepository _solicitudRepository;
    private readonly IEstudioRepository _estudioRepository;
    private readonly IClinicalRecordsClient _clinicalRecordsClient;
    private readonly IIdentityClient _identityClient;

    public CreateSolicitudEstudioCommandHandler(
        ISolicitudEstudioRepository solicitudRepository,
        IEstudioRepository estudioRepository,
        IClinicalRecordsClient clinicalRecordsClient,
        IIdentityClient identityClient)
    {
        _solicitudRepository = solicitudRepository;
        _estudioRepository = estudioRepository;
        _clinicalRecordsClient = clinicalRecordsClient;
        _identityClient = identityClient;
    }

    public async Task<SolicitudEstudioDto> Handle(CreateSolicitudEstudioCommand request, CancellationToken cancellationToken)
    {
        if (request.AtencionID <= 0 || request.PacienteID <= 0 || request.MedicoID <= 0)
            throw new DomainException(Errores.DATOS_INVALIDOS, "Atención, paciente y médico son obligatorios.");

        if (request.Detalle is null || request.Detalle.Count == 0)
            throw new DomainException(Errores.SOLICITUD_SIN_DETALLE, "La solicitud debe tener al menos un estudio.");

        var atencion = await _clinicalRecordsClient.GetAtencionAsync(request.AtencionID, cancellationToken);
        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        if (atencion.PacienteID != request.PacienteID)
            throw new DomainException(
                Errores.ATENCION_PACIENTE_INCONSISTENTE,
                $"La atención {request.AtencionID} pertenece al paciente {atencion.PacienteID}, no al paciente {request.PacienteID}.");

        var medico = await _identityClient.GetMedicoAsync(request.MedicoID, cancellationToken);
        if (medico is null)
            throw new DomainException(Errores.MEDICO_NO_ENCONTRADO, $"No existe el médico {request.MedicoID}.");

        if (!medico.Activo)
            throw new DomainException(Errores.MEDICO_INACTIVO, $"El médico {request.MedicoID} no está activo.");

        var idsEstudio = new HashSet<int>();
        var ahora = DateTime.UtcNow;

        var solicitud = new SolicitudEstudio
        {
            NumeroSolicitud = $"TMP-{Guid.NewGuid():N}".Substring(0, 20),
            AtencionID = request.AtencionID,
            PacienteID = request.PacienteID,
            MedicoID = request.MedicoID,
            FechaSolicitud = ahora,
            Estado = Estados.SolicitudPendiente,
            Observaciones = LabMapper.Truncar(request.Observaciones, 500),
            CreatedAt = ahora
        };

        foreach (var item in request.Detalle)
        {
            if (!idsEstudio.Add(item.EstudioID))
                throw new DomainException(Errores.DETALLE_DUPLICADO, $"El estudio {item.EstudioID} está repetido en la solicitud.");

            var estudio = await _estudioRepository.GetOneByAsync(
                e => e.EstudioID == item.EstudioID,
                cancellationToken: cancellationToken);

            if (estudio is null || !estudio.Activo)
                throw new DomainException(Errores.ESTUDIO_NO_ACTIVO, $"El estudio {item.EstudioID} no existe o no está activo.");

            var prioridad = string.IsNullOrWhiteSpace(item.Prioridad)
                ? Estados.PrioridadNormal
                : item.Prioridad.Trim().ToUpperInvariant();

            if (prioridad != Estados.PrioridadNormal && prioridad != Estados.PrioridadUrgente)
                throw new DomainException(Errores.DATOS_INVALIDOS, $"La prioridad '{item.Prioridad}' no es válida.");

            solicitud.Detalle.Add(new SolicitudEstudioDetalle
            {
                EstudioID = estudio.EstudioID,
                Prioridad = prioridad,
                Observaciones = LabMapper.Truncar(item.Observaciones, 500),
                CreatedAt = ahora
            });
        }

        var creada = await _solicitudRepository.AddAsync(solicitud);

        creada.NumeroSolicitud = $"SE-{creada.FechaSolicitud:yyyy}-{creada.SolicitudEstudioID:D6}";
        await _solicitudRepository.UpdateAsync(creada);

        return LabMapper.ToDto(creada);
    }
}
