using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Billing.Core.Features.Billing.Tarifas.Queries.GetOneTarifaByFilter;

public class GetOneTarifaByFilterQuery : IRequest<TarifaDto?>
{
    public string Filter { get; set; } = string.Empty;
}

public class GetOneTarifaByFilterQueryHandler
    : IRequestHandler<GetOneTarifaByFilterQuery, TarifaDto?>
{
    private readonly ITarifaRepository _repository;

    public GetOneTarifaByFilterQueryHandler(ITarifaRepository repository)
    {
        _repository = repository;
    }

    public async Task<TarifaDto?> Handle(
        GetOneTarifaByFilterQuery request,
        CancellationToken cancellationToken)
    {
        var filter = Filter.FromStringExpression<Tarifa>(request.Filter);
        var tarifa = await _repository.GetOneByAsync(filter, cancellationToken: cancellationToken);

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
