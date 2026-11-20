using System.Linq.Expressions;
using Lab.Core.DTOs;
using Lab.Core.DTOs.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;
using Shared.Kernel.Helpers;

namespace Lab.Core.Features.Lab.Estudios.Queries.GetEstudios;

public class GetEstudiosQuery : RequestParameters, IRequest<PagedDto<List<EstudioDto>>>
{
    public bool? Activo { get; set; }
    public string? Tipo { get; set; }
}

public class GetEstudiosQueryHandler : IRequestHandler<GetEstudiosQuery, PagedDto<List<EstudioDto>>>
{
    private readonly IEstudioRepository _repository;

    public GetEstudiosQueryHandler(IEstudioRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedDto<List<EstudioDto>>> Handle(GetEstudiosQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Estudio, bool>>? filtro = string.IsNullOrWhiteSpace(request.Filter)
            ? null
            : Filter.FromStringExpression<Estudio>(request.Filter);

        if (request.Activo.HasValue)
        {
            var activo = request.Activo.Value;
            filtro = filtro is null
                ? e => e.Activo == activo
                : Filter.AndAlso(filtro, e => e.Activo == activo);
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            var tipo = request.Tipo.Trim().ToUpperInvariant();
            filtro = filtro is null
                ? e => e.Tipo == tipo
                : Filter.AndAlso(filtro, e => e.Tipo == tipo);
        }

        if (!string.IsNullOrWhiteSpace(request.Buscar))
        {
            var textoLibre = Filter.FreeText<Estudio>(request.Buscar, "Codigo", "Nombre", "Descripcion");
            filtro = filtro is null ? textoLibre : Filter.AndAlso(filtro, textoLibre);
        }

        var result = await _repository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            filtro,
            Filter.ParseOrderBy<Estudio>(request.OrderBy ?? string.Empty),
            cancellationToken: cancellationToken);

        return new PagedDto<List<EstudioDto>>(
            result.TotalRecords,
            result.CurrentPage,
            result.PageSize,
            result.Data.Select(LabMapper.ToDto).ToList());
    }
}
