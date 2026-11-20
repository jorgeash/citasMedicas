using Clinical.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinical.Persistence.Context;

public class ClinicalDbContext : DbContext
{
    public ClinicalDbContext(DbContextOptions<ClinicalDbContext> options) : base(options) { }

    public DbSet<ExpedienteClinico> ExpedientesClinicos => Set<ExpedienteClinico>();
    public DbSet<AntecedenteMedico> AntecedentesMedicos => Set<AntecedenteMedico>();
    public DbSet<AntecedenteFamiliar> AntecedentesFamiliares => Set<AntecedenteFamiliar>();
    public DbSet<Alergia> Alergias => Set<Alergia>();
    public DbSet<SignosVitales> SignosVitales => Set<SignosVitales>();
    public DbSet<Habito> Habitos => Set<Habito>();
    public DbSet<Atencion> Atenciones => Set<Atencion>();
    public DbSet<NotaMedica> NotasMedicas => Set<NotaMedica>();
    public DbSet<Diagnostico> Diagnosticos => Set<Diagnostico>();
    public DbSet<AtencionDiagnostico> AtencionDiagnosticos => Set<AtencionDiagnostico>();
    public DbSet<Sintoma> Sintomas => Set<Sintoma>();
    public DbSet<AtencionSintoma> AtencionSintomas => Set<AtencionSintoma>();
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();
    public DbSet<Prescripcion> Prescripciones => Set<Prescripcion>();
    public DbSet<PrescripcionDetalle> PrescripcionDetalles => Set<PrescripcionDetalle>();
    public DbSet<Tratamiento> Tratamientos => Set<Tratamiento>();
    public DbSet<TratamientoDetalle> TratamientoDetalles => Set<TratamientoDetalle>();
    public DbSet<IndicacionMedica> IndicacionesMedicas => Set<IndicacionMedica>();
    public DbSet<ReferenciaMedica> ReferenciasMedicas => Set<ReferenciaMedica>();
    public DbSet<Procedimiento> Procedimientos => Set<Procedimiento>();
    public DbSet<AtencionProcedimiento> AtencionProcedimientos => Set<AtencionProcedimiento>();
    public DbSet<ArchivoClinico> ArchivosClinicos => Set<ArchivoClinico>();
    public DbSet<Habitacion> Habitaciones => Set<Habitacion>();
    public DbSet<Cama> Camas => Set<Cama>();
    public DbSet<Hospitalizacion> Hospitalizaciones => Set<Hospitalizacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicalDbContext).Assembly);
    }
}
