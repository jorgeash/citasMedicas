using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Sintomas.Queries.GetSintomasByAtencion;

public class GetSintomasByAtencionQuery : IRequest<List<AtencionSintomaDto>>
{
    public long AtencionID { get; set; }

    public GetSintomasByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetSintomasByAtencionQueryHandler : IRequestHandler<GetSintomasByAtencionQuery, List<AtencionSintomaDto>>
{
    private readonly IAtencionSintomaRepository _repository;

    public GetSintomasByAtencionQueryHandler(IAtencionSintomaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AtencionSintomaDto>> Handle(GetSintomasByAtencionQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(s => s.Sintoma);
        return items
            .Where(s => s.AtencionID == request.AtencionID)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
