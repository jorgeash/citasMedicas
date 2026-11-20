/* ============================================================================
   Semillas de ClinicalRecords.Service sobre ClinicaDB
   ----------------------------------------------------------------------------
   Clinical REUTILIZA las tablas existentes: este script NO crea ni altera
   esquema, solo carga datos de prueba. Es idempotente (guardas IF NOT EXISTS)
   y puede ejecutarse cuantas veces se quiera.

   Requisito: ClinicaDB ya debe tener el esquema de las 25 tablas clínicas
   (no se incluye DDL). Orden de inserción: catálogos -> habitación/cama ->
   expediente, para respetar llaves foráneas.

   IDs fijos disponibles para probar la API:
     Diagnóstico 1 = I10 Hipertensión arterial
     Síntoma 1     = S-CAB Dolores de cabeza
     Medicamento 1 = MED-PARA Paracetamol
     Procedimiento 1 = PROC-TA Toma de temperatura
     Habitación 1 / Cama 1 = C-101, estado LIBRE (lista para hospitalización)
     Expediente 1 = paciente 1 (Juan Pérez), número EXP-2026-000001
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

/* ------------------------------------------------------- 1. Diagnósticos */
SET IDENTITY_INSERT [dbo].[Diagnósticos] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Diagnósticos] WHERE DiagnosticoID = 1)
    INSERT INTO [dbo].[Diagnósticos] (DiagnosticoID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (1, 'I10', 'Hipertensión arterial', 'Presión arterial alta persistente', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Diagnósticos] WHERE DiagnosticoID = 2)
    INSERT INTO [dbo].[Diagnósticos] (DiagnosticoID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (2, 'E11', 'Diabetes mellitus tipo 2', 'Trastorno metabólico de glucosa', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Diagnósticos] WHERE DiagnosticoID = 3)
    INSERT INTO [dbo].[Diagnósticos] (DiagnosticoID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (3, 'J06', 'Infección respiratoria aguda', 'Cuadro respiratorio leve', 1, SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Diagnósticos] OFF;

/* ---------------------------------------------------------- 2. Síntomas */
SET IDENTITY_INSERT [dbo].[Síntomas] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Síntomas] WHERE SintomaID = 1)
    INSERT INTO [dbo].[Síntomas] (SintomaID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (1, 'S-CAB', 'Dolor de cabeza', 'Cefalea de intensidad leve a moderada', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Síntomas] WHERE SintomaID = 2)
    INSERT INTO [dbo].[Síntomas] (SintomaID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (2, 'S-FIE', 'Fiebre', 'Elevación de la temperatura corporal', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Síntomas] WHERE SintomaID = 3)
    INSERT INTO [dbo].[Síntomas] (SintomaID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (3, 'S-TOS', 'Tos', 'Tos seca o productiva', 1, SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Síntomas] OFF;

/* ------------------------------------------------------ 3. Medicamentos */
SET IDENTITY_INSERT [dbo].[Medicamentos] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Medicamentos] WHERE MedicamentoID = 1)
    INSERT INTO [dbo].[Medicamentos] (MedicamentoID, Codigo, Nombre, Presentacion, Concentracion, ViaAdministracion, Activo, CreatedAt)
    VALUES (1, 'MED-PARA', 'Paracetamol', 'Tableta', '500 mg', 'Oral', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Medicamentos] WHERE MedicamentoID = 2)
    INSERT INTO [dbo].[Medicamentos] (MedicamentoID, Codigo, Nombre, Presentacion, Concentracion, ViaAdministracion, Activo, CreatedAt)
    VALUES (2, 'MED-IBU', 'Ibuprofeno', 'Tableta', '400 mg', 'Oral', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Medicamentos] WHERE MedicamentoID = 3)
    INSERT INTO [dbo].[Medicamentos] (MedicamentoID, Codigo, Nombre, Presentacion, Concentracion, ViaAdministracion, Activo, CreatedAt)
    VALUES (3, 'MED-SF', 'Solución fisiológica', 'Frasco', '0.9%', 'Tópica', 1, SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Medicamentos] OFF;

/* ----------------------------------------------------- 4. Procedimientos */
SET IDENTITY_INSERT [dbo].[Procedimientos] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Procedimientos] WHERE ProcedimientoID = 1)
    INSERT INTO [dbo].[Procedimientos] (ProcedimientoID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (1, 'PROC-TA', 'Toma de temperatura', 'Medición de temperatura corporal', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Procedimientos] WHERE ProcedimientoID = 2)
    INSERT INTO [dbo].[Procedimientos] (ProcedimientoID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (2, 'PROC-PA', 'Toma de presión arterial', 'Medición de presión sistólica y diastólica', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Procedimientos] WHERE ProcedimientoID = 3)
    INSERT INTO [dbo].[Procedimientos] (ProcedimientoID, Codigo, Nombre, Descripcion, Activo, CreatedAt)
    VALUES (3, 'PROC-ECG', 'Electrocardiograma', 'Registro gráfico de la actividad eléctrica cardíaca', 1, SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Procedimientos] OFF;

/* ------------------------------------------------------ 5. Habitaciones */
SET IDENTITY_INSERT [dbo].[Habitaciones] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Habitaciones] WHERE HabitacionID = 1)
    INSERT INTO [dbo].[Habitaciones] (HabitacionID, Codigo, Numero, Tipo, Piso, AreaID, Activa, CreatedAt)
    VALUES (1, 'HAB-101', '101', 'Individual', '1', NULL, 1, SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Habitaciones] OFF;

/* ------------------------------------------------------------- 6. Camas */
SET IDENTITY_INSERT [dbo].[Camas] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Camas] WHERE CamaID = 1)
    INSERT INTO [dbo].[Camas] (CamaID, HabitacionID, Codigo, Estado, Activa, CreatedAt)
    VALUES (1, 1, 'C-101', 'LIBRE', 1, SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Camas] OFF;

/* ------------------------------------------------- 7. Expedientes_Clínicos */
SET IDENTITY_INSERT [dbo].[Expedientes_Clínicos] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Expedientes_Clínicos] WHERE ExpedienteClinicoID = 1)
    INSERT INTO [dbo].[Expedientes_Clínicos] (ExpedienteClinicoID, PacienteID, NumeroExpediente, FechaApertura, Observaciones, Activo, CreatedAt)
    VALUES (1, 1, 'EXP-2026-000001', '2026-11-19', 'Expediente inicial para pruebas de integración', 1, SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Expedientes_Clínicos] OFF;

COMMIT TRANSACTION;

/* ------------------------------------------------------------ Verificación */
SELECT 'Diagnósticos' AS Tabla, COUNT(*) AS Filas FROM [dbo].[Diagnósticos]
UNION ALL SELECT 'Síntomas',                 COUNT(*) FROM [dbo].[Síntomas]
UNION ALL SELECT 'Medicamentos',             COUNT(*) FROM [dbo].[Medicamentos]
UNION ALL SELECT 'Procedimientos',           COUNT(*) FROM [dbo].[Procedimientos]
UNION ALL SELECT 'Habitaciones',             COUNT(*) FROM [dbo].[Habitaciones]
UNION ALL SELECT 'Camas',                    COUNT(*) FROM [dbo].[Camas]
UNION ALL SELECT 'Expedientes_Clínicos',     COUNT(*) FROM [dbo].[Expedientes_Clínicos]
ORDER BY Tabla;
