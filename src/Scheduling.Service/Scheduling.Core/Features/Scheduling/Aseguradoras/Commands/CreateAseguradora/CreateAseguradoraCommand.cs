using MediatR;
using Scheduling.Core.Exceptions;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;

namespace Scheduling.Core.Features.Scheduling.Aseguradoras.Commands.CreateAseguradora;

public class CreateAseguradoraCommand : IRequest<int>
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? NumeroFiscal { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
}

public class CreateAseguradoraCommandHandler : IRequestHandler<CreateAseguradoraCommand, int>
{
    private readonly IAseguradoraRepository _aseguradoraRepository;

    public CreateAseguradoraCommandHandler(IAseguradoraRepository aseguradoraRepository)
    {
        _aseguradoraRepository = aseguradoraRepository;
    }

    public async Task<int> Handle(CreateAseguradoraCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Codigo) || string.IsNullOrWhiteSpace(request.Nombre))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El código y el nombre de la aseguradora son obligatorios.");

        var existente = await _aseguradoraRepository.GetOneByAsync(
            a => a.Codigo == request.Codigo,
            cancellationToken: cancellationToken);

        if (existente is not null)
            throw new DomainException(Errores.DATOS_INVALIDOS, $"Ya existe una aseguradora con el código {request.Codigo}.");

        var aseguradora = new Aseguradora
        {
            Codigo = request.Codigo.Trim(),
            Nombre = request.Nombre.Trim(),
            NumeroFiscal = request.NumeroFiscal,
            Telefono = request.Telefono,
            Email = request.Email,
            Direccion = request.Direccion,
            Activo = request.Activo
        };

        var creada = await _aseguradoraRepository.AddAsync(aseguradora);
        return creada.AseguradoraID;
    }
}
