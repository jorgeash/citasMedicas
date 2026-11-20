using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Expedientes.Queries.GetExpedienteByPaciente;

public class GetExpedienteByPacienteQuery : IRequest<ExpedienteDto?>
{
    public long PacienteID { get; set; }

    public GetExpedienteByPacienteQuery(long pacienteID)
    {
        PacienteID = pacienteID;
    }
}

public class GetExpedienteByPacienteQueryHandler : IRequestHandler<GetExpedienteByPacienteQuery, ExpedienteDto?>
{
    private readonly IExpedienteClinicoRepository _repository;

    public GetExpedienteByPacienteQueryHandler(IExpedienteClinicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExpedienteDto?> Handle(GetExpedienteByPacienteQuery request, CancellationToken cancellationToken)
    {
        var expediente = await _repository.GetOneByAsync(
            e => e.PacienteID == request.PacienteID && e.Activo,
            cancellationToken: cancellationToken);

        return expediente is null ? null : ClinicalMapper.ToDto(expediente);
    }
}
