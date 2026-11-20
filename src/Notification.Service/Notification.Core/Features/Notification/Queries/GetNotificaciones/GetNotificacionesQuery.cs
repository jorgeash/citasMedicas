using System.Linq.Expressions;
using MediatR;
using Notification.Core.DTOs;
using Notification.Core.DTOs.Notification;
using Notification.Core.Interfaces.Repositories;
using Notification.Domain.Entities;
using Shared.Kernel.Helpers;

namespace Notification.Core.Features.Notification.Queries.GetNotificaciones;

public class GetNotificacionesQuery : RequestParameters, IRequest<PagedDto<List<NotificacionDto>>>
{
    public long? UsuarioID { get; set; }
    public string? Tipo { get; set; }
    public string? Canal { get; set; }
    public string? Estado { get; set; }
    public string? OrigenServicio { get; set; }
    public bool? SoloActivas { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
}

public class GetNotificacionesQueryHandler : IRequestHandler<GetNotificacionesQuery, PagedDto<List<NotificacionDto>>>
{
    private readonly INotificacionRepository _repository;

    public GetNotificacionesQueryHandler(INotificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<NotificacionDto>>> Handle(
        GetNotificacionesQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Notificacion, bool>>? filtro = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Notificacion>(request.Filter);

        if (request.UsuarioID.HasValue)
        {
            var usuarioId = request.UsuarioID.Value;
            filtro = filtro is null
                ? n => n.UsuarioID == usuarioId
                : Filter.AndAlso(filtro, n => n.UsuarioID == usuarioId);
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            var tipo = request.Tipo.Trim().ToUpperInvariant();
            filtro = filtro is null
                ? n => n.Tipo == tipo
                : Filter.AndAlso(filtro, n => n.Tipo == tipo);
        }

        if (!string.IsNullOrWhiteSpace(request.Canal))
        {
            var canal = request.Canal.Trim().ToUpperInvariant();
            filtro = filtro is null
                ? n => n.Canal == canal
                : Filter.AndAlso(filtro, n => n.Canal == canal);
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            var estado = request.Estado.Trim().ToUpperInvariant();
            filtro = filtro is null
                ? n => n.Estado == estado
                : Filter.AndAlso(filtro, n => n.Estado == estado);
        }

        if (!string.IsNullOrWhiteSpace(request.OrigenServicio))
        {
            var origen = request.OrigenServicio.Trim().ToUpperInvariant();
            filtro = filtro is null
                ? n => n.OrigenServicio == origen
                : Filter.AndAlso(filtro, n => n.OrigenServicio == origen);
        }

        if (request.SoloActivas.HasValue)
        {
            var soloActivas = request.SoloActivas.Value;
            filtro = filtro is null
                ? n => n.Activo == soloActivas
                : Filter.AndAlso(filtro, n => n.Activo == soloActivas);
        }

        if (request.Desde.HasValue)
        {
            var desde = request.Desde.Value.Date;
            filtro = filtro is null
                ? n => n.FechaCreacion >= desde
                : Filter.AndAlso(filtro, n => n.FechaCreacion >= desde);
        }

        if (request.Hasta.HasValue)
        {
            var hasta = request.Hasta.Value.Date.AddDays(1);
            filtro = filtro is null
                ? n => n.FechaCreacion < hasta
                : Filter.AndAlso(filtro, n => n.FechaCreacion < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<Notificacion>(
                request.Buscar,
                "EventoID",
                "Titulo",
                "Mensaje",
                "Referencia");

            filtro = filtro is null ? textoLibre : Filter.AndAlso(filtro, textoLibre);
        }

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filtro,
            Filter.ParseOrderBy<Notificacion>(request.OrderBy ?? string.Empty),
            cancellationToken: cancellationToken);

        return new PagedDto<List<NotificacionDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(NotificationMapper.ToDto).ToList());
    }
}
