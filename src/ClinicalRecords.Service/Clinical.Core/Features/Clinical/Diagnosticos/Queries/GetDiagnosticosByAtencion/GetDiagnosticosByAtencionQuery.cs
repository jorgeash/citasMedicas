using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Diagnosticos.Queries.GetDiagnosticosByAtencion;

public class GetDiagnosticosByAtencionQuery : IRequest<List<AtencionDiagnosticoDto>>
{
    public long AtencionID { get; set; }

    public GetDiagnosticosByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetDiagnosticosByAtencionQueryHandler : IRequestHandler<GetDiagnosticosByAtencionQuery, List<AtencionDiagnosticoDto>>
{
    private readonly IAtencionDiagnosticoRepository _repository;

    public GetDiagnosticosByAtencionQueryHandler(IAtencionDiagnosticoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AtencionDiagnosticoDto>> Handle(GetDiagnosticosByAtencionQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(d => d.Diagnostico);
        return items
            .Where(d => d.AtencionID == request.AtencionID)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
