using Business.Core.DTOs;
using Shared.Kernel.Entities;

namespace Business.Core.Extensions;

public static class PagingExtensions
{
    /// <summary>
    /// Convierte el resultado paginado del repositorio en el sobre PagedDto que viaja por
    /// HTTP. Se centraliza aqui para que todos los handlers devuelvan la misma forma.
    /// </summary>
    public static PagedDto<List<TDestination>> ToPagedDto<TSource, TDestination>(
        this PagedResult<TSource> result,
        Func<TSource, TDestination> map)
        where TSource : class
    {
        return new PagedDto<List<TDestination>>
        {
            CurrentPage = result.CurrentPage,
            PageSize = result.PageSize,
            TotalPage = result.TotalPage,
            TotalRecords = result.TotalRecords,
            Data = result.Data.Select(map).ToList()
        };
    }
}