using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using Addresses.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Addresses.Persistence.Repositories;

public class CityRepository : Repository<City>, ICityRepository
{
    public CityRepository(AddressesDbContext context) : base(context) { }
}