using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using Addresses.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Addresses.Persistence.Repositories;

public class PositionRepository : Repository<Position>, IPositionRepository
{
    public PositionRepository(AddressesDbContext context) : base(context) { }
}