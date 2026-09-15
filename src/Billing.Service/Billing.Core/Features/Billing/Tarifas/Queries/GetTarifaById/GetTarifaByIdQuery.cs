using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;

namespace Billing.Core.Features.Billing.Tarifas.Queries.GetTarifaById;

public class GetTarifaByIdQuery : IRequest<TarifaDto?>
{
    public int TarifaID { get; set; }

    public GetTarifaByIdQuery(int tarifaID)
    {
        TarifaID = tarifaID;
    }
}

public class GetTarifaByIdQueryHandler : IRequestHandler<GetTarifaByIdQuery, TarifaDto?>
{
    private readonly ITarifaRepository _repository;

    public GetTarifaByIdQueryHandler(ITarifaRepository repository)
    {
        _repository = repository;
    }

    public async Task<TarifaDto?> Handle(GetTarifaByIdQuery request, CancellationToken cancellationToken)
    {
        var tarifa = await _repository.GetByIdAsync(request.TarifaID);

        if (tarifa is null)
            return null;

        return new TarifaDto
        {
            TarifaID = tarifa.TarifaID,
            ServicioID = tarifa.ServicioID,
            ServicioNombre = tarifa.Servicio?.Nombre ?? string.Empty,
            AseguradoraID = tarifa.AseguradoraID,
            NombreTarifa = tarifa.NombreTarifa,
            Precio = tarifa.Precio,
            Moneda = tarifa.Moneda,
            FechaInicio = tarifa.FechaInicio,
            FechaFin = tarifa.FechaFin,
            Activa = tarifa.Activa
        };
    }
}
