using Domain.Billing.Entities;
using MediatR;

namespace Core.Billing.Pacientes.Commands.CreatePaciente;

public record CreatePacienteCommand(Paciente Paciente) : IRequest<Paciente>;