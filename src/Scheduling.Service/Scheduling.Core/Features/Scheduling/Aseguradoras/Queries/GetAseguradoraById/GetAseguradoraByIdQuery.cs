using MediatR;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;

namespace Scheduling.Core.Features.Scheduling.Aseguradoras.Queries.GetAseguradoraById;

public class GetAseguradoraByIdQuery : IRequest<AseguradoraDto?>
{
    public int AseguradoraID { get; set; }

    public GetAseguradoraByIdQuery(int aseguradoraID)
    {
        AseguradoraID = aseguradoraID;
    }
}

public class GetAseguradoraByIdQueryHandler : IRequestHandler<GetAseguradoraByIdQuery, AseguradoraDto?>
{
    private readonly IAseguradoraRepository _aseguradoraRepository;

    public GetAseguradoraByIdQueryHandler(IAseguradoraRepository aseguradoraRepository)
    {
        _aseguradoraRepository = aseguradoraRepository;
    }

    public async Task<AseguradoraDto?> Handle(GetAseguradoraByIdQuery request, CancellationToken cancellationToken)
    {
        var aseguradora = await _aseguradoraRepository.GetOneByAsync(
            a => a.AseguradoraID == request.AseguradoraID,
            cancellationToken: cancellationToken);

        return aseguradora is null ? null : SchedulingMapper.ToDto(aseguradora);
    }
}
