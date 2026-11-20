using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Procedimientos.Commands.RegistrarProcedimiento;

public class RegistrarProcedimientoCommand : IRequest<AtencionProcedimientoDto>
{
    public long AtencionID { get; set; }
    public int ProcedimientoID { get; set; }
    public DateTime FechaRealizacion { get; set; }
    public int Cantidad { get; set; }
    public string? Observaciones { get; set; }
}

public class RegistrarProcedimientoCommandHandler : IRequestHandler<RegistrarProcedimientoCommand, AtencionProcedimientoDto>
{
    private readonly IAtencionProcedimientoRepository _repository;
    private readonly IAtencionRepository _atenciones;
    private readonly IProcedimientoRepository _catalogo;

    public RegistrarProcedimientoCommandHandler(
        IAtencionProcedimientoRepository repository,
        IAtencionRepository atenciones,
        IProcedimientoRepository catalogo)
    {
        _repository = repository;
        _atenciones = atenciones;
        _catalogo = catalogo;
    }

    public async Task<AtencionProcedimientoDto> Handle(RegistrarProcedimientoCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        var procedimiento = await _catalogo.GetByIdAsync(request.ProcedimientoID);
        if (procedimiento is null || !procedimiento.Activo)
            throw new DomainException(Errores.PROCEDIMIENTO_NO_VALIDO, $"El procedimiento {request.ProcedimientoID} no existe o no está activo.");

        if (request.Cantidad <= 0)
            throw new DomainException(Errores.PROCEDIMIENTO_CANTIDAD_INVALIDA, "La cantidad debe ser mayor que cero.");

        var registrado = new AtencionProcedimiento
        {
            AtencionID = request.AtencionID,
            ProcedimientoID = request.ProcedimientoID,
            FechaRealizacion = request.FechaRealizacion == default ? DateTime.UtcNow : request.FechaRealizacion,
            Cantidad = request.Cantidad,
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow,
            Procedimiento = procedimiento
        };

        var creado = await _repository.AddAsync(registrado);
        return ClinicalMapper.ToDto(creado);
    }
}
