using System.Threading;
using System.Threading.Tasks;
using Domain.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Shared.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
