using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Entities;
using Shared.Kernel.Interfaces;

namespace Shared.Kernel.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PagedResult<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        bool splitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "pageNumber must be 1 or greater.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize must be 1 or greater.");

        var query = ApplyIncludes(asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsQueryable(), includes);

        if (filter != null) query = query.Where(filter);
        if (splitQuery) query = query.AsSplitQuery();

        int totalRecords = await query.CountAsync(cancellationToken);

        query = orderBy != null ? orderBy(query) : query;
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        var data = await query.ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Data = data,
            TotalRecords = totalRecords,
            PageSize = pageSize,
            CurrentPage = pageNumber
        };
    }

    private static IQueryable<T> ApplyIncludes(IQueryable<T> query, Expression<Func<T, object>>[] includes)
    {
        foreach (var include in includes)
            query = query.Include(include);
        return query;
    }
}
