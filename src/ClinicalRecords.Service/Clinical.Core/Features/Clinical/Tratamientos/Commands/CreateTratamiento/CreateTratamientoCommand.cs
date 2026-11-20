using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Tratamientos.Commands.CreateTratamiento;

public class CreateTratamientoCommand : IRequest<TratamientoDto>
{
    public long AtencionID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Objetivo { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string? Observaciones { get; set; }
    public List<CreateTratamientoDetalle> Detalle { get; set; } = new();
}

public class CreateTratamientoDetalle
{
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Frecuencia { get; set; }
    public string? Observaciones { get; set; }
}

public class CreateTratamientoCommandHandler : IRequestHandler<CreateTratamientoCommand, TratamientoDto>
{
    private readonly ITratamientoRepository _repository;
    private readonly IAtencionRepository _atenciones;

    public CreateTratamientoCommandHandler(
        ITratamientoRepository repository,
        IAtencionRepository atenciones)
    {
        _repository = repository;
        _atenciones = atenciones;
    }

    public async Task<TratamientoDto> Handle(CreateTratamientoCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        if (request.Detalle is null || request.Detalle.Count == 0)
            throw new DomainException(Errores.TRATAMIENTO_SIN_DETALLE, "El tratamiento debe tener al menos un detalle.");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El nombre del tratamiento es obligatorio.");

        var tratamiento = new Tratamiento
        {
            AtencionID = request.AtencionID,
            Nombre = request.Nombre.Trim(),
            Objetivo = request.Objetivo,
            FechaInicio = request.FechaInicio == default ? DateOnly.FromDateTime(DateTime.UtcNow) : request.FechaInicio,
            FechaFin = request.FechaFin,
            Estado = "ACTIVO",
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var item in request.Detalle)
        {
            if (string.IsNullOrWhiteSpace(item.Tipo) || string.IsNullOrWhiteSpace(item.Descripcion))
                throw new DomainException(Errores.DATOS_INVALIDOS, "El tipo y la descripción de cada detalle son obligatorios.");

            tratamiento.Detalle.Add(new TratamientoDetalle
            {
                Tipo = item.Tipo.Trim(),
                Descripcion = item.Descripcion.Trim(),
                Frecuencia = item.Frecuencia,
                Observaciones = item.Observaciones,
                CreatedAt = DateTime.UtcNow
            });
        }

        var creado = await _repository.AddAsync(tratamiento);
        return ClinicalMapper.ToDto(creado);
    }
}
