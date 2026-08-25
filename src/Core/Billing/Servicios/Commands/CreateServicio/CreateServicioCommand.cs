using Domain.Billing.Entities;
using MediatR;

namespace Core.Billing.Servicios.Commands.CreateServicio;

public record CreateServicioCommand(
    string Codigo,
    string Nombre,
    string TipoServicio,
    string? Descripcion,
    bool Activo
) : IRequest<Servicio>;