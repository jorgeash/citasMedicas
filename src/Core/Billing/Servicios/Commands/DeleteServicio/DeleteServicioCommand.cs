using MediatR;

namespace Core.Billing.Servicios.Commands.DeleteServicio;

public record DeleteServicioCommand(int Id) : IRequest<bool>;
