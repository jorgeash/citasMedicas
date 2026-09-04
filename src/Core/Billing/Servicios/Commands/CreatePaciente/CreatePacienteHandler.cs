using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Billing.Pacientes.Commands.CreatePaciente;

public class CreatePacienteHandler : IRequestHandler<CreatePacienteCommand, Paciente>
{
    private readonly IPacienteRepository _repository;

    public CreatePacienteHandler(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Paciente> Handle(
        CreatePacienteCommand request,
        CancellationToken cancellationToken)
    {
        return await _repository.CreateAsync(request.Paciente, cancellationToken);
    }
}