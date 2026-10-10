using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using Business.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Business.Persistence.Repositories;

public class CustomerContactRepository : Repository<CustomerContact>, ICustomerContactRepository
{
    public CustomerContactRepository(BusinessDbContext context) : base(context) { }
}