using Clinical.Core.DTOs.Clinical;
using Clinical.Domain.Entities;

namespace Clinical.Core.Features.Clinical;

public static class ClinicalMapper
{
    public static ExpedienteDto ToDto(ExpedienteClinico e) => new()
    {
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        PacienteID = e.PacienteID,
        NumeroExpediente = e.NumeroExpediente,
        FechaApertura = e.FechaApertura,
        Observaciones = e.Observaciones,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public static ExpedienteResumenDto ToResumenDto(ExpedienteClinico e) => new()
    {
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        PacienteID = e.PacienteID,
        NumeroExpediente = e.NumeroExpediente,
        FechaApertura = e.FechaApertura,
        Activo = e.Activo
    };

    public static ExpedienteDetalleDto ToDetalleDto(ExpedienteClinico e) => new()
    {
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        PacienteID = e.PacienteID,
        NumeroExpediente = e.NumeroExpediente,
        FechaApertura = e.FechaApertura,
        Observaciones = e.Observaciones,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
        AntecedentesMedicos = e.AntecedentesMedicos.Select(ToDto).ToList(),
        AntecedentesFamiliares = e.AntecedentesFamiliares.Select(ToDto).ToList(),
        Alergias = e.Alergias.Select(ToDto).ToList(),
        Habitos = e.Habitos.Select(ToDto).ToList()
    };

    public static AntecedenteMedicoDto ToDto(AntecedenteMedico e) => new()
    {
        AntecedenteMedicoID = e.AntecedenteMedicoID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        Tipo = e.Tipo,
        Descripcion = e.Descripcion,
        Fecha = e.Fecha,
        Observaciones = e.Observaciones,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static AntecedenteFamiliarDto ToDto(AntecedenteFamiliar e) => new()
    {
        AntecedenteFamiliarID = e.AntecedenteFamiliarID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        Parentesco = e.Parentesco,
        Enfermedad = e.Enfermedad,
        Observaciones = e.Observaciones,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static AlergiaDto ToDto(Alergia e) => new()
    {
        AlergiaID = e.AlergiaID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        Sustancia = e.Sustancia,
        Tipo = e.Tipo,
        Severidad = e.Severidad,
        Reaccion = e.Reaccion,
        FechaDeteccion = e.FechaDeteccion,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static HabitoDto ToDto(Habito e) => new()
    {
        HabitoID = e.HabitoID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        Fuma = e.Fuma,
        ConsumeAlcohol = e.ConsumeAlcohol,
        ActividadFisica = e.ActividadFisica,
        FrecuenciaActividad = e.FrecuenciaActividad,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static AtencionDto ToDto(Atencion e) => new()
    {
        AtencionID = e.AtencionID,
        CitaID = e.CitaID,
        PacienteID = e.PacienteID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        MedicoID = e.MedicoID,
        FechaAtencion = e.FechaAtencion,
        Motivo = e.Motivo,
        Antecedente = e.Antecedente,
        ExamenFisico = e.ExamenFisico,
        Conclusion = e.Conclusion,
        Estado = e.Estado,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public static AtencionResumenDto ToResumenDto(Atencion e) => new()
    {
        AtencionID = e.AtencionID,
        CitaID = e.CitaID,
        PacienteID = e.PacienteID,
        MedicoID = e.MedicoID,
        FechaAtencion = e.FechaAtencion,
        Motivo = e.Motivo,
        Estado = e.Estado
    };

    public static AtencionDetalleDto ToDetalleDto(Atencion e) => new()
    {
        AtencionID = e.AtencionID,
        CitaID = e.CitaID,
        PacienteID = e.PacienteID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        MedicoID = e.MedicoID,
        FechaAtencion = e.FechaAtencion,
        Motivo = e.Motivo,
        Antecedente = e.Antecedente,
        ExamenFisico = e.ExamenFisico,
        Conclusion = e.Conclusion,
        Estado = e.Estado,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
        SignosVitales = e.SignosVitales.Select(ToDto).ToList(),
        NotasMedicas = e.NotasMedicas.Select(ToDto).ToList(),
        Diagnosticos = e.Diagnosticos.Select(ToDto).ToList(),
        Sintomas = e.Sintomas.Select(ToDto).ToList(),
        Prescripciones = e.Prescripciones.Select(ToDto).ToList(),
        Tratamientos = e.Tratamientos.Select(ToDto).ToList(),
        IndicacionesMedicas = e.IndicacionesMedicas.Select(ToDto).ToList(),
        ReferenciasMedicas = e.ReferenciasMedicas.Select(ToDto).ToList(),
        Procedimientos = e.Procedimientos.Select(ToDto).ToList(),
        ArchivosClinicos = e.ArchivosClinicos.Select(ToDto).ToList()
    };

    public static SignosVitalesDto ToDto(global::Clinical.Domain.Entities.SignosVitales e) => new()
    {
        SignosVitalesID = e.SignosVitalesID,
        AtencionID = e.AtencionID,
        FechaMedicion = e.FechaMedicion,
        Temperatura = e.Temperatura,
        FrecuenciaCardiaca = e.FrecuenciaCardiaca,
        FrecuenciaRespiratoria = e.FrecuenciaRespiratoria,
        PresionSistolica = e.PresionSistolica,
        PresionDiastolica = e.PresionDiastolica,
        SaturacionOxigeno = e.SaturacionOxigeno,
        PesoKg = e.PesoKg,
        TallaCm = e.TallaCm,
        IMC = e.IMC,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static NotaMedicaDto ToDto(NotaMedica e) => new()
    {
        NotaMedicaID = e.NotaMedicaID,
        AtencionID = e.AtencionID,
        MedicoID = e.MedicoID,
        FechaNota = e.FechaNota,
        Tipo = e.Tipo,
        Contenido = e.Contenido,
        CreatedAt = e.CreatedAt
    };

    public static AtencionDiagnosticoDto ToDto(AtencionDiagnostico e) => new()
    {
        AtencionDiagnosticoID = e.AtencionDiagnosticoID,
        AtencionID = e.AtencionID,
        DiagnosticoID = e.DiagnosticoID,
        Codigo = e.Diagnostico?.Codigo ?? string.Empty,
        Nombre = e.Diagnostico?.Nombre ?? string.Empty,
        Tipo = e.Tipo,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static AtencionSintomaDto ToDto(AtencionSintoma e) => new()
    {
        AtencionSintomaID = e.AtencionSintomaID,
        AtencionID = e.AtencionID,
        SintomaID = e.SintomaID,
        Codigo = e.Sintoma?.Codigo ?? string.Empty,
        Nombre = e.Sintoma?.Nombre ?? string.Empty,
        Severidad = e.Severidad,
        DuracionDias = e.DuracionDias,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static IndicacionMedicaDto ToDto(IndicacionMedica e) => new()
    {
        IndicacionMedicaID = e.IndicacionMedicaID,
        AtencionID = e.AtencionID,
        MedicoID = e.MedicoID,
        Tipo = e.Tipo,
        Descripcion = e.Descripcion,
        Activa = e.Activa,
        CreatedAt = e.CreatedAt
    };

    public static ReferenciaMedicaDto ToDto(ReferenciaMedica e) => new()
    {
        ReferenciaMedicaID = e.ReferenciaMedicaID,
        AtencionID = e.AtencionID,
        EspecialidadID = e.EspecialidadID,
        MedicoDestinoID = e.MedicoDestinoID,
        FechaReferencia = e.FechaReferencia,
        Motivo = e.Motivo,
        Estado = e.Estado,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static AtencionProcedimientoDto ToDto(AtencionProcedimiento e) => new()
    {
        AtencionProcedimientoID = e.AtencionProcedimientoID,
        AtencionID = e.AtencionID,
        ProcedimientoID = e.ProcedimientoID,
        Codigo = e.Procedimiento?.Codigo ?? string.Empty,
        Nombre = e.Procedimiento?.Nombre ?? string.Empty,
        FechaRealizacion = e.FechaRealizacion,
        Cantidad = e.Cantidad,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static ArchivoClinicoDto ToDto(ArchivoClinico e) => new()
    {
        ArchivoClinicoID = e.ArchivoClinicoID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        AtencionID = e.AtencionID,
        Tipo = e.Tipo,
        NombreArchivo = e.NombreArchivo,
        Ruta = e.Ruta,
        ContentType = e.ContentType,
        TamanoBytes = e.TamanoBytes,
        FechaCarga = e.FechaCarga,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static DiagnosticoDto ToDto(Diagnostico e) => new()
    {
        DiagnosticoID = e.DiagnosticoID,
        Codigo = e.Codigo,
        Nombre = e.Nombre,
        Descripcion = e.Descripcion,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static SintomaDto ToDto(Sintoma e) => new()
    {
        SintomaID = e.SintomaID,
        Codigo = e.Codigo,
        Nombre = e.Nombre,
        Descripcion = e.Descripcion,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static MedicamentoDto ToDto(Medicamento e) => new()
    {
        MedicamentoID = e.MedicamentoID,
        Codigo = e.Codigo,
        Nombre = e.Nombre,
        Presentacion = e.Presentacion,
        Concentracion = e.Concentracion,
        ViaAdministracion = e.ViaAdministracion,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static ProcedimientoDto ToDto(Procedimiento e) => new()
    {
        ProcedimientoID = e.ProcedimientoID,
        Codigo = e.Codigo,
        Nombre = e.Nombre,
        Descripcion = e.Descripcion,
        Activo = e.Activo,
        CreatedAt = e.CreatedAt
    };

    public static PrescripcionDetalleDto ToDto(PrescripcionDetalle e) => new()
    {
        PrescripcionDetalleID = e.PrescripcionDetalleID,
        PrescripcionID = e.PrescripcionID,
        MedicamentoID = e.MedicamentoID,
        NombreMedicamento = e.Medicamento?.Nombre ?? string.Empty,
        Dosis = e.Dosis,
        Frecuencia = e.Frecuencia,
        Via = e.Via,
        DuracionDias = e.DuracionDias,
        Cantidad = e.Cantidad,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static PrescripcionDto ToDto(Prescripcion e) => new()
    {
        PrescripcionID = e.PrescripcionID,
        AtencionID = e.AtencionID,
        MedicoID = e.MedicoID,
        FechaPrescripcion = e.FechaPrescripcion,
        FechaInicio = e.FechaInicio,
        FechaFin = e.FechaFin,
        Estado = e.Estado,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt,
        Detalle = e.Detalle.Select(ToDto).ToList()
    };

    public static TratamientoDetalleDto ToDto(TratamientoDetalle e) => new()
    {
        TratamientoDetalleID = e.TratamientoDetalleID,
        TratamientoID = e.TratamientoID,
        Tipo = e.Tipo,
        Descripcion = e.Descripcion,
        Frecuencia = e.Frecuencia,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    public static TratamientoDto ToDto(Tratamiento e) => new()
    {
        TratamientoID = e.TratamientoID,
        AtencionID = e.AtencionID,
        Nombre = e.Nombre,
        Objetivo = e.Objetivo,
        FechaInicio = e.FechaInicio,
        FechaFin = e.FechaFin,
        Estado = e.Estado,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt,
        Detalle = e.Detalle.Select(ToDto).ToList()
    };

    public static HabitacionDto ToDto(Habitacion e) => new()
    {
        HabitacionID = e.HabitacionID,
        Codigo = e.Codigo,
        Numero = e.Numero,
        Tipo = e.Tipo,
        Piso = e.Piso,
        AreaID = e.AreaID,
        Activa = e.Activa,
        CreatedAt = e.CreatedAt
    };

    public static CamaDto ToDto(Cama e) => new()
    {
        CamaID = e.CamaID,
        HabitacionID = e.HabitacionID,
        Codigo = e.Codigo,
        Estado = e.Estado,
        Activa = e.Activa,
        CreatedAt = e.CreatedAt
    };

    public static HospitalizacionDto ToDto(global::Clinical.Domain.Entities.Hospitalizacion e) => new()
    {
        HospitalizacionID = e.HospitalizacionID,
        ExpedienteClinicoID = e.ExpedienteClinicoID,
        AtencionID = e.AtencionID,
        CamaID = e.CamaID,
        FechaIngreso = e.FechaIngreso,
        FechaEgreso = e.FechaEgreso,
        Motivo = e.Motivo,
        Diagnostico = e.Diagnostico,
        Estado = e.Estado,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
