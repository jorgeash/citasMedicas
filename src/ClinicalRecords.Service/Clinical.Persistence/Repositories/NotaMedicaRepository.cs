using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class NotaMedicaRepository : Repository<NotaMedica>, INotaMedicaRepository
{
    public NotaMedicaRepository(ClinicalDbContext context) : base(context) { }
}
