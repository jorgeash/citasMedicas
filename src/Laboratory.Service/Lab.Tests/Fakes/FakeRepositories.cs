using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;

namespace Lab.Tests.Fakes;

public class FakeEstudioRepository : InMemoryRepository<Estudio>, IEstudioRepository
{
    public FakeEstudioRepository()
        : base(e => e.EstudioID, (e, id) => e.EstudioID = id)
    {
    }
}

public class FakeSolicitudEstudioRepository : InMemoryRepository<SolicitudEstudio>, ISolicitudEstudioRepository
{
    public FakeSolicitudEstudioRepository()
        : base(s => s.SolicitudEstudioID, (s, id) => s.SolicitudEstudioID = id)
    {
    }
}

public class FakeSolicitudEstudioDetalleRepository
    : InMemoryRepository<SolicitudEstudioDetalle>, ISolicitudEstudioDetalleRepository
{
    public FakeSolicitudEstudioDetalleRepository()
        : base(d => d.SolicitudEstudioDetalleID, (d, id) => d.SolicitudEstudioDetalleID = id)
    {
    }
}

public class FakeResultadoEstudioRepository : InMemoryRepository<ResultadoEstudio>, IResultadoEstudioRepository
{
    public FakeResultadoEstudioRepository()
        : base(r => r.ResultadoEstudioID, (r, id) => r.ResultadoEstudioID = id)
    {
    }
}
