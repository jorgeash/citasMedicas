using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Shared.Kernel.Repositories;

namespace Clinical.Persistence.Repositories;

public class HabitacionRepository : Repository<Habitacion>, IHabitacionRepository
{
    public HabitacionRepository(ClinicalDbContext context) : base(context) { }
}
