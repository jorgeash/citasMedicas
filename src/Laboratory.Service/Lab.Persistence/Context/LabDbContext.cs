using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Persistence.Context;

public class LabDbContext : DbContext
{
    public LabDbContext(DbContextOptions<LabDbContext> options) : base(options) { }

    public DbSet<Estudio> Estudios => Set<Estudio>();
    public DbSet<SolicitudEstudio> SolicitudesEstudio => Set<SolicitudEstudio>();
    public DbSet<SolicitudEstudioDetalle> SolicitudesEstudioDetalle => Set<SolicitudEstudioDetalle>();
    public DbSet<ResultadoEstudio> ResultadosEstudio => Set<ResultadoEstudio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LabDbContext).Assembly);
    }
}
