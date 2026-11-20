using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Indicaciones.Queries.GetIndicacionesByAtencion;

public class GetIndicacionesByAtencionQuery : IRequest<List<IndicacionMedicaDto>>
{
    public long AtencionID { get; set; }

    public GetIndicacionesByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetIndicacionesByAtencionQueryHandler : IRequestHandler<GetIndicacionesByAtencionQuery, List<IndicacionMedicaDto>>
{
    private readonly IIndicacionMedicaRepository _repository;

    public GetIndicacionesByAtencionQueryHandler(IIndicacionMedicaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<IndicacionMedicaDto>> Handle(GetIndicacionesByAtencionQuery request, CancellationToken cancellationToken)
    {
        var indicaciones = await _repository.GetAllAsync();
        return indicaciones
            .Where(i => i.AtencionID == request.AtencionID)
            .OrderByDescending(i => i.CreatedAt)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
