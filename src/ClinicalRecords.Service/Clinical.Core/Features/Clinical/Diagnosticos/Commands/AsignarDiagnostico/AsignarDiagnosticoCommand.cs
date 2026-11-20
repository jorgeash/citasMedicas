using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Diagnosticos.Commands.AsignarDiagnostico;

public class AsignarDiagnosticoCommand : IRequest<AtencionDiagnosticoDto>
{
    public long AtencionID { get; set; }
    public int DiagnosticoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}

public class AsignarDiagnosticoCommandHandler : IRequestHandler<AsignarDiagnosticoCommand, AtencionDiagnosticoDto>
{
    private readonly IAtencionDiagnosticoRepository _repository;
    private readonly IAtencionRepository _atenciones;
    private readonly IDiagnosticoRepository _catalogo;

    public AsignarDiagnosticoCommandHandler(
        IAtencionDiagnosticoRepository repository,
        IAtencionRepository atenciones,
        IDiagnosticoRepository catalogo)
    {
        _repository = repository;
        _atenciones = atenciones;
        _catalogo = catalogo;
    }

    public async Task<AtencionDiagnosticoDto> Handle(AsignarDiagnosticoCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        var diagnostico = await _catalogo.GetByIdAsync(request.DiagnosticoID);
        if (diagnostico is null || !diagnostico.Activo)
            throw new DomainException(Errores.DIAGNOSTICO_NO_ENCONTRADO, $"El diagnóstico {request.DiagnosticoID} no existe o no está activo.");

        if (string.IsNullOrWhiteSpace(request.Tipo))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El tipo de diagnóstico es obligatorio.");

        var tipo = request.Tipo.Trim().ToUpperInvariant();

        var duplicado = await _repository.GetOneByAsync(
            d => d.AtencionID == request.AtencionID && d.DiagnosticoID == request.DiagnosticoID,
            cancellationToken: cancellationToken);

        if (duplicado is not null)
            throw new DomainException(Errores.DIAGNOSTICO_DUPLICADO, $"El diagnóstico {request.DiagnosticoID} ya está asignado a la atención.");

        if (tipo == "PRINCIPAL")
        {
            var principal = await _repository.GetOneByAsync(
                d => d.AtencionID == request.AtencionID && d.Tipo == "PRINCIPAL",
                cancellationToken: cancellationToken);

            if (principal is not null)
                throw new DomainException(Errores.DIAGNOSTICO_PRINCIPAL_DUPLICADO, $"La atención {request.AtencionID} ya tiene un diagnóstico principal.");
        }

        var asignado = new AtencionDiagnostico
        {
            AtencionID = request.AtencionID,
            DiagnosticoID = request.DiagnosticoID,
            Tipo = tipo,
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow,
            Diagnostico = diagnostico
        };

        var creado = await _repository.AddAsync(asignado);
        return ClinicalMapper.ToDto(creado);
    }
}
