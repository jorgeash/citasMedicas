using Addresses.Core.Interfaces.Repositories;
using Addresses.Domain.Entities;
using Addresses.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Addresses.Persistence.Repositories;

public class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(AddressesDbContext context) : base(context) { }
}