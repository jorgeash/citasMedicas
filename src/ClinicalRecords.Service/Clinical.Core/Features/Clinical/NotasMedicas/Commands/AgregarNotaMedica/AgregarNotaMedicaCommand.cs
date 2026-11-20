using Clinical.Core.DTOs.Clinical;
using Clinical.Core.Exceptions;
using Clinical.Core.Features.Clinical;
using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using MediatR;

namespace Clinical.Core.Features.Clinical.NotasMedicas.Commands.AgregarNotaMedica;

public class AgregarNotaMedicaCommand : IRequest<NotaMedicaDto>
{
    public long AtencionID { get; set; }
    public int MedicoID { get; set; }
    public DateTime FechaNota { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
}

public class AgregarNotaMedicaCommandHandler : IRequestHandler<AgregarNotaMedicaCommand, NotaMedicaDto>
{
    private readonly INotaMedicaRepository _repository;
    private readonly IAtencionRepository _atenciones;

    public AgregarNotaMedicaCommandHandler(
        INotaMedicaRepository repository,
        IAtencionRepository atenciones)
    {
        _repository = repository;
        _atenciones = atenciones;
    }

    public async Task<NotaMedicaDto> Handle(AgregarNotaMedicaCommand request, CancellationToken cancellationToken)
    {
        var atencion = await _atenciones.GetOneByAsync(
            a => a.AtencionID == request.AtencionID,
            cancellationToken: cancellationToken);

        if (atencion is null)
            throw new DomainException(Errores.ATENCION_NO_ENCONTRADA, $"No existe la atención {request.AtencionID}.");

        if (string.IsNullOrWhiteSpace(request.Tipo) || string.IsNullOrWhiteSpace(request.Contenido))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El tipo y el contenido de la nota son obligatorios.");

        var nota = new NotaMedica
        {
            AtencionID = request.AtencionID,
            MedicoID = request.MedicoID,
            FechaNota = request.FechaNota == default ? DateTime.UtcNow : request.FechaNota,
            Tipo = request.Tipo.Trim(),
            Contenido = request.Contenido.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _repository.AddAsync(nota);
        return ClinicalMapper.ToDto(creada);
    }
}
