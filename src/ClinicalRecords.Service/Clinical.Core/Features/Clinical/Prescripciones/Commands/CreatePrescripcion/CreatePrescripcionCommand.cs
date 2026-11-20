using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Prescripciones.Commands.CreatePrescripcion;

public class CreatePrescripcionCommand : IRequest<PrescripcionDto>
{
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaPrescripcion { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string? Observaciones { get; set; }
    public List<CreatePrescripcionDetalle> Detalle { get; set; } = new();
}

public class CreatePrescripcionDetalle
{
    public int MedicamentoID { get; set; }
    public string Dosis { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public string Via { get; set; } = string.Empty;
    public int? DuracionDias { get; set; }
    public int? Cantidad { get; set; }
    public string? Observaciones { get; set; }
}

public class CreatePrescripcionCommandHandler : IRequestHandler<CreatePrescripcionCommand, PrescripcionDto>
{
    private readonly IPrescripcionRepository _repository;
    private readonly IAtencionRepository _atenciones;
    private readonly IMedicamentoRepository _medicamentos;

    public CreatePrescripcionCommandHandler(
        IPrescripcionRepository repository,
        IAtencionRepository atenciones,
        IMedicamentoRepository medicamentos)
    {
        _repository = repository;
        _atenciones = atenciones;
        _medicamentos = medicamentos;
    }

    public async Task<PrescripcionDto> Handle(CreatePrescripcionCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        if (request.Detalle is null || request.Detalle.Count == 0)
            throw new DomainException(Errores.PRESCRIPCION_SIN_DETALLE, "La prescripción debe tener al menos un detalle.");

        var prescripcion = new Prescripcion
        {
            AtencionID = request.AtencionID,
            MedicoID = request.MedicoID,
            FechaPrescripcion = request.FechaPrescripcion == default ? DateTime.UtcNow : request.FechaPrescripcion,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Estado = "ACTIVA",
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in request.Detalle)
        {
            var medicamento = await _medicamentos.GetByIdAsync(item.MedicamentoID);
            if (medicamento is null || !medicamento.Activo)
                throw new DomainException(Errores.MEDICAMENTO_NO_VALIDO, $"El medicamento {item.MedicamentoID} no existe o no está activo.");

            prescripcion.Detalle.Add(new PrescripcionDetalle
            {
                MedicamentoID = item.MedicamentoID,
                Dosis = item.Dosis,
                Frecuencia = item.Frecuencia,
                Via = item.Via,
                DuracionDias = item.DuracionDias,
                Cantidad = item.Cantidad,
                Observaciones = item.Observaciones,
                CreatedAt = DateTime.UtcNow,
                Medicamento = medicamento
            });
        }

        var creada = await _repository.AddAsync(prescripcion);
        return ClinicalMapper.ToDto(creada);
    }
}
