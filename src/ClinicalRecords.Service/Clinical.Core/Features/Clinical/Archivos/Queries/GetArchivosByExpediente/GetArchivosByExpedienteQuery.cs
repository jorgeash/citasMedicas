using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Archivos.Queries.GetArchivosByExpediente;

public class GetArchivosByExpedienteQuery : IRequest<List<ArchivoClinicoDto>>
{
    public int ExpedienteClinicoID { get; set; }

    public GetArchivosByExpedienteQuery(int expedienteClinicoID)
    {
        ExpedienteClinicoID = expedienteClinicoID;
    }
}

public class GetArchivosByExpedienteQueryHandler : IRequestHandler<GetArchivosByExpedienteQuery, List<ArchivoClinicoDto>>
{
    private readonly IArchivoClinicoRepository _repository;

    public GetArchivosByExpedienteQueryHandler(IArchivoClinicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ArchivoClinicoDto>> Handle(GetArchivosByExpedienteQuery request, CancellationToken cancellationToken)
    {
        var archivos = await _repository.GetAllAsync();
        return archivos
            .Where(a => a.ExpedienteClinicoID == request.ExpedienteClinicoID && a.Activo)
            .OrderByDescending(a => a.FechaCarga)
            .Select(ClinicalMapper.ToDto)
            .ToList();
    }
}
