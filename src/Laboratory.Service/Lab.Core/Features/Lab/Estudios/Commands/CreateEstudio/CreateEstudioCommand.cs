using Lab.Core.DTOs.Lab;
using Lab.Core.Exceptions;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;

namespace Lab.Core.Features.Lab.Estudios.Commands.CreateEstudio;

public class CreateEstudioCommand : IRequest<EstudioDto>
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool RequiereAyuno { get; set; }
    public bool Activo { get; set; } = true;
    public decimal? Precio { get; set; }
}

public class CreateEstudioCommandHandler : IRequestHandler<CreateEstudioCommand, EstudioDto>
{
    private readonly IEstudioRepository _repository;

    public CreateEstudioCommandHandler(IEstudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<EstudioDto> Handle(CreateEstudioCommand request, CancellationToken cancellationToken)
    {
        var codigo = LabMapper.Truncar(request.Codigo?.Trim().ToUpperInvariant(), 50);

        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(request.Nombre))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El código y el nombre del estudio son obligatorios.");

        var tipo = request.Tipo?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(tipo) || !Estados.TiposEstudio.Contains(tipo))
            throw new DomainException(Errores.DATOS_INVALIDOS, $"El tipo de estudio '{request.Tipo}' no es válido.");

        var existente = await _repository.GetOneByAsync(
            e => e.Codigo == codigo,
            cancellationToken: cancellationToken);

        if (existente is not null)
            throw new DomainException(Errores.ESTUDIO_CODIGO_DUPLICADO, $"Ya existe un estudio con el código {codigo}.");

        var estudio = new Estudio
        {
            Codigo = codigo,
            Nombre = LabMapper.Truncar(request.Nombre, 200)!,
            Tipo = tipo,
            Descripcion = LabMapper.Truncar(request.Descripcion, 500),
            RequiereAyuno = request.RequiereAyuno,
            Activo = request.Activo,
            Precio = request.Precio,
            CreatedAt = DateTime.UtcNow
        };

        var creado = await _repository.AddAsync(estudio);
        return LabMapper.ToDto(creado);
    }
}
