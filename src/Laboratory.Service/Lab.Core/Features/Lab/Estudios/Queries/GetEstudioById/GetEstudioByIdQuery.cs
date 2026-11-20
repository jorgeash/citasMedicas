using Lab.Core.DTOs.Lab;
using Lab.Core.Interfaces.Repositories;
using MediatR;

namespace Lab.Core.Features.Lab.Estudios.Queries.GetEstudioById;

public class GetEstudioByIdQuery : IRequest<EstudioDto?>
{
    public int EstudioID { get; set; }

    public GetEstudioByIdQuery(int estudioId)
    {
        EstudioID = estudioId;
    }
}

public class GetEstudioByIdQueryHandler : IRequestHandler<GetEstudioByIdQuery, EstudioDto?>
{
    private readonly IEstudioRepository _repository;

    public GetEstudioByIdQueryHandler(IEstudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<EstudioDto?> Handle(GetEstudioByIdQuery request, CancellationToken cancellationToken)
    {
        var estudio = await _repository.GetByIdAsync(request.EstudioID);
        return estudio is null ? null : LabMapper.ToDto(estudio);
    }
}
