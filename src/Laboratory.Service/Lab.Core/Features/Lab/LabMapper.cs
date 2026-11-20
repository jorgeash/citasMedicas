using Lab.Core.DTOs.Lab;
using Lab.Domain.Entities;

namespace Lab.Core.Features.Lab;

internal static class LabMapper
{
    public static EstudioDto ToDto(Estudio estudio)
    {
        return new EstudioDto
        {
            EstudioID = estudio.EstudioID,
            Codigo = estudio.Codigo,
            Nombre = estudio.Nombre,
            Tipo = estudio.Tipo,
            Descripcion = estudio.Descripcion,
            RequiereAyuno = estudio.RequiereAyuno,
            Activo = estudio.Activo,
            Precio = estudio.Precio,
            CreatedAt = estudio.CreatedAt
        };
    }

    public static SolicitudEstudioDto ToDto(SolicitudEstudio solicitud)
    {
        return new SolicitudEstudioDto
        {
            SolicitudEstudioID = solicitud.SolicitudEstudioID,
            NumeroSolicitud = solicitud.NumeroSolicitud,
            AtencionID = solicitud.AtencionID,
            PacienteID = solicitud.PacienteID,
            MedicoID = solicitud.MedicoID,
            FechaSolicitud = solicitud.FechaSolicitud,
            Estado = solicitud.Estado,
            Observaciones = solicitud.Observaciones,
            CreatedAt = solicitud.CreatedAt,
            UpdatedAt = solicitud.UpdatedAt,
            Detalle = solicitud.Detalle.Select(ToDetalleDto).ToList(),
            Resultados = solicitud.Resultados.Select(ToDto).ToList()
        };
    }

    public static ResultadoEstudioDto ToDto(ResultadoEstudio resultado)
    {
        return new ResultadoEstudioDto
        {
            ResultadoEstudioID = resultado.ResultadoEstudioID,
            SolicitudEstudioID = resultado.SolicitudEstudioID,
            SolicitudEstudioDetalleID = resultado.SolicitudEstudioDetalleID,
            NumeroSolicitud = resultado.SolicitudEstudio?.NumeroSolicitud,
            PacienteID = resultado.SolicitudEstudio?.PacienteID,
            FechaResultado = resultado.FechaResultado,
            Valor = resultado.Valor,
            Unidad = resultado.Unidad,
            RangoReferencia = resultado.RangoReferencia,
            Estado = resultado.Estado,
            Interpretacion = resultado.Interpretacion,
            ArchivoUrl = resultado.ArchivoUrl,
            ValidadoPorMedicoID = resultado.ValidadoPorMedicoID,
            FechaValidacion = resultado.FechaValidacion,
            CreatedAt = resultado.CreatedAt
        };
    }

    private static SolicitudEstudioDetalleDto ToDetalleDto(SolicitudEstudioDetalle detalle)
    {
        return new SolicitudEstudioDetalleDto
        {
            SolicitudEstudioDetalleID = detalle.SolicitudEstudioDetalleID,
            SolicitudEstudioID = detalle.SolicitudEstudioID,
            EstudioID = detalle.EstudioID,
            CodigoEstudio = detalle.Estudio?.Codigo,
            NombreEstudio = detalle.Estudio?.Nombre,
            Prioridad = detalle.Prioridad,
            Observaciones = detalle.Observaciones,
            CreatedAt = detalle.CreatedAt
        };
    }

    public static string? Truncar(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }
}
