using MediatR;
using Notification.Core.DTOs.Notification;
using Notification.Core.Exceptions;
using Notification.Core.Interfaces.Repositories;
using Notification.Domain.Entities;

namespace Notification.Core.Features.Notification.Commands.CreateNotificacion;

public class CreateNotificacionCommand : IRequest<NotificacionDto>
{
    public string EventoID { get; set; } = string.Empty;
    public long UsuarioID { get; set; }
    public string? Destinatario { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Canal { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public string OrigenServicio { get; set; } = string.Empty;
    public string? Referencia { get; set; }
    public DateTime? FechaProgramada { get; set; }
}

public class CreateNotificacionCommandHandler : IRequestHandler<CreateNotificacionCommand, NotificacionDto>
{
    private const string CanalInterno = "INTERNO";
    private const string CanalEmail = "EMAIL";

    private readonly INotificacionRepository _repository;

    public CreateNotificacionCommandHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificacionDto> Handle(CreateNotificacionCommand request, CancellationToken cancellationToken)
    {
        var eventoId = NotificationMapper.Truncar(request.EventoID?.Trim().ToUpperInvariant(), 100);
        if (string.IsNullOrWhiteSpace(eventoId))
            throw new DomainException(Errores.EVENTO_INVALIDO, "El EventoID de la notificación es obligatorio.");

        if (request.UsuarioID <= 0)
            throw new DomainException(Errores.DATOS_INVALIDOS, "El UsuarioID debe ser mayor que cero.");

        var tipo = NotificationMapper.Truncar(request.Tipo?.Trim().ToUpperInvariant(), 30);
        if (string.IsNullOrWhiteSpace(tipo) || !Estados.Tipos.Contains(tipo))
            throw new DomainException(Errores.TIPO_INVALIDO, $"El tipo de notificación '{request.Tipo}' no es válido.");

        var canal = NotificationMapper.Truncar(request.Canal?.Trim().ToUpperInvariant(), 20);
        if (string.IsNullOrWhiteSpace(canal) || !Estados.Canales.Contains(canal))
            throw new DomainException(Errores.CANAL_INVALIDO, $"El canal de notificación '{request.Canal}' no es válido.");

        var prioridad = NotificationMapper.Truncar(request.Prioridad?.Trim().ToUpperInvariant(), 20);
        if (string.IsNullOrWhiteSpace(prioridad) || !Estados.Prioridades.Contains(prioridad))
            throw new DomainException(Errores.PRIORIDAD_INVALIDA, $"La prioridad '{request.Prioridad}' no es válida.");

        var origen = NotificationMapper.Truncar(request.OrigenServicio?.Trim().ToUpperInvariant(), 30);
        if (string.IsNullOrWhiteSpace(origen) || !Estados.Origenes.Contains(origen))
            throw new DomainException(Errores.ORIGEN_INVALIDO, $"El origen '{request.OrigenServicio}' no es válido.");

        var titulo = NotificationMapper.Truncar(request.Titulo, 200);
        var mensaje = NotificationMapper.Truncar(request.Mensaje, 1000);
        if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(mensaje))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El título y el mensaje de la notificación son obligatorios.");

        var destinatario = NotificationMapper.Truncar(request.Destinatario, 256);
        if (canal != CanalInterno && string.IsNullOrWhiteSpace(destinatario))
            throw new DomainException(Errores.DESTINATARIO_INVALIDO, "El destinatario es obligatorio para este canal.");

        if (canal == CanalEmail && (destinatario is null || !destinatario.Contains('@')))
            throw new DomainException(Errores.DESTINATARIO_INVALIDO, "El destinatario de un correo electrónico debe contener '@'.");

        if (request.FechaProgramada.HasValue && request.FechaProgramada.Value < DateTime.UtcNow)
            throw new DomainException(Errores.FECHA_PROGRAMADA_INVALIDA, "La fecha programada no puede ser menor a la fecha actual.");

        var existente = await _repository.GetOneByAsync(
            n => n.EventoID == eventoId,
            cancellationToken: cancellationToken);

        if (existente is not null)
            return NotificationMapper.ToDto(existente);

        var ahora = DateTime.UtcNow;
        var notificacion = new Notificacion
        {
            EventoID = eventoId,
            UsuarioID = request.UsuarioID,
            Destinatario = destinatario,
            Titulo = titulo!,
            Mensaje = mensaje!,
            Tipo = tipo,
            Canal = canal,
            Prioridad = prioridad,
            Estado = Estados.Pendiente,
            OrigenServicio = origen,
            Referencia = NotificationMapper.Truncar(request.Referencia, 100),
            FechaCreacion = ahora,
            FechaProgramada = request.FechaProgramada,
            Intentos = 0,
            Activo = true,
            CreatedAt = ahora
        };

        var creada = await _repository.AddAsync(notificacion);
        return NotificationMapper.ToDto(creada);
    }
}
