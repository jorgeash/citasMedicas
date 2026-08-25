using Domain.Billing.Entities;
using Domain.Billing.Interfaces;
using MediatR;

namespace Core.Features.Billing.Servicios.Commands.CreateServicio;

public class CreateServicioCommand : IRequest<int>
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string TipoServicio { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}

public class CreateServicioCommandHandler : IRequestHandler<CreateServicioCommand, int>
{
    private readonly IServicioRepository _servicioRepository;

    public CreateServicioCommandHandler(IServicioRepository servicioRepository)
    {
        _servicioRepository = servicioRepository;
    }

    public async Task<int> Handle(CreateServicioCommand request, CancellationToken cancellationToken)
    {
        var servicio = new Servicio
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            TipoServicio = request.TipoServicio,
            Descripcion = request.Descripcion,
            Activo = request.Activo
        };

        var created = await _servicioRepository.AddAsync(servicio);
        return created.ServicioID;
    }
}
