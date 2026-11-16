using System.Linq.Expressions;
using Shared.Kernel.Entities;
using Shared.Kernel.Interfaces;

namespace Scheduling.Tests.Fakes;

/// <summary>Repositorio en memoria mínimo para probar handlers sin EF.</summary>
public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> _items = new();
    private readonly Func<T, long> _getId;
    private readonly Action<T, long> _setId;

    public InMemoryRepository(Func<T, long> getId, Action<T, long> setId)
    {
        _getId = getId;
        _setId = setId;
    }

    public void Seed(params T[] items) => _items.AddRange(items);

    public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult<IEnumerable<T>>(_items);

    public Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        => Task.FromResult<IEnumerable<T>>(_items);

    public Task<T?> GetByIdAsync(int id)
        => Task.FromResult(_items.FirstOrDefault(x => _getId(x) == id));

    public Task<T> AddAsync(T entity)
    {
        var nextId = _items.Count == 0 ? 1 : _items.Max(_getId) + 1;
        _setId(entity, nextId);
        _items.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity) => Task.CompletedTask;

    public Task DeleteAsync(int id)
    {
        _items.RemoveAll(x => _getId(x) == id);
        return Task.CompletedTask;
    }

    public Task<T?> GetOneByAsync(
        Expression<Func<T, bool>>? filter = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        var query = _items.AsEnumerable();
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
        var query = _items.AsQueryable();

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
            var nextId = _items.Count == 0 ? 1 : _items.Max(_getId) + 1;
            _setId(entity, nextId);
            _items.Add(entity);
        }

        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities) => Task.CompletedTask;
}
