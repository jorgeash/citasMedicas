using MediatR;
using Scheduling.Core.Exceptions;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;

namespace Scheduling.Core.Features.Scheduling.Pacientes.Commands.CreatePaciente;

public class CreatePacienteCommand : IRequest<long>
{
    public string CodigoPaciente { get; set; } = string.Empty;
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? EstadoCivil { get; set; }
    public string? Telefono { get; set; }
    public string? TelefonoSecundario { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
    public string? Ocupacion { get; set; }
    public string? TipoSangre { get; set; }
    public int? AseguradoraID { get; set; }
    public string? NumeroPoliza { get; set; }
    public bool Activo { get; set; } = true;
}

public class CreatePacienteCommandHandler : IRequestHandler<CreatePacienteCommand, long>
{
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IAseguradoraRepository _aseguradoraRepository;

    public CreatePacienteCommandHandler(
        IPacienteRepository pacienteRepository,
        IAseguradoraRepository aseguradoraRepository)
    {
        _pacienteRepository = pacienteRepository;
        _aseguradoraRepository = aseguradoraRepository;
    }

    public async Task<long> Handle(CreatePacienteCommand request, CancellationToken cancellationToken)
    {
        Validar(request, cancellationToken);

        var existenteCodigo = await _pacienteRepository.GetOneByAsync(
            p => p.CodigoPaciente == request.CodigoPaciente,
            cancellationToken: cancellationToken);

        if (existenteCodigo is not null)
            throw new DomainException(Errores.PACIENTE_DUPLICADO, $"Ya existe un paciente con el código {request.CodigoPaciente}.");

        var existenteDocumento = await _pacienteRepository.GetOneByAsync(
            p => p.TipoDocumento == request.TipoDocumento && p.NumeroDocumento == request.NumeroDocumento,
            cancellationToken: cancellationToken);

        if (existenteDocumento is not null)
            throw new DomainException(Errores.PACIENTE_DUPLICADO, $"Ya existe un paciente con el documento {request.NumeroDocumento}.");

        if (request.AseguradoraID.HasValue)
        {
            var aseguradora = await _aseguradoraRepository.GetOneByAsync(
                a => a.AseguradoraID == request.AseguradoraID.Value,
                cancellationToken: cancellationToken);

            if (aseguradora is null)
                throw new DomainException(Errores.ASEGURADORA_NO_ENCONTRADA, $"No existe la aseguradora {request.AseguradoraID.Value}.");

            if (string.IsNullOrWhiteSpace(request.NumeroPoliza))
                throw new DomainException(Errores.DATOS_INVALIDOS, "El número de póliza es obligatorio cuando se asigna una aseguradora.");
        }

        var paciente = new Paciente
        {
            CodigoPaciente = request.CodigoPaciente.Trim(),
            TipoDocumento = request.TipoDocumento.Trim(),
            NumeroDocumento = request.NumeroDocumento.Trim(),
            Nombres = request.Nombres.Trim(),
            Apellidos = request.Apellidos.Trim(),
            FechaNacimiento = request.FechaNacimiento,
            Sexo = request.Sexo.Trim(),
            EstadoCivil = request.EstadoCivil,
            Telefono = request.Telefono,
            TelefonoSecundario = request.TelefonoSecundario,
            Email = request.Email,
            Direccion = request.Direccion,
            Ciudad = request.Ciudad,
            Pais = request.Pais,
            Ocupacion = request.Ocupacion,
            TipoSangre = request.TipoSangre,
            Activo = request.Activo,
            FechaRegistro = DateTime.UtcNow
        };

        if (request.AseguradoraID.HasValue)
        {
            paciente.Seguros.Add(new PacienteAseguradora
            {
                AseguradoraID = request.AseguradoraID.Value,
                NumeroPoliza = request.NumeroPoliza!.Trim(),
                Activo = true,
                FechaInicio = DateOnly.FromDateTime(DateTime.UtcNow)
            });
        }

        var creado = await _pacienteRepository.AddAsync(paciente);
        return creado.PacienteID;
    }

    private static void Validar(CreatePacienteCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CodigoPaciente) ||
            string.IsNullOrWhiteSpace(request.TipoDocumento) ||
            string.IsNullOrWhiteSpace(request.NumeroDocumento) ||
            string.IsNullOrWhiteSpace(request.Nombres) ||
            string.IsNullOrWhiteSpace(request.Apellidos) ||
            string.IsNullOrWhiteSpace(request.Sexo))
        {
            throw new DomainException(Errores.DATOS_INVALIDOS, "Faltan datos obligatorios del paciente.");
        }

        if (request.FechaNacimiento == default)
            throw new DomainException(Errores.DATOS_INVALIDOS, "La fecha de nacimiento es obligatoria.");

        if (request.Sexo.Trim().Length != 1)
            throw new DomainException(Errores.DATOS_INVALIDOS, "El sexo debe tener un solo carácter (M/F/O).");
    }
}
