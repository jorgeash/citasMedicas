using System.Threading;
using System.Threading.Tasks;

namespace Domain.Shared.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
