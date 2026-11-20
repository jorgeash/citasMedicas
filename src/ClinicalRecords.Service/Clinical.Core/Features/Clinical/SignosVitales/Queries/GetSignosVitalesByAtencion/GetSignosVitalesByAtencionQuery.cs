using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.SignosVitales.Queries.GetSignosVitalesByAtencion;

public class GetSignosVitalesByAtencionQuery : IRequest<List<SignosVitalesDto>>
{
    public long AtencionID { get; set; }

    public GetSignosVitalesByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetSignosVitalesByAtencionQueryHandler : IRequestHandler<GetSignosVitalesByAtencionQuery, List<SignosVitalesDto>>
{
    private readonly ISignosVitalesRepository _repository;

    public GetSignosVitalesByAtencionQueryHandler(ISignosVitalesRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SignosVitalesDto>> Handle(GetSignosVitalesByAtencionQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync();
        return items
            .Where(s => s.AtencionID == request.AtencionID)
            .OrderByDescending(s => s.FechaMedicion)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
