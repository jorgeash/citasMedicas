using MediatR;
using Scheduling.Core.DTOs;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;

namespace Scheduling.Core.Features.Scheduling.EstadosCita.Queries.GetEstadosCita;

/// <summary>Catálogo pequeño y fijo: se devuelve completo, sin paginar.</summary>
public class GetEstadosCitaQuery : IRequest<HttpResponse<List<EstadoCitaDto>>>
{
}

public class GetEstadosCitaQueryHandler : IRequestHandler<GetEstadosCitaQuery, HttpResponse<List<EstadoCitaDto>>>
{
    private readonly IEstadoCitaRepository _estadoCitaRepository;

    public GetEstadosCitaQueryHandler(IEstadoCitaRepository estadoCitaRepository)
    {
        _estadoCitaRepository = estadoCitaRepository;
    }

    public async Task<HttpResponse<List<EstadoCitaDto>>> Handle(
        GetEstadosCitaQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _estadoCitaRepository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 100,
            orderBy: query => query.OrderBy(e => e.EstadoCitaID),
            cancellationToken: cancellationToken);

        return new HttpResponse<List<EstadoCitaDto>>(
            result.Data.Select(SchedulingMapper.ToDto).ToList());
    }
}
