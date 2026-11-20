using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Referencias.Queries.GetReferenciasByAtencion;

public class GetReferenciasByAtencionQuery : IRequest<List<ReferenciaMedicaDto>>
{
    public long AtencionID { get; set; }

    public GetReferenciasByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetReferenciasByAtencionQueryHandler : IRequestHandler<GetReferenciasByAtencionQuery, List<ReferenciaMedicaDto>>
{
    private readonly IReferenciaMedicaRepository _repository;

    public GetReferenciasByAtencionQueryHandler(IReferenciaMedicaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ReferenciaMedicaDto>> Handle(GetReferenciasByAtencionQuery request, CancellationToken cancellationToken)
    {
        var referencias = await _repository.GetAllAsync();
        return referencias
            .Where(r => r.AtencionID == request.AtencionID)
            .OrderByDescending(r => r.FechaReferencia)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
