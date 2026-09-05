using Domain.Billing.Entities;
using MediatR;
using Domain.Interfaces;

namespace Core.Features.Billing.Tarifas.Commands.CreateTarifa;

public class CreateTarifaCommand : IRequest<int>
{
    public int ServicioID { get; set; }
    public int? AseguradoraID { get; set; }
    public string NombreTarifa { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = "USD";
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public bool Activa { get; set; } = true;
}

public class CreateTarifaCommandHandler : IRequestHandler<CreateTarifaCommand, int>
{
    private readonly IRepository<Tarifa> _tarifaRepository;
    private readonly IRepository<Servicio> _servicioRepository;

    public CreateTarifaCommandHandler(IRepository<Tarifa> tarifaRepository, IRepository<Servicio> servicioRepository)
    {
        _tarifaRepository = tarifaRepository;
        _servicioRepository = servicioRepository;
    }

    public async Task<int> Handle(CreateTarifaCommand request, CancellationToken cancellationToken)
    {
        var servicio = await _servicioRepository.GetByIdAsync(request.ServicioID);
        if (servicio is null)
            throw new InvalidOperationException($"No existe el servicio con ID {request.ServicioID}.");

        var tarifa = new Tarifa
        {
            ServicioID = request.ServicioID,
            AseguradoraID = request.AseguradoraID,
            NombreTarifa = request.NombreTarifa,
            Precio = request.Precio,
            Moneda = request.Moneda,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Activa = request.Activa
        };

        var created = await _tarifaRepository.AddAsync(tarifa);
        return created.TarifaID;
    }
}
