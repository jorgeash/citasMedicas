/* ============================================================================
   Semillas de Identity.Service sobre ClinicaDB
   ----------------------------------------------------------------------------
   Identity REUTILIZA las tablas existentes: este script NO crea ni altera
   esquema, solo carga datos de acceso. Es idempotente (guardas IF NOT EXISTS)
   y puede ejecutarse cuantas veces se quiera.

   Orden de inserción (las FKs lo exigen):
      1. Permisos     (catálogo; tabla vacía)
      2. Roles        (agrega el rol 3 "Servicio"; roles 1 y 2 ya están)
      3. Rol_Permiso  (PK compuesta, guarda por fila)
      4. Usuarios     (admin real + cuenta de servicio + reemplaza los hashes
                       dummy de los 5 médicos sembrados por Scheduling)

   CREDENCIALES DE PRUEBA (documentadas a propósito; es un entorno local):
      admin         / Admin123!      rol Administrador (UsuarioID 6)
      svc-billing   / BillingSvc123! rol Servicio      (UsuarioID 7)
      lcastro, rmartinez, jdelgado, cherrera, mromero / Medico123!  rol Médico

   Los hashes son PBKDF2-SHA256 (100 000 iteraciones, sal de 16 bytes) con el
   mismo formato que genera Identity.Core.Services.PasswordHasher:
      PBKDF2$<iteraciones>$<saltBase64>$<hashBase64>

   NOTA DE SEGURIDAD: en producción estos hashes NO se siembran desde un script
   versionado; el alta de usuarios se haría por API con política de contraseñas.
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

/* ------------------------------------------------------------- 1. Permisos */
SET IDENTITY_INSERT dbo.Permisos ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 1)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (1, 'facturas.leer',      N'Ver facturas',            N'Consulta el listado y el detalle de facturas');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 2)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (2, 'facturas.crear',     N'Crear facturas',          N'Emite facturas nuevas a partir de una cita');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 3)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (3, 'facturas.anular',    N'Anular facturas',         N'Anula facturas con motivo');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 4)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (4, 'servicios.leer',     N'Ver servicios',           N'Consulta el catálogo de servicios');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 5)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (5, 'tarifas.leer',       N'Ver tarifas',             N'Consulta las tarifas vigentes');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 6)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (6, 'pacientes.leer',     N'Ver pacientes',           N'Consulta el padrón de pacientes');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 7)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (7, 'pacientes.crear',    N'Crear pacientes',         N'Registra pacientes nuevos');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 8)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (8, 'citas.leer',         N'Ver citas',               N'Consulta la agenda de citas');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 9)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (9, 'citas.crear',        N'Crear citas',             N'Agenda citas nuevas');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 10)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (10, 'citas.cancelar',    N'Cancelar citas',          N'Cancela citas de la agenda');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 11)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (11, 'aseguradoras.leer', N'Ver aseguradoras',        N'Consulta el directorio de aseguradoras');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 12)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (12, 'medicos.leer',      N'Ver médicos',             N'Consulta el directorio de médicos');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 13)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (13, 'usuarios.gestionar', N'Gestionar usuarios',     N'Alta y edición de usuarios del sistema');
IF NOT EXISTS (SELECT 1 FROM dbo.Permisos WHERE PermisoID = 14)
    INSERT INTO dbo.Permisos (PermisoID, Codigo, Nombre, Descripcion)
    VALUES (14, 'permisos.gestionar', N'Gestionar permisos',     N'Asigna permisos a los roles');

SET IDENTITY_INSERT dbo.Permisos OFF;

/* --------------------------------------------------------------- 2. Roles */
/* Roles 1 (Administrador) y 2 (Médico) ya los creó seed-scheduling.sql. */
SET IDENTITY_INSERT dbo.Roles ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RolID = 3)
    INSERT INTO dbo.Roles (RolID, Nombre, Descripcion, Activo)
    VALUES (3, 'Servicio', N'Cuentas de servidor a servidor (sin interactividad)', 1);

SET IDENTITY_INSERT dbo.Roles OFF;

/* --------------------------------------------------------- 3. Rol_Permiso */
/* PK compuesta (RolID, PermisoID): la guarda es por fila. */

/* Administrador → los 14 permisos. */
INSERT INTO dbo.Rol_Permiso (RolID, PermisoID)
SELECT r.RolID, p.PermisoID
FROM dbo.Roles r
CROSS JOIN dbo.Permisos p
WHERE r.RolID = 1
  AND NOT EXISTS (
      SELECT 1 FROM dbo.Rol_Permiso rp
      WHERE rp.RolID = r.RolID AND rp.PermisoID = p.PermisoID);

/* Médico → solo lectura + agenda (9 de 14; no crea usuarios ni anula facturas). */
INSERT INTO dbo.Rol_Permiso (RolID, PermisoID)
SELECT 2, p.PermisoID
FROM dbo.Permisos p
WHERE p.Codigo IN (
        'facturas.leer', 'servicios.leer', 'tarifas.leer',
        'pacientes.leer', 'pacientes.crear',
        'citas.leer', 'citas.crear', 'citas.cancelar',
        'aseguradoras.leer', 'medicos.leer')
  AND NOT EXISTS (
      SELECT 1 FROM dbo.Rol_Permiso rp
      WHERE rp.RolID = 2 AND rp.PermisoID = p.PermisoID);

/* Servicio (Billing llamando a Scheduling) → solo lectura de lo que consume. */
INSERT INTO dbo.Rol_Permiso (RolID, PermisoID)
SELECT 3, p.PermisoID
FROM dbo.Permisos p
WHERE p.Codigo IN ('pacientes.leer', 'citas.leer')
  AND NOT EXISTS (
      SELECT 1 FROM dbo.Rol_Permiso rp
      WHERE rp.RolID = 3 AND rp.PermisoID = p.PermisoID);

/* ------------------------------------------------------------- 4. Usuarios */
SET IDENTITY_INSERT dbo.Usuarios ON;

/* UsuarioID 6 = administrador: quien entra a la app. */
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = 6)
    INSERT INTO dbo.Usuarios (UsuarioID, RolID, SucursalID, NombreUsuario, NombreCompleto, Email, PasswordHash, Activo, UltimoAcceso, FechaCreacion)
    VALUES (6, 1, 1, 'admin', N'Administrador del Sistema', 'admin@clinica.local',
            'PBKDF2$100000$GBtI0xs4194YdZ7dsE+3gg==$e2/BNu4Ox+tNdUB2Aw5HpEGpVeq0hCItv2V3/DHb6q8=',
            1, NULL, SYSDATETIME());

/* UsuarioID 7 = cuenta de servicio: Billing se autentica contra Scheduling. */
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = 7)
    INSERT INTO dbo.Usuarios (UsuarioID, RolID, SucursalID, NombreUsuario, NombreCompleto, Email, PasswordHash, Activo, UltimoAcceso, FechaCreacion)
    VALUES (7, 3, NULL, 'svc-billing', N'Servicio Billing', NULL,
            'PBKDF2$100000$yJRsH5JMRBLMKLzHuOc9oA==$QBYzfKdB+dtoirhNvd2qv040rf9krzCHn4saoEhwnDY=',
            1, NULL, SYSDATETIME());

SET IDENTITY_INSERT dbo.Usuarios OFF;

/* Los 5 médicos sembrados por Scheduling traían PasswordHash dummy
   'SEED-DUMMY-NO-USAR' (no autenticaba a nadie). Se reemplaza por un hash
   PBKDF2 real de Medico123!, con sal propia por usuario. */
UPDATE dbo.Usuarios SET PasswordHash = 'PBKDF2$100000$CsUimg8VG4zhEU1kHVwXbw==$yJsfcOmO72db5fZari3DYKtc2TPh6sW5K4/gaIEKmb4='
 WHERE NombreUsuario = 'lcastro'   AND PasswordHash = 'SEED-DUMMY-NO-USAR';

UPDATE dbo.Usuarios SET PasswordHash = 'PBKDF2$100000$WjzogrTvmu2s/fZOkSpnsA==$Ur07cg4XIuYd8oTWcI2GbU6+rxYNj68NLbfHmOfwoFA='
 WHERE NombreUsuario = 'rmartinez' AND PasswordHash = 'SEED-DUMMY-NO-USAR';

UPDATE dbo.Usuarios SET PasswordHash = 'PBKDF2$100000$1KZVGVtpdq7bu8DIcYJvgg==$1sGkaNaqQSFM+6ulV02Y7ZYPtVc7MJvd/5ZdmbNWVgY='
 WHERE NombreUsuario = 'jdelgado'  AND PasswordHash = 'SEED-DUMMY-NO-USAR';

UPDATE dbo.Usuarios SET PasswordHash = 'PBKDF2$100000$IlOtTYHVL5LEyEW3T4m37g==$3uvitfaCPmawI6f38ToBIL5C/9HvshmQZFT+SPgQHkY='
 WHERE NombreUsuario = 'cherrera'  AND PasswordHash = 'SEED-DUMMY-NO-USAR';

UPDATE dbo.Usuarios SET PasswordHash = 'PBKDF2$100000$QbozW2eqKsaqjcspdE2AkQ==$4QKvN6DwRq1JQ1FeRq1yTUYa5AKm/nsTeyhjfSW/Qm8='
 WHERE NombreUsuario = 'mromero'   AND PasswordHash = 'SEED-DUMMY-NO-USAR';

COMMIT TRANSACTION;

/* ------------------------------------------------------------ Verificación */
SELECT 'Permisos'      AS Tabla, COUNT(*) AS Filas FROM dbo.Permisos
UNION ALL SELECT 'Roles',         COUNT(*) FROM dbo.Roles
UNION ALL SELECT 'Rol_Permiso',   COUNT(*) FROM dbo.Rol_Permiso
UNION ALL SELECT 'Usuarios',      COUNT(*) FROM dbo.Usuarios
UNION ALL SELECT 'Sesiones_Usuario', COUNT(*) FROM dbo.Sesiones_Usuario
ORDER BY Tabla;

/* Comprobación de que ningún usuario quedó con el hash dummy. */
SELECT COUNT(*) AS UsuariosSinHashValido
FROM dbo.Usuarios
WHERE PasswordHash = 'SEED-DUMMY-NO-USAR'
   OR PasswordHash NOT LIKE 'PBKDF2$%';
