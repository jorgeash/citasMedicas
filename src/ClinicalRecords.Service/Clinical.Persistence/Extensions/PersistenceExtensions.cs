using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;
using Clinical.Persistence.Context;
using Clinical.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clinical.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClinicalDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ClinicaDB")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ClinicalDbContext>());

        services.AddScoped<IExpedienteClinicoRepository, ExpedienteClinicoRepository>();
        services.AddScoped<IAntecedenteMedicoRepository, AntecedenteMedicoRepository>();
        services.AddScoped<IAntecedenteFamiliarRepository, AntecedenteFamiliarRepository>();
        services.AddScoped<IAlergiaRepository, AlergiaRepository>();
        services.AddScoped<ISignosVitalesRepository, SignosVitalesRepository>();
        services.AddScoped<IHabitoRepository, HabitoRepository>();
        services.AddScoped<IAtencionRepository, AtencionRepository>();
        services.AddScoped<INotaMedicaRepository, NotaMedicaRepository>();
        services.AddScoped<IDiagnosticoRepository, DiagnosticoRepository>();
        services.AddScoped<IAtencionDiagnosticoRepository, AtencionDiagnosticoRepository>();
        services.AddScoped<ISintomaRepository, SintomaRepository>();
        services.AddScoped<IAtencionSintomaRepository, AtencionSintomaRepository>();
        services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
        services.AddScoped<IPrescripcionRepository, PrescripcionRepository>();
        services.AddScoped<IPrescripcionDetalleRepository, PrescripcionDetalleRepository>();
        services.AddScoped<ITratamientoRepository, TratamientoRepository>();
        services.AddScoped<ITratamientoDetalleRepository, TratamientoDetalleRepository>();
        services.AddScoped<IIndicacionMedicaRepository, IndicacionMedicaRepository>();
        services.AddScoped<IReferenciaMedicaRepository, ReferenciaMedicaRepository>();
        services.AddScoped<IProcedimientoRepository, ProcedimientoRepository>();
        services.AddScoped<IAtencionProcedimientoRepository, AtencionProcedimientoRepository>();
        services.AddScoped<IArchivoClinicoRepository, ArchivoClinicoRepository>();
        services.AddScoped<IHabitacionRepository, HabitacionRepository>();
        services.AddScoped<ICamaRepository, CamaRepository>();
        services.AddScoped<IHospitalizacionRepository, HospitalizacionRepository>();

        return services;
    }
}
