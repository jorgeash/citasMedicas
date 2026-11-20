using System.Linq.Expressions;
using Lab.Core.DTOs.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;

namespace Lab.Core.Features.Lab.SolicitudesEstudio.Queries.GetSolicitudEstudioById;

public class GetSolicitudEstudioByIdQuery : IRequest<SolicitudEstudioDto?>
{
    public int SolicitudEstudioID { get; set; }

    public GetSolicitudEstudioByIdQuery(int solicitudEstudioId)
    {
        SolicitudEstudioID = solicitudEstudioId;
    }
}

public class GetSolicitudEstudioByIdQueryHandler : IRequestHandler<GetSolicitudEstudioByIdQuery, SolicitudEstudioDto?>
{
    private readonly ISolicitudEstudioRepository _repository;

    public GetSolicitudEstudioByIdQueryHandler(ISolicitudEstudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<SolicitudEstudioDto?> Handle(GetSolicitudEstudioByIdQuery request, CancellationToken cancellationToken)
    {
        var solicitud = await _repository.GetOneByAsync(
            s => s.SolicitudEstudioID == request.SolicitudEstudioID,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<SolicitudEstudio, object>>[]
            {
                s => s.Detalle,
                s => s.Resultados
            });

        return solicitud is null ? null : LabMapper.ToDto(solicitud);
    }
}
