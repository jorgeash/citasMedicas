using System.Linq.Expressions;
using Shared.Kernel.Entities;
using Shared.Kernel.Interfaces;

namespace Identity.Tests.Fakes;

/// <summary>Repositorio en memoria mínimo para probar handlers sin EF.</summary>
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    /// <summary>Visible para los tests: les permite asertar sobre lo sembrado.</summary>
    public readonly List<T> Items = new();

    private readonly Func<T, long> _getId;
    private readonly Action<T, long> _setId;

    public InMemoryRepository(Func<T, long> getId, Action<T, long> setId)
    {
        _getId = getId;
        _setId = setId;
    }

    public void Seed(params T[] items) => Items.AddRange(items);

    public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult<IEnumerable<T>>(Items);

    public Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        => Task.FromResult<IEnumerable<T>>(Items);

    public Task<T?> GetByIdAsync(int id)
        => Task.FromResult(Items.FirstOrDefault(x => _getId(x) == id));

    public Task<T> AddAsync(T entity)
    {
        var nextId = Items.Count == 0 ? 1 : Items.Max(_getId) + 1;
        _setId(entity, nextId);
        Items.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity) => Task.CompletedTask;

    public Task DeleteAsync(int id)
    {
        Items.RemoveAll(x => _getId(x) == id);
        return Task.CompletedTask;
    }

    public Task<T?> GetOneByAsync(
        Expression<Func<T, bool>>? filter = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = Items.AsEnumerable();
        if (filter is not null)
            query = query.Where(filter.Compile());

        return Task.FromResult(query.FirstOrDefault());
    }

    public Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = Items.AsQueryable();

        if (filter is not null)
            query = query.Where(filter);

        var total = query.Count();

        if (orderBy is not null)
            query = orderBy(query);

        var data = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<T>
        {
            Data = data,
            TotalRecords = total,
            CurrentPage = pageNumber,
            PageSize = pageSize
        });
    }

    public Task AddRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            var nextId = Items.Count == 0 ? 1 : Items.Max(_getId) + 1;
            _setId(entity, nextId);
            Items.Add(entity);
        }

        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities) => Task.CompletedTask;
}
