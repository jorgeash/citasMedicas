using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Atenciones.Queries.GetAtencionById;

public class GetAtencionByIdQuery : IRequest<AtencionDetalleDto?>
{
    public long AtencionID { get; set; }

    public GetAtencionByIdQuery(long atencionID)
    {
        AtencionID = atencionID;
    }
}

public class GetAtencionByIdQueryHandler : IRequestHandler<GetAtencionByIdQuery, AtencionDetalleDto?>
{
    private readonly IAtencionRepository _atenciones;
    private readonly IPrescripcionRepository _prescripciones;
    private readonly ITratamientoRepository _tratamientos;
    private readonly IMedicamentoRepository _medicamentos;

    public GetAtencionByIdQueryHandler(
        IAtencionRepository atenciones,
        IPrescripcionRepository prescripciones,
        ITratamientoRepository tratamientos,
        IMedicamentoRepository medicamentos)
    {
        _atenciones = atenciones;
        _prescripciones = prescripciones;
        _tratamientos = tratamientos;
        _medicamentos = medicamentos;
    }

    public async Task<AtencionDetalleDto?> Handle(GetAtencionByIdQuery request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            true,
            false,
            cancellationToken,
            a => a.SignosVitales,
            a => a.NotasMedicas,
            a => a.Diagnosticos,
            a => a.Sintomas,
            a => a.Prescripciones,
            a => a.Tratamientos,
            a => a.IndicacionesMedicas,
            a => a.ReferenciasMedicas,
            a => a.Procedimientos,
            a => a.ArchivosClinicos);

        if (atencion is null)
            return null;

        var dto = ClinicalMapper.ToDetalleDto(atencion);

        var prescripciones = await _prescripciones.GetAllAsync(p => p.Detalle);
        foreach (var prescripcion in dto.Prescripciones)
        {
            var completa = prescripciones.FirstOrDefault(p => p.PrescripcionID == prescripcion.PrescripcionID);
            if (completa is not null)
                prescripcion.Detalle = completa.Detalle.Select(ClinicalMapper.ToDto).ToList();
        }

        var tratamientos = await _tratamientos.GetAllAsync(t => t.Detalle);
        foreach (var tratamiento in dto.Tratamientos)
        {
            var completo = tratamientos.FirstOrDefault(t => t.TratamientoID == tratamiento.TratamientoID);
            if (completo is not null)
                tratamiento.Detalle = completo.Detalle.Select(ClinicalMapper.ToDto).ToList();
        }

        var medicamentos = (await _medicamentos.GetAllAsync())
            .ToDictionary(m => m.MedicamentoID);

        foreach (var detalle in dto.Prescripciones.SelectMany(p => p.Detalle))
            detalle.NombreMedicamento = medicamentos.TryGetValue(detalle.MedicamentoID, out var medicamento)
                ? medicamento.Nombre
                : string.Empty;

        return dto;
    }
}
