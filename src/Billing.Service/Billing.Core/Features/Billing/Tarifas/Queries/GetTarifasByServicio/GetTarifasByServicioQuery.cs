using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;

namespace Billing.Core.Features.Billing.Tarifas.Queries.GetTarifasByServicio;

public class GetTarifasByServicioQuery : IRequest<IEnumerable<TarifaDto>?>
{
    public int ServicioID { get; set; }

    public GetTarifasByServicioQuery(int servicioID)
    {
        ServicioID = servicioID;
    }
}

public class GetTarifasByServicioQueryHandler : IRequestHandler<GetTarifasByServicioQuery, IEnumerable<TarifaDto>?>
{
    private readonly IServicioRepository _servicioRepository;
    private readonly ITarifaRepository _tarifaRepository;

    public GetTarifasByServicioQueryHandler(IServicioRepository servicioRepository, ITarifaRepository tarifaRepository)
    {
        _servicioRepository = servicioRepository;
        _tarifaRepository = tarifaRepository;
    }

    public async Task<IEnumerable<TarifaDto>?> Handle(GetTarifasByServicioQuery request, CancellationToken cancellationToken)
    {
        var servicio = await _servicioRepository.GetByIdAsync(request.ServicioID);

        if (servicio is null)
            return null;

        var tarifas = await _tarifaRepository.GetAllAsync(t => t.Servicio);

        return tarifas
            .Where(t => t.ServicioID == request.ServicioID)
            .Select(t => new TarifaDto
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
            });
    }
}
