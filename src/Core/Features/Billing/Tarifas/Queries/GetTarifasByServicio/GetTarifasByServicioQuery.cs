using Core.DTOs.Billing;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Features.Billing.Tarifas.Queries.GetTarifasByServicio;

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

        var tarifas = await _tarifaRepository.GetByServicioIdAsync(request.ServicioID);

        return tarifas.Select(t => new TarifaDto
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
