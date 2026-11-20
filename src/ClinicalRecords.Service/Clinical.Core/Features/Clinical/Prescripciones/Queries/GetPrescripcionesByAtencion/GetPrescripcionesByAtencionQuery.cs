using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Prescripciones.Queries.GetPrescripcionesByAtencion;

public class GetPrescripcionesByAtencionQuery : IRequest<List<PrescripcionDto>>
{
    public long AtencionID { get; set; }

    public GetPrescripcionesByAtencionQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetPrescripcionesByAtencionQueryHandler : IRequestHandler<GetPrescripcionesByAtencionQuery, List<PrescripcionDto>>
{
    private readonly IPrescripcionRepository _repository;
    private readonly IMedicamentoRepository _medicamentos;

    public GetPrescripcionesByAtencionQueryHandler(
        IPrescripcionRepository repository,
        IMedicamentoRepository medicamentos)
    {
        _repository = repository;
        _medicamentos = medicamentos;
    }

    public async Task<List<PrescripcionDto>> Handle(GetPrescripcionesByAtencionQuery request, CancellationToken cancellationToken)
    {
        var prescripciones = await _repository.GetAllAsync(p => p.Detalle);
        var medicamentos = (await _medicamentos.GetAllAsync()).ToDictionary(m => m.MedicamentoID);

        return prescripciones
            .Where(p => p.AtencionID == request.AtencionID)
            .OrderByDescending(p => p.FechaPrescripcion)
            .Select(p =>
            {
                var dto = ClinicalMapper.ToDto(p);
                foreach (var detalle in dto.Detalle)
                {
                    detalle.NombreMedicamento = medicamentos.TryGetValue(detalle.MedicamentoID, out var medicamento)
                        ? medicamento.Nombre
                        : string.Empty;
                }

                return dto;
            })
            .ToList();
    }
}
