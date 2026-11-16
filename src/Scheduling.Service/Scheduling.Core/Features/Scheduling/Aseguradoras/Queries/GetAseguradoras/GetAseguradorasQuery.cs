using System.Linq.Expressions;
using MediatR;
using Scheduling.Core.DTOs;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Shared.Kernel.Helpers;

namespace Scheduling.Core.Features.Scheduling.Aseguradoras.Queries.GetAseguradoras;

public class GetAseguradorasQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<AseguradoraDto>>>>
{
}

public class GetAseguradorasQueryHandler : IRequestHandler<GetAseguradorasQuery, HttpResponse<PagedDto<List<AseguradoraDto>>>>
{
    private readonly IAseguradoraRepository _aseguradoraRepository;

    public GetAseguradorasQueryHandler(IAseguradoraRepository aseguradoraRepository)
    {
        _aseguradoraRepository = aseguradoraRepository;
    }

    public async Task<HttpResponse<PagedDto<List<AseguradoraDto>>>> Handle(
        GetAseguradorasQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Aseguradora, bool>>? filtro =
            !string.IsNullOrWhiteSpace(request.Filter)
                ? Filter.FromStringExpression<Aseguradora>(request.Filter)
                : null;

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<Aseguradora>(
                request.Buscar,
                "Codigo", "Nombre", "NumeroFiscal", "Telefono", "Email");
            filtro = filtro is null ? textoLibre : Filter.AndAlso(filtro, textoLibre);
        }

        var result = await _aseguradoraRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filtro,
            Filter.ParseOrderBy<Aseguradora>(request.OrderBy ?? string.Empty)
                ?? (query => query.OrderBy(a => a.Nombre)),
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedDto<List<AseguradoraDto>>>(new PagedDto<List<AseguradoraDto>>
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = result.Data.Select(SchedulingMapper.ToDto).ToList()
        });
    }
}
