using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Hospitalizacion.Commands.IngresarPaciente;

public class IngresarPacienteCommand : IRequest<HospitalizacionDto>
{
    public int ExpedienteClinicoID { get; set; }
    public long AtencionID { get; set; }
    public int CamaID { get; set; }
    public DateTime FechaIngreso { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Observaciones { get; set; }
}

public class IngresarPacienteCommandHandler : IRequestHandler<IngresarPacienteCommand, HospitalizacionDto>
{
    private readonly IHospitalizacionRepository _repository;
    private readonly IExpedienteClinicoRepository _expedientes;
    private readonly IAtencionRepository _atenciones;
    private readonly ICamaRepository _camas;

    public IngresarPacienteCommandHandler(
        IHospitalizacionRepository repository,
        IExpedienteClinicoRepository expedientes,
        IAtencionRepository atenciones,
        ICamaRepository camas)
    {
        _repository = repository;
        _expedientes = expedientes;
        _atenciones = atenciones;
        _camas = camas;
    }

    public async Task<HospitalizacionDto> Handle(IngresarPacienteCommand request, CancellationToken cancellationToken)
    {
        var expediente = await _expedientes.GetOneByAsync(
            e => e.ExpedienteClinicoID == request.ExpedienteClinicoID,
            cancellationToken: cancellationToken);

        if (expediente is null)
            throw new DomainException(Errores.EXPEDIENTE_NO_ENCONTRADO, $"No existe el expediente {request.ExpedienteClinicoID}.");

        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        var cama = await _camas.GetByIdAsync(request.CamaID);
        if (cama is null)
            throw new DomainException(Errores.CAMA_NO_ENCONTRADA, $"No existe la cama {request.CamaID}.");

        if (!cama.Activa || !string.Equals(cama.Estado, "LIBRE", StringComparison.OrdinalIgnoreCase))
            throw new DomainException(Errores.CAMA_NO_DISPONIBLE, $"La cama {request.CamaID} no está disponible.");

        var activa = await _repository.GetOneByAsync(
            h => h.ExpedienteClinicoID == request.ExpedienteClinicoID && h.Estado == "ACTIVA",
            cancellationToken: cancellationToken);

        if (activa is not null)
            throw new DomainException(Errores.HOSPITALIZACION_ACTIVA_PREVIA, $"El expediente {request.ExpedienteClinicoID} ya tiene una hospitalización activa.");

        if (string.IsNullOrWhiteSpace(request.Motivo))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El motivo de la hospitalización es obligatorio.");

        var hospitalizacion = new global::Clinical.Domain.Entities.Hospitalizacion
        {
            ExpedienteClinicoID = request.ExpedienteClinicoID,
            AtencionID = request.AtencionID,
            CamaID = request.CamaID,
            FechaIngreso = request.FechaIngreso == default ? DateTime.UtcNow : request.FechaIngreso,
            Motivo = request.Motivo.Trim(),
            Diagnostico = request.Diagnostico,
            Estado = "ACTIVA",
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _repository.AddAsync(hospitalizacion);

        cama.Estado = "OCUPADA";
        await _camas.UpdateAsync(cama);

        return ClinicalMapper.ToDto(creada);
    }
}
