using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using Business.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Business.Persistence.Repositories;

public class CustomerTypeRepository : Repository<CustomerType>, ICustomerTypeRepository
{
    public CustomerTypeRepository(BusinessDbContext context) : base(context) { }
}