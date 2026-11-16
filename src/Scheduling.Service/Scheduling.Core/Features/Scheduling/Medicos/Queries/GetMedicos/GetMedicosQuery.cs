using System.Linq.Expressions;
using MediatR;
using Scheduling.Core.DTOs;
using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Core.Features.Scheduling;
using Scheduling.Core.Interfaces.Repositories;
using Scheduling.Domain.Entities;
using Shared.Kernel.Helpers;

namespace Scheduling.Core.Features.Scheduling.Medicos.Queries.GetMedicos;

public class GetMedicosQuery : RequestParameters, IRequest<HttpResponse<PagedDto<List<MedicoDto>>>>
{
}

public class GetMedicosQueryHandler : IRequestHandler<GetMedicosQuery, HttpResponse<PagedDto<List<MedicoDto>>>>
{
    private readonly IMedicoRepository _medicoRepository;

    public GetMedicosQueryHandler(IMedicoRepository medicoRepository)
    {
        _medicoRepository = medicoRepository;
    }

    public async Task<HttpResponse<PagedDto<List<MedicoDto>>>> Handle(
        GetMedicosQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Medico, bool>>? filtro =
            !string.IsNullOrWhiteSpace(request.Filter)
                ? Filter.FromStringExpression<Medico>(request.Filter)
                : null;

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<Medico>(
                request.Buscar,
                "CodigoMedico", "Nombres", "Apellidos", "NumeroLicencia", "Email");
            filtro = filtro is null ? textoLibre : Filter.AndAlso(filtro, textoLibre);
        }

        var result = await _medicoRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filtro,
            Filter.ParseOrderBy<Medico>(request.OrderBy ?? string.Empty)
                ?? (query => query.OrderBy(m => m.Apellidos).ThenBy(m => m.Nombres)),
            cancellationToken: cancellationToken);

        return new HttpResponse<PagedDto<List<MedicoDto>>>(new PagedDto<List<MedicoDto>>
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = result.Data.Select(SchedulingMapper.ToDto).ToList()
        });
    }
}
