using Billing.Core.DTOs;
using Billing.Core.DTOs.Billing;
using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Billing.Core.Features.Billing.Servicios.Queries.GetServicios;

public class GetServiciosQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<ServicioDto>>>>
{
}

public class GetServiciosQueryEventHandler : IRequestHandler<GetServiciosQuery, HttpResponse<PagedDto<List<ServicioDto>>>>
{
    private readonly IServicioRepository _servicioRepository;

    public GetServiciosQueryEventHandler(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
    }

    public async Task<HttpResponse<PagedDto<List<ServicioDto>>>> Handle(GetServiciosQuery request, CancellationToken cancellationToken)
    {
        var result = await _servicioRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            !string.IsNullOrEmpty(request.Filter) ? Filter.FromStringExpression<Servicio>(request.Filter) : null,
            orderByProperty: request.OrderBy,
            orderByDescending: request.OrderByDescending,
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedDto<List<ServicioDto>>>(new PagedDto<List<ServicioDto>>()
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = result.Data.Select(s => new ServicioDto
            {
                ServicioID = s.ServicioID,
                Codigo = s.Codigo,
                Nombre = s.Nombre,
                TipoServicio = s.TipoServicio,
                Descripcion = s.Descripcion,
                Activo = s.Activo
            }).ToList()
        });
    }
}
