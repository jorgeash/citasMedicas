using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using MediatR;

namespace Clinical.Core.Features.Clinical.Hospitalizacion.Commands.EgresarPaciente;

public class EgresarPacienteCommand : IRequest<HospitalizacionDto>
{
    public long HospitalizacionID { get; set; }
    public DateTime FechaEgreso { get; set; }
    public string? Observaciones { get; set; }
}

public class EgresarPacienteCommandHandler : IRequestHandler<EgresarPacienteCommand, HospitalizacionDto>
{
    private readonly IHospitalizacionRepository _repository;
    private readonly ICamaRepository _camas;

    public EgresarPacienteCommandHandler(
        IHospitalizacionRepository repository,
        ICamaRepository camas)
    {
        _repository = repository;
        _camas = camas;
    }

    public async Task<HospitalizacionDto> Handle(EgresarPacienteCommand request, CancellationToken cancellationToken)
    {
        var hospitalizacion = await _repository.GetOneByAsync(
            h => h.HospitalizacionID == request.HospitalizacionID,
            cancellationToken: cancellationToken);

        if (hospitalizacion is null)
            throw new DomainException(Errores.HOSPITALIZACION_NO_ENCONTRADA, $"No existe la hospitalización {request.HospitalizacionID}.");

        if (hospitalizacion.Estado != "ACTIVA")
            throw new DomainException(Errores.HOSPITALIZACION_NO_ACTIVA, $"La hospitalización {request.HospitalizacionID} no está activa.");

        var fechaEgreso = request.FechaEgreso == default ? DateTime.UtcNow : request.FechaEgreso;
        if (fechaEgreso < hospitalizacion.FechaIngreso)
            throw new DomainException(Errores.FECHA_EGRESO_INVALIDA, "La fecha de egreso no puede ser anterior a la fecha de ingreso.");

        hospitalizacion.FechaEgreso = fechaEgreso;
        hospitalizacion.Estado = "EGRESADA";
        hospitalizacion.UpdatedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(request.Observaciones))
            hospitalizacion.Observaciones = request.Observaciones.Trim();

        await _repository.UpdateAsync(hospitalizacion);

        var cama = await _camas.GetByIdAsync(hospitalizacion.CamaID);
        if (cama is not null)
        {
            cama.Estado = "LIBRE";
            await _camas.UpdateAsync(cama);
        }

        return ClinicalMapper.ToDto(hospitalizacion);
    }
}
