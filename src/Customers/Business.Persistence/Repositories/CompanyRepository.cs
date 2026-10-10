using Business.Core.Interfaces.Repositories;
using Business.Domain.Entities;
using Business.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Business.Persistence.Repositories;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(BusinessDbContext context) : base(context) { }
}