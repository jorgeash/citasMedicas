using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Tratamientos.Queries.GetTratamientosByAtencion;

public class GetTratamientosByAtencionQuery : IRequest<List<TratamientoDto>>
{
    public long AtencionID { get; set; }

    public GetTratamientosByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetTratamientosByAtencionQueryHandler : IRequestHandler<GetTratamientosByAtencionQuery, List<TratamientoDto>>
{
    private readonly ITratamientoRepository _repository;

    public GetTratamientosByAtencionQueryHandler(ITratamientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TratamientoDto>> Handle(GetTratamientosByAtencionQuery request, CancellationToken cancellationToken)
    {
        var tratamientos = await _repository.GetAllAsync(t => t.Detalle);
        return tratamientos
            .Where(t => t.AtencionID == request.AtencionID)
            .OrderByDescending(t => t.FechaInicio)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
