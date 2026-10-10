using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using Business.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Business.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(BusinessDbContext context) : base(context) { }
}