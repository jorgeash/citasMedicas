using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Expedientes.Queries.GetExpedienteById;

public class GetExpedienteByIdQuery : IRequest<ExpedienteDetalleDto?>
{
    public int ExpedienteClinicoID { get; set; }

    public GetExpedienteByIdQuery(int expedienteClinicoID)
    {
        ExpedienteClinicoID = expedienteClinicoID;
    }
}

public class GetExpedienteByIdQueryHandler : IRequestHandler<GetExpedienteByIdQuery, ExpedienteDetalleDto?>
{
    private readonly IExpedienteClinicoRepository _repository;

    public GetExpedienteByIdQueryHandler(IExpedienteClinicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExpedienteDetalleDto?> Handle(GetExpedienteByIdQuery request, CancellationToken cancellationToken)
    {
        var expediente = await _repository.GetOneByAsync(
            e => e.ExpedienteClinicoID == request.ExpedienteClinicoID,
            true,
            false,
            cancellationToken,
            e => e.AntecedentesMedicos,
            e => e.AntecedentesFamiliares,
            e => e.Alergias,
            e => e.Habitos);

        return expediente is null ? null : ClinicalMapper.ToDetalleDto(expediente);
    }
}
