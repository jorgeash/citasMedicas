/* ============================================================================
   Semillas de Notification.Service sobre ClinicaDB
   ----------------------------------------------------------------------------
   REQUIERE que la tabla [dbo].[Notificaciones] ya exista en ClinicaDB.
   Este script NO crea ni altera esquema: solo carga datos de prueba y es
   idempotente (guardas IF NOT EXISTS + IDENTITY_INSERT). Notification.Service
   reutiliza ClinicaDB y NO tiene migraciones, al igual que Laboratory.Service.

   Filas sembradas:
     1 = confirmación de cita   (CITA, INTERNO, PENDIENTE, SCHEDULING)
     2 = recordatorio de cita   (RECORDATORIO, EMAIL, ENVIADA, SCHEDULING)
     3 = resultado disponible   (RESULTADO, INTERNO, LEIDA, LABORATORY)
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

SET IDENTITY_INSERT [dbo].[Notificaciones] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notificaciones] WHERE NotificacionID = 1)
    INSERT INTO [dbo].[Notificaciones]
        (NotificacionID, EventoID, UsuarioID, Destinatario, Titulo, Mensaje,
         Tipo, Canal, Prioridad, Estado, OrigenServicio, Referencia,
         FechaCreacion, FechaProgramada, FechaEnvio, FechaLectura,
         FechaCancelacion, MotivoCancelacion, Intentos, UltimoError,
         Activo, CreatedAt, UpdatedAt)
    VALUES
        (1, N'CITA-2026-000001', 1, NULL, N'Confirmación de cita',
         N'Su cita ha sido confirmada en el consultorio.',
         N'CITA', N'INTERNO', N'NORMAL', N'PENDIENTE', N'SCHEDULING', N'CITA-1',
         SYSDATETIME(), NULL, NULL, NULL,
         NULL, NULL, 0, NULL,
         1, SYSDATETIME(), NULL);

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notificaciones] WHERE NotificacionID = 2)
    INSERT INTO [dbo].[Notificaciones]
        (NotificacionID, EventoID, UsuarioID, Destinatario, Titulo, Mensaje,
         Tipo, Canal, Prioridad, Estado, OrigenServicio, Referencia,
         FechaCreacion, FechaProgramada, FechaEnvio, FechaLectura,
         FechaCancelacion, MotivoCancelacion, Intentos, UltimoError,
         Activo, CreatedAt, UpdatedAt)
    VALUES
        (2, N'RECORDATORIO-2026-000001', 1, N'paciente@example.com', N'Recordatorio de cita',
         N'Le recordamos su cita programada para mañana.',
         N'RECORDATORIO', N'EMAIL', N'NORMAL', N'ENVIADA', N'SCHEDULING', N'CITA-1',
         SYSDATETIME(), NULL, SYSDATETIME(), NULL,
         NULL, NULL, 1, NULL,
         1, SYSDATETIME(), SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM [dbo].[Notificaciones] WHERE NotificacionID = 3)
    INSERT INTO [dbo].[Notificaciones]
        (NotificacionID, EventoID, UsuarioID, Destinatario, Titulo, Mensaje,
         Tipo, Canal, Prioridad, Estado, OrigenServicio, Referencia,
         FechaCreacion, FechaProgramada, FechaEnvio, FechaLectura,
         FechaCancelacion, MotivoCancelacion, Intentos, UltimoError,
         Activo, CreatedAt, UpdatedAt)
    VALUES
        (3, N'RESULTADO-2026-000001', 1, NULL, N'Resultado disponible',
         N'Su resultado de laboratorio ya está disponible.',
         N'RESULTADO', N'INTERNO', N'ALTA', N'LEIDA', N'LABORATORY', N'ESTUDIO-1',
         SYSDATETIME(), NULL, SYSDATETIME(), SYSDATETIME(),
         NULL, NULL, 1, NULL,
         1, SYSDATETIME(), SYSDATETIME());

SET IDENTITY_INSERT [dbo].[Notificaciones] OFF;

COMMIT TRANSACTION;

SELECT COUNT(*) AS TotalNotificacionesSeed
FROM [dbo].[Notificaciones]
WHERE NotificacionID IN (1, 2, 3);
