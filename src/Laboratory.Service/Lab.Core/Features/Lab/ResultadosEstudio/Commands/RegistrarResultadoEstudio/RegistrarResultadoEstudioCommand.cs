using System.Linq.Expressions;
using Lab.Core.DTOs.Lab;
using Lab.Core.Exceptions;
using Lab.Core.Features.Lab;
using Lab.Core.Interfaces.Repositories;
using Lab.Domain.Entities;
using MediatR;

namespace Lab.Core.Features.Lab.ResultadosEstudio.Commands.RegistrarResultadoEstudio;

public class RegistrarResultadoEstudioCommand : IRequest<ResultadoEstudioDto>
{
    public int SolicitudEstudioID { get; set; }
    public int SolicitudEstudioDetalleID { get; set; }
    public DateTime? FechaResultado { get; set; }
    public string Valor { get; set; } = string.Empty;
    public string? Unidad { get; set; }
    public string? RangoReferencia { get; set; }
    public string Estado { get; set; } = "NORMAL";
    public string? Interpretacion { get; set; }
    public string? ArchivoUrl { get; set; }
    public int? ValidadoPorMedicoID { get; set; }
}

public class RegistrarResultadoEstudioCommandHandler : IRequestHandler<RegistrarResultadoEstudioCommand, ResultadoEstudioDto>
{
    private readonly IResultadoEstudioRepository _resultadoRepository;
    private readonly ISolicitudEstudioRepository _solicitudRepository;

    public RegistrarResultadoEstudioCommandHandler(
        IResultadoEstudioRepository resultadoRepository,
        ISolicitudEstudioRepository solicitudRepository)
    {
        _resultadoRepository = resultadoRepository;
        _solicitudRepository = solicitudRepository;
    }

    public async Task<ResultadoEstudioDto> Handle(
        RegistrarResultadoEstudioCommand request,
        CancellationToken cancellationToken)
    {
        var solicitud = await _solicitudRepository.GetOneByAsync(
            s => s.SolicitudEstudioID == request.SolicitudEstudioID,
            asNoTracking: false,
            cancellationToken: cancellationToken,
            includes: new Expression<Func<SolicitudEstudio, object>>[]
            {
                s => s.Detalle,
                s => s.Resultados
            });

        if (solicitud is null)
            throw new DomainException(Errores.SOLICITUD_NO_ENCONTRADA, $"No existe la solicitud {request.SolicitudEstudioID}.");

        if (solicitud.Estado == Estados.SolicitudCancelada)
            throw new DomainException(Errores.SOLICITUD_CANCELADA, "No se puede registrar un resultado en una solicitud cancelada.");

        if (solicitud.Detalle.All(d => d.SolicitudEstudioDetalleID != request.SolicitudEstudioDetalleID))
            throw new DomainException(
                Errores.DETALLE_NO_ENCONTRADO,
                $"El detalle {request.SolicitudEstudioDetalleID} no pertenece a la solicitud {request.SolicitudEstudioID}.");

        if (solicitud.Resultados.Any(r => r.SolicitudEstudioDetalleID == request.SolicitudEstudioDetalleID))
            throw new DomainException(
                Errores.RESULTADO_DUPLICADO,
                $"Ya existe un resultado para el detalle {request.SolicitudEstudioDetalleID}.");

        var estado = request.Estado?.Trim().ToUpperInvariant();
        if (!Estados.EstadosResultado.Contains(estado))
            throw new DomainException(
                Errores.ESTADO_RESULTADO_INVALIDO,
                $"El estado del resultado '{request.Estado}' no es válido.");

        var valor = LabMapper.Truncar(request.Valor, 500);
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException(Errores.DATOS_INVALIDOS, "El valor del resultado es obligatorio.");

        var ahora = DateTime.UtcNow;
        var resultado = new ResultadoEstudio
        {
            SolicitudEstudioID = solicitud.SolicitudEstudioID,
            SolicitudEstudioDetalleID = request.SolicitudEstudioDetalleID,
            FechaResultado = request.FechaResultado ?? ahora,
            Valor = valor,
            Unidad = LabMapper.Truncar(request.Unidad, 50),
            RangoReferencia = LabMapper.Truncar(request.RangoReferencia, 200),
            Estado = estado!,
            Interpretacion = LabMapper.Truncar(request.Interpretacion, 2000),
            ArchivoUrl = LabMapper.Truncar(request.ArchivoUrl, 500),
            ValidadoPorMedicoID = request.ValidadoPorMedicoID,
            FechaValidacion = request.ValidadoPorMedicoID.HasValue ? ahora : null,
            CreatedAt = ahora,
            SolicitudEstudio = solicitud
        };

        await _resultadoRepository.AddAsync(resultado);
        solicitud.Resultados.Add(resultado);

        if (solicitud.Estado == Estados.SolicitudPendiente)
            solicitud.Estado = Estados.SolicitudEnProceso;

        if (solicitud.Detalle.All(d =>
                solicitud.Resultados.Any(r => r.SolicitudEstudioDetalleID == d.SolicitudEstudioDetalleID)))
        {
            solicitud.Estado = Estados.SolicitudCompletada;
            solicitud.UpdatedAt = ahora;
        }

        await _solicitudRepository.UpdateAsync(solicitud);

        return LabMapper.ToDto(resultado);
    }
}
