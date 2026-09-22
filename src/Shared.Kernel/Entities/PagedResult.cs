namespace Shared.Kernel.Entities;

public class PagedResult<TEntity> where TEntity : class
{
    public List<TEntity> Data { get; set; } = new();
    public int TotalRecords { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage => (int)Math.Ceiling((double)TotalRecords / PageSize);
}
