using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;

namespace Billing.Core.Features.Billing.Tarifas.Commands.CreateTarifasBulk;

public class CreateTarifasBulkCommand : IRequest<int>
{
    public List<CreateTarifaItem> Tarifas { get; set; } = new();
}

public class CreateTarifaItem
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

public class CreateTarifasBulkCommandHandler : IRequestHandler<CreateTarifasBulkCommand, int>
{
    private readonly ITarifaRepository _tarifaRepository;

    public CreateTarifasBulkCommandHandler(ITarifaRepository tarifaRepository)
    {
        _tarifaRepository = tarifaRepository;
    }

    public async Task<int> Handle(CreateTarifasBulkCommand request, CancellationToken cancellationToken)
    {
        var tarifas = request.Tarifas.Select(t => new Tarifa
        {
            ServicioID = t.ServicioID,
            AseguradoraID = t.AseguradoraID,
            NombreTarifa = t.NombreTarifa,
            Precio = t.Precio,
            Moneda = t.Moneda,
            FechaInicio = t.FechaInicio,
            FechaFin = t.FechaFin,
            Activa = t.Activa
        }).ToList();

        await _tarifaRepository.AddRangeAsync(tarifas);
        return tarifas.Count;
    }
}
