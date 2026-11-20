using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Archivos.Commands.AdjuntarArchivoClinico;

public class AdjuntarArchivoClinicoCommand : IRequest<ArchivoClinicoDto>
{
    public int ExpedienteClinicoID { get; set; }
    public long? AtencionID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? TamanoBytes { get; set; }
    public DateTime FechaCarga { get; set; }
}

public class AdjuntarArchivoClinicoCommandHandler : IRequestHandler<AdjuntarArchivoClinicoCommand, ArchivoClinicoDto>
{
    private readonly IArchivoClinicoRepository _repository;
    private readonly IExpedienteClinicoRepository _expedientes;
    private readonly IAtencionRepository _atenciones;

    public AdjuntarArchivoClinicoCommandHandler(
        IArchivoClinicoRepository repository,
        IExpedienteClinicoRepository expedientes,
        IAtencionRepository atenciones)
    {
        _repository = repository;
        _expedientes = expedientes;
        _atenciones = atenciones;
    }

    public async Task<ArchivoClinicoDto> Handle(AdjuntarArchivoClinicoCommand request, CancellationToken cancellationToken)
    {
        var expediente = await _expedientes.GetOneByAsync(
            e => e.ExpedienteClinicoID == request.ExpedienteClinicoID,
            cancellationToken: cancellationToken);

        if (expediente is null)
            throw new DomainException(Errores.EXPEDIENTE_NO_ENCONTRADO, $"No existe el expediente {request.ExpedienteClinicoID}.");

        if (request.AtencionID.HasValue)
        {
            var atencion = await _atenciones.GetOneByAsync(
                a => a.AtencionID == request.AtencionID.Value,
                cancellationToken: cancellationToken);

            if (atencion is null)
                throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID.Value}.");
        }

        if (string.IsNullOrWhiteSpace(request.NombreArchivo) || string.IsNullOrWhiteSpace(request.Ruta))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El nombre y la ruta del archivo son obligatorios.");

        var archivo = new ArchivoClinico
        {
            ExpedienteClinicoID = request.ExpedienteClinicoID,
            AtencionID = request.AtencionID,
            Tipo = request.Tipo,
            NombreArchivo = request.NombreArchivo.Trim(),
            Ruta = request.Ruta.Trim(),
            ContentType = request.ContentType,
            TamanoBytes = request.TamanoBytes,
            FechaCarga = request.FechaCarga == default ? DateTime.UtcNow : request.FechaCarga,
            Activo = true,
            CreatedAt = DateTime.UtcNow
        };

        var creado = await _repository.AddAsync(archivo);
        return ClinicalMapper.ToDto(creado);
    }
}
