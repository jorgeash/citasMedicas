using Scheduling.Core.DTOs.Scheduling;
using Scheduling.Domain.Entities;

namespace Scheduling.Core.Features.Scheduling;

internal static class SchedulingMapper
{
    public static PacienteDto ToDto(Paciente paciente)
    {
        return new PacienteDto
        {
            PacienteID = paciente.PacienteID,
            CodigoPaciente = paciente.CodigoPaciente,
            TipoDocumento = paciente.TipoDocumento,
            NumeroDocumento = paciente.NumeroDocumento,
            Nombres = paciente.Nombres,
            Apellidos = paciente.Apellidos,
            FechaNacimiento = paciente.FechaNacimiento,
            Sexo = paciente.Sexo,
            Telefono = paciente.Telefono,
            Email = paciente.Email,
            Direccion = paciente.Direccion,
            AseguradoraID = ObtenerAseguradoraActiva(paciente),
            Activo = paciente.Activo
        };
    }

    public static CitaDto ToDto(Cita cita)
    {
        return new CitaDto
        {
            CitaID = cita.CitaID,
            PacienteID = cita.PacienteID,
            PacienteNombre = cita.Paciente is null
                ? null
                : $"{cita.Paciente.Nombres} {cita.Paciente.Apellidos}".Trim(),
            MedicoID = cita.MedicoID,
            MedicoNombre = cita.Medico is null
                ? null
                : $"{cita.Medico.Nombres} {cita.Medico.Apellidos}".Trim(),
            EspecialidadID = cita.EspecialidadID,
            ConsultorioID = cita.ConsultorioID,
            EstadoCitaID = cita.EstadoCitaID,
            EstadoCita = cita.EstadoCita?.Nombre,
            FechaHoraInicio = cita.FechaHoraInicio,
            FechaHoraFin = cita.FechaHoraFin,
            Motivo = cita.Motivo,
            Observaciones = cita.Observaciones,
            FechaCreacion = cita.FechaCreacion
        };
    }

    public static AseguradoraDto ToDto(Aseguradora aseguradora)
    {
        return new AseguradoraDto
        {
            AseguradoraID = aseguradora.AseguradoraID,
            Codigo = aseguradora.Codigo,
            Nombre = aseguradora.Nombre,
            NumeroFiscal = aseguradora.NumeroFiscal,
            Telefono = aseguradora.Telefono,
            Email = aseguradora.Email,
            Direccion = aseguradora.Direccion,
            Activo = aseguradora.Activo
        };
    }

    public static MedicoDto ToDto(Medico medico)
    {
        return new MedicoDto
        {
            MedicoID = medico.MedicoID,
            CodigoMedico = medico.CodigoMedico,
            Nombres = medico.Nombres,
            Apellidos = medico.Apellidos,
            NumeroLicencia = medico.NumeroLicencia,
            Telefono = medico.Telefono,
            Email = medico.Email,
            Activo = medico.Activo
        };
    }

    public static EstadoCitaDto ToDto(EstadoCita estadoCita)
    {
        return new EstadoCitaDto
        {
            EstadoCitaID = estadoCita.EstadoCitaID,
            Codigo = estadoCita.Codigo,
            Nombre = estadoCita.Nombre
        };
    }

    private static int? ObtenerAseguradoraActiva(Paciente paciente)
    {
        var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

        return paciente.Seguros
            .Where(s => s.Activo
                && (s.FechaInicio is null || s.FechaInicio <= hoy)
                && (s.FechaFin is null || s.FechaFin >= hoy))
            .OrderByDescending(s => s.FechaInicio)
            .Select(s => (int?)s.AseguradoraID)
            .FirstOrDefault();
    }
}
