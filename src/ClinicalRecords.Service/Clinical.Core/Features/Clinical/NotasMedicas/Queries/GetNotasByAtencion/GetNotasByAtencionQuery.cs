using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.NotasMedicas.Queries.GetNotasByAtencion;

public class GetNotasByAtencionQuery : IRequest<List<NotaMedicaDto>>
{
    public long AtencionID { get; set; }

    public GetNotasByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetNotasByAtencionQueryHandler : IRequestHandler<GetNotasByAtencionQuery, List<NotaMedicaDto>>
{
    private readonly INotaMedicaRepository _repository;

    public GetNotasByAtencionQueryHandler(INotaMedicaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<NotaMedicaDto>> Handle(GetNotasByAtencionQuery request, CancellationToken cancellationToken)
    {
        var notas = await _repository.GetAllAsync();
        return notas
            .Where(n => n.AtencionID == request.AtencionID)
            .OrderByDescending(n => n.FechaNota)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
