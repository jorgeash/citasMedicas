using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Expedientes.Commands.CreateExpediente;

public class CreateExpedienteCommand : IRequest<ExpedienteDto>
{
    public long PacienteID { get; set; }
    public DateOnly FechaApertura { get; set; }
    public string? Observaciones { get; set; }
}

public class CreateExpedienteCommandHandler : IRequestHandler<CreateExpedienteCommand, ExpedienteDto>
{
    private readonly IExpedienteClinicoRepository _repository;

    public CreateExpedienteCommandHandler(IExpedienteClinicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<ExpedienteDto> Handle(CreateExpedienteCommand request, CancellationToken cancellationToken)
    {
        if (request.PacienteID <= 0)
            throw new DomainException(Errores.DATOS_INVALIDOS, "El paciente es obligatorio.");

        var existente = await _repository.GetOneByAsync(
            e => e.PacienteID == request.PacienteID && e.Activo,
            cancellationToken: cancellationToken);

        if (existente is not null)
            throw new DomainException(Errores.EXPEDIENTE_DUPLICADO, $"Ya existe un expediente activo para el paciente {request.PacienteID}.");

        var fechaApertura = request.FechaApertura == default ? DateOnly.FromDateTime(DateTime.UtcNow) : request.FechaApertura;

        var expediente = new ExpedienteClinico
        {
            PacienteID = request.PacienteID,
            NumeroExpediente = $"TMP-{Guid.NewGuid():N}".Substring(0, 20),
            FechaApertura = fechaApertura,
            Observaciones = request.Observaciones,
            Activo = true,
            CreatedAt = DateTime.UtcNow
        };

        var creado = await _repository.AddAsync(expediente);
        creado.NumeroExpediente = $"EXP-{creado.FechaApertura:yyyy}-{creado.ExpedienteClinicoID:D6}";
        await _repository.UpdateAsync(creado);

        return ClinicalMapper.ToDto(creado);
    }
}
