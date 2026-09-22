using Billing.Core.Interfaces.Repositories;
using Billing.Domain.Entities;
using MediatR;

namespace Billing.Core.Features.Billing.Servicios.Commands.CreateServiciosBulk;

public class CreateServiciosBulkCommand : IRequest<int>
{
    public List<CreateServicioItem> Servicios { get; set; } = new();
}

public class CreateServicioItem
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string TipoServicio { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

public class CreateServiciosBulkCommandHandler : IRequestHandler<CreateServiciosBulkCommand, int>
{
    private readonly IServicioRepository _servicioRepository;

    public CreateServiciosBulkCommandHandler(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
    }

    public async Task<int> Handle(CreateServiciosBulkCommand request, CancellationToken cancellationToken)
    {
        var servicios = request.Servicios.Select(s => new Servicio
        {
            Codigo = s.Codigo,
            Nombre = s.Nombre,
            TipoServicio = s.TipoServicio,
            Descripcion = s.Descripcion,
            Activo = s.Activo
        }).ToList();

        await _servicioRepository.AddRangeAsync(servicios);
        return servicios.Count;
    }
}
