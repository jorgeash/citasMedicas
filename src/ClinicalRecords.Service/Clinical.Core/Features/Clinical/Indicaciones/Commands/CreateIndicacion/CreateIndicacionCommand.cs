using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Indicaciones.Commands.CreateIndicacion;

public class CreateIndicacionCommand : IRequest<IndicacionMedicaDto>
{
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class CreateIndicacionCommandHandler : IRequestHandler<CreateIndicacionCommand, IndicacionMedicaDto>
{
    private readonly IIndicacionMedicaRepository _repository;
    private readonly IAtencionRepository _atenciones;

    public CreateIndicacionCommandHandler(
        IIndicacionMedicaRepository repository,
        IAtencionRepository atenciones)
    {
        _repository = repository;
        _atenciones = atenciones;
    }

    public async Task<IndicacionMedicaDto> Handle(CreateIndicacionCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        if (string.IsNullOrWhiteSpace(request.Tipo) || string.IsNullOrWhiteSpace(request.Descripcion))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El tipo y la descripción son obligatorios.");

        var indicacion = new IndicacionMedica
        {
            AtencionID = request.AtencionID,
            MedicoID = request.MedicoID,
            Tipo = request.Tipo.Trim(),
            Descripcion = request.Descripcion.Trim(),
            Activa = true,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _repository.AddAsync(indicacion);
        return ClinicalMapper.ToDto(creada);
    }
}
