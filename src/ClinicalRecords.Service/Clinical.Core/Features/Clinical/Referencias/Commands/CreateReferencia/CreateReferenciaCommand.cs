using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Referencias.Commands.CreateReferencia;

public class CreateReferenciaCommand : IRequest<ReferenciaMedicaDto>
{
    public long AtencionID { get; set; }
    public int EspecialidadID { get; set; }
    public int? MedicoDestinoID { get; set; }
    public DateTime FechaReferencia { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}

public class CreateReferenciaCommandHandler : IRequestHandler<CreateReferenciaCommand, ReferenciaMedicaDto>
{
    private readonly IReferenciaMedicaRepository _repository;
    private readonly IAtencionRepository _atenciones;

    public CreateReferenciaCommandHandler(
        IReferenciaMedicaRepository repository,
        IAtencionRepository atenciones)
    {
        _repository = repository;
        _atenciones = atenciones;
    }

    public async Task<ReferenciaMedicaDto> Handle(CreateReferenciaCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        if (request.EspecialidadID <= 0 || string.IsNullOrWhiteSpace(request.Motivo))
            throw new DomainException(Errores.DATOS_INVALIDOS, "La especialidad y el motivo son obligatorios.");

        var referencia = new ReferenciaMedica
        {
            AtencionID = request.AtencionID,
            EspecialidadID = request.EspecialidadID,
            MedicoDestinoID = request.MedicoDestinoID,
            FechaReferencia = request.FechaReferencia == default ? DateTime.UtcNow : request.FechaReferencia,
            Motivo = request.Motivo.Trim(),
            Estado = string.IsNullOrWhiteSpace(request.Estado) ? "PENDIENTE" : request.Estado.Trim().ToUpperInvariant(),
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _repository.AddAsync(referencia);
        return ClinicalMapper.ToDto(creada);
    }
}
