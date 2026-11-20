using System.Linq.Expressions;
using Lab.Core.DTOs.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;

namespace Lab.Core.Features.Lab.ResultadosEstudio.Queries.GetResultadosBySolicitud;

public class GetResultadosBySolicitudQuery : IRequest<List<ResultadoEstudioDto>>
{
    public int SolicitudEstudioID { get; set; }

    public GetResultadosBySolicitudQuery(int solicitudEstudioId)
    {
        SolicitudEstudioID = solicitudEstudioId;
    }
}

public class GetResultadosBySolicitudQueryHandler : IRequestHandler<GetResultadosBySolicitudQuery, List<ResultadoEstudioDto>>
{
    private readonly IResultadoEstudioRepository _repository;

    public GetResultadosBySolicitudQueryHandler(IResultadoEstudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ResultadoEstudioDto>> Handle(
        GetResultadosBySolicitudQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetPagedAsync(
            1,
            100,
            r => r.SolicitudEstudioID == request.SolicitudEstudioID,
            q => q.OrderByDescending(r => r.FechaResultado),
            cancellationToken: cancellationToken,
            includes: new Expression<Func<ResultadoEstudio, object>>[]
            {
                r => r.SolicitudEstudio!
            });

        return result.Data.Select(LabMapper.ToDto).ToList();
    }
}
