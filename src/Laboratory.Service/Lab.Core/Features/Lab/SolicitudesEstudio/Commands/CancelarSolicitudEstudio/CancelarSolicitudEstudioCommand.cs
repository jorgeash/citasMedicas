using Lab.Core.DTOs.Lab;
using Lab.Core.Exceptions;
using Lab.Core.Features.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;

namespace Lab.Core.Features.Lab.SolicitudesEstudio.Commands.CancelarSolicitudEstudio;

public class CancelarSolicitudEstudioCommand : IRequest<SolicitudEstudioDto>
{
    public int SolicitudEstudioID { get; set; }
    public string? Motivo { get; set; }
}

public class CancelarSolicitudEstudioCommandHandler : IRequestHandler<CancelarSolicitudEstudioCommand, SolicitudEstudioDto>
{
    private readonly ISolicitudEstudioRepository _repository;

    public CancelarSolicitudEstudioCommandHandler(ISolicitudEstudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<SolicitudEstudioDto> Handle(CancelarSolicitudEstudioCommand request, CancellationToken cancellationToken)
    {
        var motivo = LabMapper.Truncar(request.Motivo, 480);

        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException(Errores.MOTIVO_OBLIGATORIO, "El motivo de la cancelación es obligatorio.");

        var solicitud = await _repository.GetOneByAsync(
            s => s.SolicitudEstudioID == request.SolicitudEstudioID,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: new System.Linq.Expressions.Expression<Func<SolicitudEstudio, object>>[]
            {
                s => s.Detalle,
                s => s.Resultados
            });

        if (solicitud is null)
            throw new DomainException(Errores.SOLICITUD_NO_ENCONTRADA, $"No existe la solicitud {request.SolicitudEstudioID}.");

        if (solicitud.Estado != Estados.SolicitudPendiente && solicitud.Estado != Estados.SolicitudEnProceso)
            throw new DomainException(
                Errores.SOLICITUD_ESTADO_INVALIDO,
                $"La solicitud está {solicitud.Estado} y solo se puede cancelar desde PENDIENTE o EN_PROCESO.");

        if (solicitud.Resultados.Count > 0)
            throw new DomainException(
                Errores.SOLICITUD_CON_RESULTADOS,
                "No se puede cancelar una solicitud que ya tiene resultados registrados.");

        var cancelacion = $"CANCELADA: {motivo}";
        solicitud.Observaciones = string.IsNullOrWhiteSpace(solicitud.Observaciones)
            ? cancelacion
            : LabMapper.Truncar($"{solicitud.Observaciones} | {cancelacion}", 500);
        solicitud.Estado = Estados.SolicitudCancelada;
        solicitud.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(solicitud);

        return LabMapper.ToDto(solicitud);
    }
}
