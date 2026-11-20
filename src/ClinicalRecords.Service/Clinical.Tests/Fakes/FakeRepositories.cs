using Clinical.Core.Interfaces.Repositories;
using Clinical.Domain.Entities;

namespace Clinical.Tests.Fakes;

public class FakeExpedienteClinicoRepository : InMemoryRepository<ExpedienteClinico>, IExpedienteClinicoRepository
{
    public FakeExpedienteClinicoRepository()
        : base(x => x.ExpedienteClinicoID, (x, id) => x.ExpedienteClinicoID = id)
    {
    }
}

public class FakeAtencionRepository : InMemoryRepository<Atencion>, IAtencionRepository
{
    public FakeAtencionRepository()
        : base(x => (int)x.AtencionID, (x, id) => x.AtencionID = id)
    {
    }
}

public class FakeSignosVitalesRepository : InMemoryRepository<SignosVitales>, ISignosVitalesRepository
{
    public FakeSignosVitalesRepository()
        : base(x => x.SignosVitalesID, (x, id) => x.SignosVitalesID = id)
    {
    }
}

public class FakeNotaMedicaRepository : InMemoryRepository<NotaMedica>, INotaMedicaRepository
{
    public FakeNotaMedicaRepository()
        : base(x => (int)x.NotaMedicaID, (x, id) => x.NotaMedicaID = id)
    {
    }
}

public class FakeDiagnosticoRepository : InMemoryRepository<Diagnostico>, IDiagnosticoRepository
{
    public FakeDiagnosticoRepository()
        : base(x => x.DiagnosticoID, (x, id) => x.DiagnosticoID = id)
    {
    }
}

public class FakeAtencionDiagnosticoRepository : InMemoryRepository<AtencionDiagnostico>, IAtencionDiagnosticoRepository
{
    public FakeAtencionDiagnosticoRepository()
        : base(x => (int)x.AtencionDiagnosticoID, (x, id) => x.AtencionDiagnosticoID = id)
    {
    }
}

public class FakeSintomaRepository : InMemoryRepository<Sintoma>, ISintomaRepository
{
    public FakeSintomaRepository()
        : base(x => x.SintomaID, (x, id) => x.SintomaID = id)
    {
    }
}

public class FakeAtencionSintomaRepository : InMemoryRepository<AtencionSintoma>, IAtencionSintomaRepository
{
    public FakeAtencionSintomaRepository()
        : base(x => (int)x.AtencionSintomaID, (x, id) => x.AtencionSintomaID = id)
    {
    }
}

public class FakeMedicamentoRepository : InMemoryRepository<Medicamento>, IMedicamentoRepository
{
    public FakeMedicamentoRepository()
        : base(x => x.MedicamentoID, (x, id) => x.MedicamentoID = id)
    {
    }
}

public class FakePrescripcionRepository : InMemoryRepository<Prescripcion>, IPrescripcionRepository
{
    public FakePrescripcionRepository()
        : base(x => (int)x.PrescripcionID, (x, id) => x.PrescripcionID = id)
    {
    }
}

public class FakePrescripcionDetalleRepository : InMemoryRepository<PrescripcionDetalle>, IPrescripcionDetalleRepository
{
    public FakePrescripcionDetalleRepository()
        : base(x => (int)x.PrescripcionDetalleID, (x, id) => x.PrescripcionDetalleID = id)
    {
    }
}

public class FakeTratamientoRepository : InMemoryRepository<Tratamiento>, ITratamientoRepository
{
    public FakeTratamientoRepository()
        : base(x => (int)x.TratamientoID, (x, id) => x.TratamientoID = id)
    {
    }
}

public class FakeTratamientoDetalleRepository : InMemoryRepository<TratamientoDetalle>, ITratamientoDetalleRepository
{
    public FakeTratamientoDetalleRepository()
        : base(x => (int)x.TratamientoDetalleID, (x, id) => x.TratamientoDetalleID = id)
    {
    }
}

public class FakeIndicacionMedicaRepository : InMemoryRepository<IndicacionMedica>, IIndicacionMedicaRepository
{
    public FakeIndicacionMedicaRepository()
        : base(x => (int)x.IndicacionMedicaID, (x, id) => x.IndicacionMedicaID = id)
    {
    }
}

public class FakeReferenciaMedicaRepository : InMemoryRepository<ReferenciaMedica>, IReferenciaMedicaRepository
{
    public FakeReferenciaMedicaRepository()
        : base(x => (int)x.ReferenciaMedicaID, (x, id) => x.ReferenciaMedicaID = id)
    {
    }
}

public class FakeProcedimientoRepository : InMemoryRepository<Procedimiento>, IProcedimientoRepository
{
    public FakeProcedimientoRepository()
        : base(x => x.ProcedimientoID, (x, id) => x.ProcedimientoID = id)
    {
    }
}

public class FakeAtencionProcedimientoRepository : InMemoryRepository<AtencionProcedimiento>, IAtencionProcedimientoRepository
{
    public FakeAtencionProcedimientoRepository()
        : base(x => (int)x.AtencionProcedimientoID, (x, id) => x.AtencionProcedimientoID = id)
    {
    }
}

public class FakeArchivoClinicoRepository : InMemoryRepository<ArchivoClinico>, IArchivoClinicoRepository
{
    public FakeArchivoClinicoRepository()
        : base(x => (int)x.ArchivoClinicoID, (x, id) => x.ArchivoClinicoID = id)
    {
    }
}

public class FakeAntecedenteMedicoRepository : InMemoryRepository<AntecedenteMedico>, IAntecedenteMedicoRepository
{
    public FakeAntecedenteMedicoRepository()
        : base(x => x.AntecedenteMedicoID, (x, id) => x.AntecedenteMedicoID = id)
    {
    }
}

public class FakeAntecedenteFamiliarRepository : InMemoryRepository<AntecedenteFamiliar>, IAntecedenteFamiliarRepository
{
    public FakeAntecedenteFamiliarRepository()
        : base(x => x.AntecedenteFamiliarID, (x, id) => x.AntecedenteFamiliarID = id)
    {
    }
}

public class FakeAlergiaRepository : InMemoryRepository<Alergia>, IAlergiaRepository
{
    public FakeAlergiaRepository()
        : base(x => x.AlergiaID, (x, id) => x.AlergiaID = id)
    {
    }
}

public class FakeHabitoRepository : InMemoryRepository<Habito>, IHabitoRepository
{
    public FakeHabitoRepository()
        : base(x => x.HabitoID, (x, id) => x.HabitoID = id)
    {
    }
}

public class FakeHabitacionRepository : InMemoryRepository<Habitacion>, IHabitacionRepository
{
    public FakeHabitacionRepository()
        : base(x => x.HabitacionID, (x, id) => x.HabitacionID = id)
    {
    }
}

public class FakeCamaRepository : InMemoryRepository<Cama>, ICamaRepository
{
    public FakeCamaRepository()
        : base(x => x.CamaID, (x, id) => x.CamaID = id)
    {
    }
}

public class FakeHospitalizacionRepository : InMemoryRepository<Hospitalizacion>, IHospitalizacionRepository
{
    public FakeHospitalizacionRepository()
        : base(x => (int)x.HospitalizacionID, (x, id) => x.HospitalizacionID = id)
    {
    }
}
