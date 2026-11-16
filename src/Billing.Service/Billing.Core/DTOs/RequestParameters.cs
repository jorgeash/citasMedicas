namespace Billing.Core.DTOs;

public class RequestParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }

    /// <summary>Texto libre de búsqueda genérica: se compara contra varios campos
    /// del listado (a diferencia de <see cref="Filter"/>, que es una expresión LINQ).</summary>
    public string? Buscar { get; set; }
}
