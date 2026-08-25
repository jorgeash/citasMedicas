using Domain.Billing.Entities;
using MediatR;

namespace Core.Billing.Servicios.Commands.UpdateServicio;

public record UpdateServicioCommand(
    int ServicioID,
    string Codigo,
    string Nombre,
    string TipoServicio,
    string? Descripcion,
    bool Activo
) : IRequest<Servicio?>;