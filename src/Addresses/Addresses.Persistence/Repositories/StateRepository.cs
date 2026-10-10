using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using Addresses.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Addresses.Persistence.Repositories;

public class StateRepository : Repository<State>, IStateRepository
{
    public StateRepository(AddressesDbContext context) : base(context) { }
}