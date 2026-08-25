using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Billing.Pacientes.Queries.GetPacienteById;

public class GetPacienteByIdHandler : IRequestHandler<GetPacienteByIdQuery, Paciente?>
{
    private readonly IPacienteRepository _repository;

    public GetPacienteByIdHandler(IPacienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Paciente?> Handle(
        GetPacienteByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id);
    }
}