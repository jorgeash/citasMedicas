/* ============================================================================
   Semillas de Laboratory.Service sobre ClinicaDB
   ----------------------------------------------------------------------------
   REQUIERE que las tablas [Catálogo_Estudios], Solicitudes_Estudio,
   Solicitud_Estudio_Detalle y Resultados_Estudio ya existan en ClinicaDB.
   Este script NO crea ni altera esquema: solo carga datos de prueba y es
   idempotente (guardas IF NOT EXISTS + IDENTITY_INSERT). Laboratory.Service
   reutiliza ClinicaDB y NO tiene migraciones, al igual que Scheduling.Service.

   Catálogo sembrado:
     1 = Hemograma completo      (LABORATORIO, activo)
     2 = Glucosa en sangre       (LABORATORIO, activo, requiere ayuno)
     3 = Perfil lipidico         (LABORATORIO, activo, requiere ayuno)
     4 = Radiografia de torax    (IMAGEN, activo)
     5 = Biopsia                 (PATOLOGIA, activo)
     6 = Ecografia abdominal     (IMAGEN, INACTIVO -> prueba el error 1109)
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.[Catálogo_Estudios] ON;

IF NOT EXISTS (SELECT 1 FROM dbo.[Catálogo_Estudios] WHERE EstudioID = 1)
    INSERT INTO dbo.[Catálogo_Estudios]
        (EstudioID, Codigo, Nombre, Tipo, Descripcion, RequiereAyuno, Activo, Precio, CreatedAt)
    VALUES
        (1, N'HEM', N'Hemograma completo', N'LABORATORIO', N'Conteo sanguineo completo', 0, 1, 15.50, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.[Catálogo_Estudios] WHERE EstudioID = 2)
    INSERT INTO dbo.[Catálogo_Estudios]
        (EstudioID, Codigo, Nombre, Tipo, Descripcion, RequiereAyuno, Activo, Precio, CreatedAt)
    VALUES
        (2, N'GLU', N'Glucosa en sangre', N'LABORATORIO', N'Glucemia en ayunas', 1, 1, 9.00, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.[Catálogo_Estudios] WHERE EstudioID = 3)
    INSERT INTO dbo.[Catálogo_Estudios]
        (EstudioID, Codigo, Nombre, Tipo, Descripcion, RequiereAyuno, Activo, Precio, CreatedAt)
    VALUES
        (3, N'PERLIP', N'Perfil lipidico', N'LABORATORIO', N'Colesterol y trigliceridos', 1, 1, 35.00, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.[Catálogo_Estudios] WHERE EstudioID = 4)
    INSERT INTO dbo.[Catálogo_Estudios]
        (EstudioID, Codigo, Nombre, Tipo, Descripcion, RequiereAyuno, Activo, Precio, CreatedAt)
    VALUES
        (4, N'RXTOR', N'Radiografia de torax', N'IMAGEN', N'Radiografia simple de torax', 0, 1, 60.00, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.[Catálogo_Estudios] WHERE EstudioID = 5)
    INSERT INTO dbo.[Catálogo_Estudios]
        (EstudioID, Codigo, Nombre, Tipo, Descripcion, RequiereAyuno, Activo, Precio, CreatedAt)
    VALUES
        (5, N'BIOP', N'Biopsia', N'PATOLOGIA', N'Estudio histopatologico', 0, 1, 120.00, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.[Catálogo_Estudios] WHERE EstudioID = 6)
    INSERT INTO dbo.[Catálogo_Estudios]
        (EstudioID, Codigo, Nombre, Tipo, Descripcion, RequiereAyuno, Activo, Precio, CreatedAt)
    VALUES
        (6, N'ECOAB', N'Ecografia abdominal', N'IMAGEN', N'Estudio temporalmente fuera de servicio', 1, 0, 70.00, SYSDATETIME());

SET IDENTITY_INSERT dbo.[Catálogo_Estudios] OFF;

COMMIT TRANSACTION;
