using Domain.Billing.Entities;
using MediatR;

namespace Core.Billing.Pacientes.Queries.GetPacienteById;

public record GetPacienteByIdQuery(long Id) : IRequest<Paciente?>;
