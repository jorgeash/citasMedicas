using Billing.Core.DTOs.Billing;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces.Repositories;
using Billing.Core.Services;
using Billing.Domain.Entities;
using Billing.Domain.Enums;
using MediatR;

namespace Billing.Core.Features.Billing.Facturas.Commands.CreateFactura;

public class CreateFacturaCommand : IRequest<FacturaDto>
{
    public int PacienteID { get; set; }
    public int? CitaID { get; set; }
    public int? AseguradoraID { get; set; }
    public string Moneda { get; set; } = "USD";
    public string? Observaciones { get; set; }
    public List<CreateFacturaItem> Detalle { get; set; } = new();
}

public class CreateFacturaItem
{
    public int TarifaID { get; set; }
    public int Cantidad { get; set; }
}

public class CreateFacturaCommandHandler : IRequestHandler<CreateFacturaCommand, FacturaDto>
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly ITarifaRepository _tarifaRepository;
    private readonly ISchedulingClient _schedulingClient;

    public CreateFacturaCommandHandler(
        IFacturaRepository facturaRepository,
        ITarifaRepository tarifaRepository,
        ISchedulingClient schedulingClient)
    {
        _facturaRepository = facturaRepository;
        _tarifaRepository = tarifaRepository;
        _schedulingClient = schedulingClient;
    }

    public async Task<FacturaDto> Handle(CreateFacturaCommand request, CancellationToken cancellationToken)
    {
        if (request.Detalle is null || request.Detalle.Count == 0)
            throw new DomainException(Errores.FACTURA_SIN_DETALLE, "La factura debe tener al menos un detalle.");

        if (request.PacienteID <= 0)
            throw new DomainException(Errores.PACIENTE_INVALIDO, "El paciente es obligatorio.");

        foreach (var item in request.Detalle)
        {
            if (item.Cantidad <= 0)
                throw new DomainException(Errores.CANTIDAD_INVALIDA, "La cantidad de cada línea debe ser mayor que cero.");
        }

        var paciente = await _schedulingClient.GetPacienteAsync(request.PacienteID, cancellationToken);
        if (paciente is null)
            throw new DomainException(Errores.PACIENTE_NO_ENCONTRADO, $"No existe el paciente {request.PacienteID}.");

        // La cita es opcional, pero si viene hay que validarla en Scheduling:
        // que exista y que pertenezca al mismo paciente de la factura.
        if (request.CitaID.HasValue)
        {
            var cita = await _schedulingClient.GetCitaAsync(request.CitaID.Value, cancellationToken);

            if (cita is null)
                throw new DomainException(Errores.CITA_NO_ENCONTRADA, $"No existe la cita {request.CitaID.Value}.");

            if (cita.PacienteID != request.PacienteID)
                throw new DomainException(
                    Errores.CITA_PACIENTE_INCONSISTENTE,
                    $"La cita {request.CitaID.Value} pertenece al paciente {cita.PacienteID}, no al paciente {request.PacienteID}.");
        }

        var factura = new Factura
        {
            Numero = $"TMP-{Guid.NewGuid():N}".Substring(0, 20),
            PacienteID = request.PacienteID,
            PacienteNombre = Truncar(paciente.Nombre, 200),
            CitaID = request.CitaID,
            AseguradoraID = request.AseguradoraID ?? paciente.AseguradoraID,
            FechaEmision = DateTime.UtcNow,
            Moneda = string.IsNullOrWhiteSpace(request.Moneda) ? "USD" : request.Moneda.Trim().ToUpperInvariant(),
            Observaciones = Truncar(request.Observaciones, 500),
            Estado = EstadoFactura.PENDIENTE
        };

        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var item in request.Detalle)
        {
            var tarifa = await _tarifaRepository.GetOneByAsync(
                t => t.TarifaID == item.TarifaID,
                true,
                false,
                cancellationToken,
                t => t.Servicio!);

            if (tarifa is null || !tarifa.Activa || tarifa.Servicio is null || !tarifa.Servicio.Activo)
                throw new DomainException(Errores.TARIFA_NO_VALIDA, $"La tarifa {item.TarifaID} no existe o no está activa.");

            if (tarifa.FechaInicio > hoy || (tarifa.FechaFin.HasValue && tarifa.FechaFin.Value < hoy))
                throw new DomainException(Errores.TARIFA_NO_VALIDA, $"La tarifa {item.TarifaID} está fuera de vigencia.");

            var detalle = new FacturaDetalle
            {
                ServicioID = tarifa.ServicioID,
                TarifaID = tarifa.TarifaID,
                Descripcion = Truncar(tarifa.Servicio.Nombre, 200) ?? string.Empty,
                Cantidad = item.Cantidad,
                PrecioUnitario = tarifa.Precio
            };

            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
            factura.Detalle.Add(detalle);
        }

        factura.Subtotal = factura.Detalle.Sum(d => d.Subtotal);
        factura.Impuesto = 0m;
        factura.Total = factura.Subtotal + factura.Impuesto;
        factura.Saldo = factura.Total;

        var creada = await _facturaRepository.AddAsync(factura);

        // El número definitivo se arma con el ID que genera la base de datos.
        creada.Numero = $"F-{creada.FechaEmision:yyyy}-{creada.FacturaID:D6}";
        await _facturaRepository.UpdateAsync(creada);

        return FacturaMapper.ToDto(creada);
    }

    private static string? Truncar(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
