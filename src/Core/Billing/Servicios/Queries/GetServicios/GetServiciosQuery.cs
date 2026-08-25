using Domain.Billing.Entities;
using MediatR;

namespace Core.Billing.Servicios.Queries.GetServicios;

public record GetServiciosQuery : IRequest<IEnumerable<Servicio>>;