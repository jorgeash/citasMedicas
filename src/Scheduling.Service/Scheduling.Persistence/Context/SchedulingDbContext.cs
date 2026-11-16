using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Entities;

namespace Scheduling.Persistence.Context;

/// <summary>
/// Contexto de Scheduling. Reutiliza las tablas existentes de ClinicaDB:
/// no hay migraciones ni creación de esquema, solo mapeo.
/// </summary>
public class SchedulingDbContext : DbContext
{
    public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<PacienteAseguradora> PacienteAseguradoras => Set<PacienteAseguradora>();
    public DbSet<Aseguradora> Aseguradoras => Set<Aseguradora>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<EstadoCita> EstadosCita => Set<EstadoCita>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
    }
}
