/* ============================================================================
   Semillas de Scheduling.Service sobre ClinicaDB
   ----------------------------------------------------------------------------
   Scheduling REUTILIZA las tablas existentes: este script NO crea ni altera
   esquema, solo carga datos de prueba. Es idempotente (guardas IF NOT EXISTS)
   y puede ejecutarse cuantas veces se quiera.

   Orden de inserción (las FKs lo exigen):
      1. Sucursales  (FK de Consultorios; tabla vacía)
      2. Areas       (FK de Consultorios; opcional pero se siembra)
      3. Roles + Usuarios (exigidos por UQ_Medicos_Usuario: solo admite 1 NULL,
                           y por FK de Medicos.UsuarioID -> Usuarios.RolID)
      4. Estados_Cita
      5. Especialidades
      6. Medicos  (+ Medico_Especialidad)
      7. Aseguradoras
      8. Consultorios (FK Sucursales + Areas)
      9. Pacientes
     10. Paciente_Aseguradora
     11. Citas       (FK Pacientes, Medicos, Especialidades, Consultorios,
                      Estados_Cita; UsuarioCreacionID = NULL, columna nullable:
                      los usuarios sembrados son de medicos, no de creacion)

   IDs fijos que quedan disponibles para probar la integración con Billing:
     Paciente 1 = Juan Perez  ->  Aseguradora 1 (Seguros Salud)
     Paciente 3 = Carlos Ruiz ->  Aseguradora 2 (Particular)
     Cita 1 -> paciente 1, PROGRAMADA  (válida para factura)
     Cita 2 -> paciente 2, ATENDIDA    (de otro paciente -> error 810)
     Cita 999 no existe                 -> error 809
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

/* -------------------------------------------------------- 1a. Sucursales */
/* FK obligatoria de Consultorios (SucursalID NOT NULL). Tabla estaba vacía. */
SET IDENTITY_INSERT dbo.Sucursales ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Sucursales WHERE SucursalID = 1)
    INSERT INTO dbo.Sucursales (SucursalID, Codigo, Nombre, Direccion, Telefono, Email, Activo, FechaCreacion)
    VALUES (1, 'SUC-01', 'Clínica Central', 'Av. Principal 100', '555-0301', 'central@clinica.local', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Sucursales WHERE SucursalID = 2)
    INSERT INTO dbo.Sucursales (SucursalID, Codigo, Nombre, Direccion, Telefono, Email, Activo, FechaCreacion)
    VALUES (2, 'SUC-02', 'Clínica Norte', 'Cra. Norte 45', '555-0302', 'norte@clinica.local', 1, SYSDATETIME());

SET IDENTITY_INSERT dbo.Sucursales OFF;

/* ----------------------------------------------------------- 1b. Areas */
SET IDENTITY_INSERT dbo.Areas ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Areas WHERE AreaID = 1)
    INSERT INTO dbo.Areas (AreaID, SucursalID, Codigo, Nombre, Activo)
    VALUES (1, 1, 'AMB', 'Ambulatorio', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Areas WHERE AreaID = 2)
    INSERT INTO dbo.Areas (AreaID, SucursalID, Codigo, Nombre, Activo)
    VALUES (2, 1, 'URG', 'Urgencias', 1);

SET IDENTITY_INSERT dbo.Areas OFF;

/* ------------------------------------------------- 1c. Roles y Usuarios */
/* Sembrados por requerimiento del esquema: Medicos.UsuarioID tiene una
   constraint UNIQUE sin filtro que admite UN SOLO NULL (lo usa el medico 1),
   asi que los medicos 2-6 necesitan UsuarioID con valores unicos y estos
   apuntan por FK a Usuarios.RolID. NO es autenticacion: es solo datos. */
SET IDENTITY_INSERT dbo.Roles ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RolID = 1)
    INSERT INTO dbo.Roles (RolID, Nombre, Descripcion, Activo)
    VALUES (1, 'Administrador', 'Acceso total al sistema', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RolID = 2)
    INSERT INTO dbo.Roles (RolID, Nombre, Descripcion, Activo)
    VALUES (2, 'Médico', 'Usuarios vinculados a medicos', 1);

SET IDENTITY_INSERT dbo.Roles OFF;

SET IDENTITY_INSERT dbo.Usuarios ON;

/* PasswordHash dummy: sin login real, nunca debe autenticar a nadie. */
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = 1)
    INSERT INTO dbo.Usuarios (UsuarioID, RolID, SucursalID, NombreUsuario, NombreCompleto, Email, PasswordHash, Activo, UltimoAcceso, FechaCreacion)
    VALUES (1, 2, 1, 'lcastro', 'Luis Fernando Castro', 'luis.castro@clinica.local', 'SEED-DUMMY-NO-USAR', 1, NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = 2)
    INSERT INTO dbo.Usuarios (UsuarioID, RolID, SucursalID, NombreUsuario, NombreCompleto, Email, PasswordHash, Activo, UltimoAcceso, FechaCreacion)
    VALUES (2, 2, 1, 'rmartinez', 'Rosa Elena Martínez', 'rosa.martinez@clinica.local', 'SEED-DUMMY-NO-USAR', 1, NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = 3)
    INSERT INTO dbo.Usuarios (UsuarioID, RolID, SucursalID, NombreUsuario, NombreCompleto, Email, PasswordHash, Activo, UltimoAcceso, FechaCreacion)
    VALUES (3, 2, 1, 'jdelgado', 'Jorge Iván Delgado', 'jorge.delgado@clinica.local', 'SEED-DUMMY-NO-USAR', 1, NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = 4)
    INSERT INTO dbo.Usuarios (UsuarioID, RolID, SucursalID, NombreUsuario, NombreCompleto, Email, PasswordHash, Activo, UltimoAcceso, FechaCreacion)
    VALUES (4, 2, 2, 'cherrera', 'Carla Herrera', 'carla.herrera@clinica.local', 'SEED-DUMMY-NO-USAR', 1, NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE UsuarioID = 5)
    INSERT INTO dbo.Usuarios (UsuarioID, RolID, SucursalID, NombreUsuario, NombreCompleto, Email, PasswordHash, Activo, UltimoAcceso, FechaCreacion)
    VALUES (5, 2, 2, 'mromero', 'Miguel Ángel Romero', 'miguel.romero@clinica.local', 'SEED-DUMMY-NO-USAR', 1, NULL, SYSDATETIME());

SET IDENTITY_INSERT dbo.Usuarios OFF;

/* ---------------------------------------------------------------- 1. Estados_Cita */
SET IDENTITY_INSERT dbo.Estados_Cita ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Cita WHERE EstadoCitaID = 1)
    INSERT INTO dbo.Estados_Cita (EstadoCitaID, Codigo, Nombre)
    VALUES (1, 'PROGRAMADA', 'Programada');

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Cita WHERE EstadoCitaID = 2)
    INSERT INTO dbo.Estados_Cita (EstadoCitaID, Codigo, Nombre)
    VALUES (2, 'ATENDIDA', 'Atendida');

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Cita WHERE EstadoCitaID = 3)
    INSERT INTO dbo.Estados_Cita (EstadoCitaID, Codigo, Nombre)
    VALUES (3, 'CANCELADA', 'Cancelada');

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Cita WHERE EstadoCitaID = 4)
    INSERT INTO dbo.Estados_Cita (EstadoCitaID, Codigo, Nombre)
    VALUES (4, 'NO_ASISTIO', 'No asistió');

SET IDENTITY_INSERT dbo.Estados_Cita OFF;

/* ----------------------------------------------------------- 2. Especialidades */
SET IDENTITY_INSERT dbo.Especialidades ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE EspecialidadID = 1)
    INSERT INTO dbo.Especialidades (EspecialidadID, Codigo, Nombre, Descripcion, Activo)
    VALUES (1, 'MEDGEN', 'Medicina General', 'Atención primaria y consultas generales', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE EspecialidadID = 2)
    INSERT INTO dbo.Especialidades (EspecialidadID, Codigo, Nombre, Descripcion, Activo)
    VALUES (2, 'PEDIAT', 'Pediatría', 'Atención de salud infantil', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE EspecialidadID = 3)
    INSERT INTO dbo.Especialidades (EspecialidadID, Codigo, Nombre, Descripcion, Activo)
    VALUES (3, 'CARDIO', 'Cardiología', 'Diagnóstico y tratamiento del corazón', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE EspecialidadID = 4)
    INSERT INTO dbo.Especialidades (EspecialidadID, Codigo, Nombre, Descripcion, Activo)
    VALUES (4, 'GINECO', 'Ginecología', 'Salud de la mujer', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE EspecialidadID = 5)
    INSERT INTO dbo.Especialidades (EspecialidadID, Codigo, Nombre, Descripcion, Activo)
    VALUES (5, 'DERMAT', 'Dermatología', 'Enfermedades de la piel', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE EspecialidadID = 6)
    INSERT INTO dbo.Especialidades (EspecialidadID, Codigo, Nombre, Descripcion, Activo)
    VALUES (6, 'TRAUMA', 'Traumatología', 'Huesos, articulaciones y músculos', 1);

SET IDENTITY_INSERT dbo.Especialidades OFF;

/* ------------------------------------------------------------------- 3. Medicos */
SET IDENTITY_INSERT dbo.Medicos ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE MedicoID = 1)
    INSERT INTO dbo.Medicos (MedicoID, UsuarioID, CodigoMedico, Nombres, Apellidos,
                            NumeroLicencia, Telefono, Email, Activo)
    VALUES (1, NULL, 'MED-001', 'Ana María', 'Gómez', 'LIC-12345', '555-0101', 'ana.gomez@clinica.local', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE MedicoID = 2)
    INSERT INTO dbo.Medicos (MedicoID, UsuarioID, CodigoMedico, Nombres, Apellidos,
                            NumeroLicencia, Telefono, Email, Activo)
    VALUES (2, 1, 'MED-002', 'Luis Fernando', 'Castro', 'LIC-23456', '555-0102', 'luis.castro@clinica.local', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE MedicoID = 3)
    INSERT INTO dbo.Medicos (MedicoID, UsuarioID, CodigoMedico, Nombres, Apellidos,
                            NumeroLicencia, Telefono, Email, Activo)
    VALUES (3, 2, 'MED-003', 'Rosa Elena', 'Martínez', 'LIC-34567', '555-0103', 'rosa.martinez@clinica.local', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE MedicoID = 4)
    INSERT INTO dbo.Medicos (MedicoID, UsuarioID, CodigoMedico, Nombres, Apellidos,
                            NumeroLicencia, Telefono, Email, Activo)
    VALUES (4, 3, 'MED-004', 'Jorge Iván', 'Delgado', 'LIC-45678', '555-0104', 'jorge.delgado@clinica.local', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE MedicoID = 5)
    INSERT INTO dbo.Medicos (MedicoID, UsuarioID, CodigoMedico, Nombres, Apellidos,
                            NumeroLicencia, Telefono, Email, Activo)
    VALUES (5, 4, 'MED-005', 'Carla', 'Herrera', 'LIC-56789', '555-0105', 'carla.herrera@clinica.local', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE MedicoID = 6)
    INSERT INTO dbo.Medicos (MedicoID, UsuarioID, CodigoMedico, Nombres, Apellidos,
                            NumeroLicencia, Telefono, Email, Activo)
    VALUES (6, 5, 'MED-006', 'Miguel Ángel', 'Romero', 'LIC-67890', '555-0106', 'miguel.romero@clinica.local', 1);

SET IDENTITY_INSERT dbo.Medicos OFF;

IF NOT EXISTS (SELECT 1 FROM dbo.Medico_Especialidad WHERE MedicoID = 1 AND EspecialidadID = 1)
    INSERT INTO dbo.Medico_Especialidad (MedicoID, EspecialidadID, Principal)
    VALUES (1, 1, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medico_Especialidad WHERE MedicoID = 2 AND EspecialidadID = 2)
    INSERT INTO dbo.Medico_Especialidad (MedicoID, EspecialidadID, Principal)
    VALUES (2, 2, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medico_Especialidad WHERE MedicoID = 3 AND EspecialidadID = 3)
    INSERT INTO dbo.Medico_Especialidad (MedicoID, EspecialidadID, Principal)
    VALUES (3, 3, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medico_Especialidad WHERE MedicoID = 4 AND EspecialidadID = 4)
    INSERT INTO dbo.Medico_Especialidad (MedicoID, EspecialidadID, Principal)
    VALUES (4, 4, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medico_Especialidad WHERE MedicoID = 5 AND EspecialidadID = 5)
    INSERT INTO dbo.Medico_Especialidad (MedicoID, EspecialidadID, Principal)
    VALUES (5, 5, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Medico_Especialidad WHERE MedicoID = 6 AND EspecialidadID = 6)
    INSERT INTO dbo.Medico_Especialidad (MedicoID, EspecialidadID, Principal)
    VALUES (6, 6, 1);

/* --------------------------------------------------------------- 4. Aseguradoras */
SET IDENTITY_INSERT dbo.Aseguradoras ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Aseguradoras WHERE AseguradoraID = 1)
    INSERT INTO dbo.Aseguradoras (AseguradoraID, Codigo, Nombre, NumeroFiscal, Telefono, Email, Direccion, Activo)
    VALUES (1, 'SEGSALUD', 'Seguros Salud S.A.', 'F-1001', '555-0201', 'contacto@segurossalud.local', 'Av. Principal 100', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Aseguradoras WHERE AseguradoraID = 2)
    INSERT INTO dbo.Aseguradoras (AseguradoraID, Codigo, Nombre, NumeroFiscal, Telefono, Email, Direccion, Activo)
    VALUES (2, 'PARTICULAR', 'Particular (sin seguro)', NULL, NULL, NULL, NULL, 1);

SET IDENTITY_INSERT dbo.Aseguradoras OFF;

/* -------------------------------------------------------------- 6b. Consultorios */
/* Primera carga: la tabla estaba vacía. FK a Sucursales (obligatoria) y Areas. */
SET IDENTITY_INSERT dbo.Consultorios ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorios WHERE ConsultorioID = 1)
    INSERT INTO dbo.Consultorios (ConsultorioID, SucursalID, AreaID, Codigo, Nombre, Piso, Activo)
    VALUES (1, 1, 1, 'CONS-01', 'Consultorio 1', '1', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorios WHERE ConsultorioID = 2)
    INSERT INTO dbo.Consultorios (ConsultorioID, SucursalID, AreaID, Codigo, Nombre, Piso, Activo)
    VALUES (2, 1, 1, 'CONS-02', 'Consultorio 2', '1', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorios WHERE ConsultorioID = 3)
    INSERT INTO dbo.Consultorios (ConsultorioID, SucursalID, AreaID, Codigo, Nombre, Piso, Activo)
    VALUES (3, 1, 2, 'CONS-03', 'Consultorio 3', '2', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorios WHERE ConsultorioID = 4)
    INSERT INTO dbo.Consultorios (ConsultorioID, SucursalID, AreaID, Codigo, Nombre, Piso, Activo)
    VALUES (4, 1, 2, 'CONS-04', 'Consultorio 4', '2', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorios WHERE ConsultorioID = 5)
    INSERT INTO dbo.Consultorios (ConsultorioID, SucursalID, AreaID, Codigo, Nombre, Piso, Activo)
    VALUES (5, 2, NULL, 'CONS-05', 'Consultorio 5', '1', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Consultorios WHERE ConsultorioID = 6)
    INSERT INTO dbo.Consultorios (ConsultorioID, SucursalID, AreaID, Codigo, Nombre, Piso, Activo)
    VALUES (6, 2, NULL, 'CONS-06', 'Consultorio 6', '2', 1);

SET IDENTITY_INSERT dbo.Consultorios OFF;

/* --------------------------------------------------------------- 5. Pacientes */
SET IDENTITY_INSERT dbo.Pacientes ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE PacienteID = 1)
    INSERT INTO dbo.Pacientes (PacienteID, CodigoPaciente, TipoDocumento, NumeroDocumento, Nombres, Apellidos,
                              FechaNacimiento, Sexo, EstadoCivil, Telefono, Email, Direccion, Ciudad, Pais,
                              Ocupacion, TipoSangre, Activo, FechaRegistro)
    VALUES (1, 'PAC-000001', 'DNI', '12345678', 'Juan', 'Pérez',
            '1990-05-20', 'M', 'Soltero', '555-1001', 'juan.perez@correo.local',
            'Calle 1 No. 10', 'Ciudad', 'País', 'Docente', 'O+', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE PacienteID = 2)
    INSERT INTO dbo.Pacientes (PacienteID, CodigoPaciente, TipoDocumento, NumeroDocumento, Nombres, Apellidos,
                              FechaNacimiento, Sexo, EstadoCivil, Telefono, Email, Direccion, Ciudad, Pais,
                              Ocupacion, TipoSangre, Activo, FechaRegistro)
    VALUES (2, 'PAC-000002', 'DNI', '87654321', 'María', 'López',
            '1985-03-15', 'F', 'Casada', '555-1002', 'maria.lopez@correo.local',
            'Calle 2 No. 20', 'Ciudad', 'País', 'Enfermera', 'A+', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE PacienteID = 3)
    INSERT INTO dbo.Pacientes (PacienteID, CodigoPaciente, TipoDocumento, NumeroDocumento, Nombres, Apellidos,
                              FechaNacimiento, Sexo, EstadoCivil, Telefono, Email, Direccion, Ciudad, Pais,
                              Ocupacion, TipoSangre, Activo, FechaRegistro)
    VALUES (3, 'PAC-000003', 'DNI', '11223344', 'Carlos', 'Ruiz',
            '1978-11-02', 'M', 'Casado', '555-1003', 'carlos.ruiz@correo.local',
            'Calle 3 No. 30', 'Ciudad', 'País', 'Operario', 'B+', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE PacienteID = 4)
    INSERT INTO dbo.Pacientes (PacienteID, CodigoPaciente, TipoDocumento, NumeroDocumento, Nombres, Apellidos,
                              FechaNacimiento, Sexo, EstadoCivil, Telefono, Email, Direccion, Ciudad, Pais,
                              Ocupacion, TipoSangre, Activo, FechaRegistro)
    VALUES (4, 'PAC-000004', 'DNI', '55667788', 'Lucía', 'Fernández',
            '1992-07-10', 'F', 'Soltera', '555-1004', 'lucia.fernandez@correo.local',
            'Calle 4 No. 40', 'Ciudad', 'País', 'Abogada', 'O-', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE PacienteID = 5)
    INSERT INTO dbo.Pacientes (PacienteID, CodigoPaciente, TipoDocumento, NumeroDocumento, Nombres, Apellidos,
                              FechaNacimiento, Sexo, EstadoCivil, Telefono, Email, Direccion, Ciudad, Pais,
                              Ocupacion, TipoSangre, Activo, FechaRegistro)
    VALUES (5, 'PAC-000005', 'DNI', '99887766', 'Diego', 'Morales',
            '2001-01-25', 'M', 'Soltero', '555-1005', 'diego.morales@correo.local',
            'Calle 5 No. 50', 'Ciudad', 'País', 'Estudiante', 'A-', 1, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE PacienteID = 6)
    INSERT INTO dbo.Pacientes (PacienteID, CodigoPaciente, TipoDocumento, NumeroDocumento, Nombres, Apellidos,
                              FechaNacimiento, Sexo, EstadoCivil, Telefono, Email, Direccion, Ciudad, Pais,
                              Ocupacion, TipoSangre, Activo, FechaRegistro)
    VALUES (6, 'PAC-000006', 'PASAPORTE', 'AB123456', 'Carmen', 'Rojas',
            '1965-09-30', 'F', 'Viuda', '555-1006', 'carmen.rojas@correo.local',
            'Calle 6 No. 60', 'Ciudad', 'País', 'Contadora', 'AB+', 1, SYSDATETIME());

SET IDENTITY_INSERT dbo.Pacientes OFF;

/* --------------------------------------------------- 6. Paciente_Aseguradora */
SET IDENTITY_INSERT dbo.Paciente_Aseguradora ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Paciente_Aseguradora WHERE PacienteAseguradoraID = 1)
    INSERT INTO dbo.Paciente_Aseguradora (PacienteAseguradoraID, PacienteID, AseguradoraID, NumeroPoliza,
                                         NumeroAfiliacion, TitularNombre, ParentescoTitular,
                                         FechaInicio, FechaFin, CoberturaPorcentaje, Activo)
    VALUES (1, 1, 1, 'POL-0001', 'AFI-0001', 'Juan Pérez', 'Titular',
            DATEADD(DAY, -180, CAST(GETDATE() AS DATE)), NULL, 80.00, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Paciente_Aseguradora WHERE PacienteAseguradoraID = 2)
    INSERT INTO dbo.Paciente_Aseguradora (PacienteAseguradoraID, PacienteID, AseguradoraID, NumeroPoliza,
                                         NumeroAfiliacion, TitularNombre, ParentescoTitular,
                                         FechaInicio, FechaFin, CoberturaPorcentaje, Activo)
    VALUES (2, 3, 2, 'POL-0002', NULL, NULL, NULL,
            NULL, NULL, NULL, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Paciente_Aseguradora WHERE PacienteAseguradoraID = 3)
    INSERT INTO dbo.Paciente_Aseguradora (PacienteAseguradoraID, PacienteID, AseguradoraID, NumeroPoliza,
                                         NumeroAfiliacion, TitularNombre, ParentescoTitular,
                                         FechaInicio, FechaFin, CoberturaPorcentaje, Activo)
    VALUES (3, 4, 1, 'POL-0003', 'AFI-0003', 'Lucía Fernández', 'Titular',
            DATEADD(DAY, -90, CAST(GETDATE() AS DATE)), NULL, 70.00, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Paciente_Aseguradora WHERE PacienteAseguradoraID = 4)
    INSERT INTO dbo.Paciente_Aseguradora (PacienteAseguradoraID, PacienteID, AseguradoraID, NumeroPoliza,
                                         NumeroAfiliacion, TitularNombre, ParentescoTitular,
                                         FechaInicio, FechaFin, CoberturaPorcentaje, Activo)
    VALUES (4, 6, 1, 'POL-0004', 'AFI-0004', 'Carmen Rojas', 'Titular',
            DATEADD(DAY, -30, CAST(GETDATE() AS DATE)), DATEADD(DAY, 335, CAST(GETDATE() AS DATE)), 60.00, 1);

SET IDENTITY_INSERT dbo.Paciente_Aseguradora OFF;

/* ------------------------------------------------------------------ 7. Citas */
SET IDENTITY_INSERT dbo.Citas ON;

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 1)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (1, 1, 1, 1, NULL, 1,
            '2026-11-20T09:00:00', '2026-11-20T09:30:00',
            'Consulta de control', NULL, NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 2)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (2, 2, 1, 1, NULL, 2,
            '2026-11-05T10:00:00', '2026-11-05T10:30:00',
            'Dolor de cabeza', 'Se indicó reposo.', NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 3)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (3, 3, 1, 1, NULL, 3,
            '2026-11-06T11:00:00', '2026-11-06T11:30:00',
            'Control anual', 'Cancelada por el paciente.', NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 4)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (4, 4, 3, 3, 3, 1,
            '2026-11-21T08:30:00', '2026-11-21T09:00:00',
            'Chequeo cardiológico anual', NULL, NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 5)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (5, 5, 2, 2, 1, 4,
            '2026-11-10T10:00:00', '2026-11-10T10:30:00',
            'Control pediátrico', 'El paciente no se presentó.', NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 6)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (6, 6, 6, 6, 5, 2,
            '2026-11-12T14:00:00', '2026-11-12T14:30:00',
            'Dolor lumbar', 'Se indicó radiografía y fisioterapia.', NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 7)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (7, 4, 4, 4, 4, 1,
            '2026-11-25T11:00:00', '2026-11-25T11:30:00',
            'Consulta de control ginecológico', NULL, NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 8)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (8, 5, 5, 5, 6, 3,
            '2026-11-13T16:00:00', '2026-11-13T16:30:00',
            'Revisión de lunares', 'Reprogramada por el médico.', NULL, SYSDATETIME());

IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE CitaID = 9)
    INSERT INTO dbo.Citas (CitaID, PacienteID, MedicoID, EspecialidadID, ConsultorioID, EstadoCitaID,
                          FechaHoraInicio, FechaHoraFin, Motivo, Observaciones, UsuarioCreacionID, FechaCreacion)
    VALUES (9, 6, 3, 3, 2, 1,
            '2026-11-28T09:00:00', '2026-11-28T09:30:00',
            'Seguimiento de presión arterial', NULL, NULL, SYSDATETIME());

SET IDENTITY_INSERT dbo.Citas OFF;

COMMIT TRANSACTION;

/* ------------------------------------------------------------ Verificación */
SELECT 'Sucursales'         AS Tabla, COUNT(*) AS Filas FROM dbo.Sucursales
UNION ALL SELECT 'Areas',               COUNT(*) FROM dbo.Areas
UNION ALL SELECT 'Roles',               COUNT(*) FROM dbo.Roles
UNION ALL SELECT 'Usuarios',            COUNT(*) FROM dbo.Usuarios
UNION ALL SELECT 'Estados_Cita',        COUNT(*) FROM dbo.Estados_Cita
UNION ALL SELECT 'Especialidades',      COUNT(*) FROM dbo.Especialidades
UNION ALL SELECT 'Medicos',             COUNT(*) FROM dbo.Medicos
UNION ALL SELECT 'Medico_Especialidad', COUNT(*) FROM dbo.Medico_Especialidad
UNION ALL SELECT 'Aseguradoras',        COUNT(*) FROM dbo.Aseguradoras
UNION ALL SELECT 'Consultorios',        COUNT(*) FROM dbo.Consultorios
UNION ALL SELECT 'Pacientes',           COUNT(*) FROM dbo.Pacientes
UNION ALL SELECT 'Paciente_Aseguradora',COUNT(*) FROM dbo.Paciente_Aseguradora
UNION ALL SELECT 'Citas',               COUNT(*) FROM dbo.Citas
ORDER BY Tabla;
