using Core.DTOs.Billing;
using Domain.Billing.Entities;
using MediatR;
using Domain.Interfaces;

namespace Core.Features.Billing.Tarifas.Queries.GetTarifas;

public class GetTarifasQuery : IRequest<IEnumerable<TarifaDto>>
{
}

public class GetTarifasQueryHandler : IRequestHandler<GetTarifasQuery, IEnumerable<TarifaDto>>
{
    private readonly IRepository<Tarifa> _repository;

    public GetTarifasQueryHandler(IRepository<Tarifa> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TarifaDto>> Handle(GetTarifasQuery request, CancellationToken cancellationToken)
    {
        var tarifas = await _repository.GetAllAsync();

        return tarifas.Select(MapToDto);
    }

    private static TarifaDto MapToDto(Tarifa t)
    {
        return new TarifaDto
        {
            TarifaID = t.TarifaID,
            ServicioID = t.ServicioID,
            ServicioNombre = t.Servicio?.Nombre ?? string.Empty,
            AseguradoraID = t.AseguradoraID,
            NombreTarifa = t.NombreTarifa,
            Precio = t.Precio,
            Moneda = t.Moneda,
            FechaInicio = t.FechaInicio,
            FechaFin = t.FechaFin,
            Activa = t.Activa
        };
    }
}
