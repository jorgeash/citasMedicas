using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Procedimientos.Queries.GetProcedimientosByAtencion;

public class GetProcedimientosByAtencionQuery : IRequest<List<AtencionProcedimientoDto>>
{
    public long AtencionID { get; set; }

    public GetProcedimientosByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetProcedimientosByAtencionQueryHandler : IRequestHandler<GetProcedimientosByAtencionQuery, List<AtencionProcedimientoDto>>
{
    private readonly IAtencionProcedimientoRepository _repository;

    public GetProcedimientosByAtencionQueryHandler(IAtencionProcedimientoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AtencionProcedimientoDto>> Handle(GetProcedimientosByAtencionQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(p => p.Procedimiento);
        return items
            .Where(p => p.AtencionID == request.AtencionID)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
