using Domain.Billing.Entities;
using MediatR;

namespace Core.Billing.Servicios.Queries.GetServicioById;

public record GetServicioByIdQuery(int Id) : IRequest<Servicio?>;