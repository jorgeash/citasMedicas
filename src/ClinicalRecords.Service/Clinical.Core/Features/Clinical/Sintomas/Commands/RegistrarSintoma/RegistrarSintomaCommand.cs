using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.Sintomas.Commands.RegistrarSintoma;

public class RegistrarSintomaCommand : IRequest<AtencionSintomaDto>
{
    public long AtencionID { get; set; }
    public int SintomaID { get; set; }
    public string Severidad { get; set; } = string.Empty;
    public int? DuracionDias { get; set; }
    public string? Observaciones { get; set; }
}

public class RegistrarSintomaCommandHandler : IRequestHandler<RegistrarSintomaCommand, AtencionSintomaDto>
{
    private readonly IAtencionSintomaRepository _repository;
    private readonly IAtencionRepository _atenciones;
    private readonly ISintomaRepository _catalogo;

    public RegistrarSintomaCommandHandler(
        IAtencionSintomaRepository repository,
        IAtencionRepository atenciones,
        ISintomaRepository catalogo)
    {
        _repository = repository;
        _atenciones = atenciones;
        _catalogo = catalogo;
    }

    public async Task<AtencionSintomaDto> Handle(RegistrarSintomaCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        var sintoma = await _catalogo.GetByIdAsync(request.SintomaID);
        if (sintoma is null || !sintoma.Activo)
            throw new DomainException(Errores.SINTOMA_NO_ENCONTRADO, $"El síntoma {request.SintomaID} no existe o no está activo.");

        if (string.IsNullOrWhiteSpace(request.Severidad))
            throw new DomainException(Errores.DATOS_INVALIDOS, "La severidad es obligatoria.");

        var registrado = new AtencionSintoma
        {
            AtencionID = request.AtencionID,
            SintomaID = request.SintomaID,
            Severidad = request.Severidad.Trim(),
            DuracionDias = request.DuracionDias,
            Observaciones = request.Observaciones,
            CreatedAt = DateTime.UtcNow,
            Sintoma = sintoma
        };

        var creado = await _repository.AddAsync(registrado);
        return ClinicalMapper.ToDto(creado);
    }
}
