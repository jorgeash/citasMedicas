using Billing.Core.DTOs;
using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Billing.Core.Features.Billing.Tarifas.Queries.GetTarifas;

public class GetTarifasQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<TarifaDto>>>>
{
}

public class GetTarifasQueryEventHandler : IRequestHandler<GetTarifasQuery, HttpResponse<PagedDto<List<TarifaDto>>>>
{
    private readonly ITarifaRepository _tarifaRepository;

    public GetTarifasQueryEventHandler(ITarifaRepository tarifaRepository)
    {
        _tarifaRepository = tarifaRepository;
    }

    public async Task<HttpResponse<PagedDto<List<TarifaDto>>>> Handle(GetTarifasQuery request, CancellationToken cancellationToken)
    {
        var result = await _tarifaRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<Tarifa>(request.Filter) : null,
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedDto<List<TarifaDto>>>(new PagedDto<List<TarifaDto>>()
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = result.Data.Select(MapToDto).ToList()
        });
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
