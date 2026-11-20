# Propuesta: Microservicios — Sistema de Gestión Clínica

**Backend:** C# / .NET 9 — Arquitectura Hexagonal — Microservicios
**Frontend:** Vue 3

---

## 1. Distribución de 60 tablas en 6 microservicios

| # | Microservicio | Tablas | Pregunta que responde | Estado |
|---|---|---|---|---|
| 1 | **Identity.Service** | 13 | ¿QUIÉN es esta persona y QUÉ puede hacer? | Ya |
| 2 | **Scheduling.Service** | 9 | ¿CUÁNDO y DÓNDE ocurre la atención? | Ya |
| 3 | **ClinicalRecords.Service** | 25 | ¿QUÉ pasó médicamente en la atención? | Ya |
| 4 | **Laboratory.Service** | 4 | ¿QUÉ estudios se pidieron y QUÉ mostraron? | Ya |
| 5 | **Billing.Service** | 8 | ¿CUÁNTO se cobra y se PAGÓ? | Ya |
| 6 | **Notification.Service** | 1 | ¿Qué notificaciones se generan? | Ya |

---

## 2. Tablas por microservicio

### Identity.Service (13 tablas)

| # | Tabla | Tipo |
|---|---|---|
| 1 | Usuarios | Seguridad |
| 2 | Roles | Seguridad |
| 3 | Permisos | Seguridad |
| 4 | Rol_Permiso | Seguridad |
| 5 | Médicos | Catálogo |
| 6 | Especialidades | Catálogo |
| 7 | Médico_Especialidad | Relación |
| 8 | Sucursales | Organización |
| 9 | Áreas | Organización |
| 10 | Catálogos | Configuración |
| 11 | Estados | Configuración |
| 12 | Auditoría | Transversal |
| 13 | Sesiones_Usuario | Seguridad |

### Scheduling.Service (9 tablas)

| # | Tabla | Tipo |
|---|---|---|
| 14 | Pacientes | Negocio |
| 15 | Contactos_Emergencia | Negocio |
| 16 | Aseguradoras | Catálogo |
| 17 | Paciente_Aseguradora | Relación |
| 18 | Consultorios | Recurso |
| 19 | Horarios_Médicos | Agenda |
| 20 | Citas | Negocio |
| 21 | Estados_Cita | Configuración |
| 22 | Turnos | Negocio |

### ClinicalRecords.Service (25 tablas)

| # | Tabla | Tipo |
|---|---|---|
| 23 | Expedientes_Clínicos | Negocio |
| 24 | Antecedentes_Médicos | Negocio |
| 25 | Antecedentes_Familiares | Negocio |
| 26 | Alergias | Negocio |
| 27 | Signos_Vitales | Negocio |
| 28 | Hábitos | Negocio |
| 29 | Atenciones | Negocio |
| 30 | Notas_Médicas | Negocio |
| 31 | Diagnósticos | Catálogo |
| 32 | Atención_Diagnóstico | Relación |
| 33 | Síntomas | Catálogo |
| 34 | Atención_Síntoma | Relación |
| 35 | Medicamentos | Catálogo |
| 36 | Prescripciones | Negocio |
| 37 | Prescripción_Detalle | Negocio |
| 38 | Tratamientos | Negocio |
| 39 | Tratamiento_Detalle | Negocio |
| 40 | Indicaciones_Médicas | Negocio |
| 41 | Referencias_Médicas | Negocio |
| 42 | Procedimientos | Catálogo |
| 43 | Atención_Procedimiento | Relación |
| 44 | Archivos_Clínicos | Negocio |
| 45 | Habitaciones | Recurso |
| 46 | Camas | Recurso |
| 47 | Hospitalizaciones | Negocio |

### Laboratory.Service (4 tablas)

| # | Tabla | Tipo |
|---|---|---|
| 48 | Catálogo_Estudios | Catálogo |
| 49 | Solicitudes_Estudio | Negocio |
| 50 | Solicitud_Estudio_Detalle | Negocio |
| 51 | Resultados_Estudio | Negocio |

### Billing.Service (8 tablas)

| # | Tabla | Tipo |
|---|---|---|
| 52 | Servicios | Catálogo |
| 53 | Tarifas | Catálogo |
| 54 | Facturas | Negocio |
| 55 | Factura_Detalle | Negocio |
| 56 | Formas_Pago | Catálogo |
| 57 | Pagos | Negocio |
| 58 | Créditos_Médicos | Negocio |

### Notification.Service (1 tabla)

| # | Tabla | Tipo |
|---|---|---|
| 59 | Notificaciones | Negocio |

---

## 3. Estructura del proyecto

### Árbol general

```
citasMedicas/
├── src/
│   ├── Shared.Kernel/                     (IRepository<T>, Repository<T>, Filter, PagedResult)
│   │
│   ├── Identity.Service/                  (13 tablas)
│   │   ├── Identity.Domain/
│   │   ├── Identity.Core/
│   │   ├── Identity.Persistence/
│   │   ├── Identity.Api/                  :52096
│   │   └── Identity.Tests/
│   │
│   ├── Scheduling.Service/                (9 tablas)
│   │   ├── Scheduling.Domain/
│   │   ├── Scheduling.Core/
│   │   ├── Scheduling.Persistence/
│   │   ├── Scheduling.Api/                :52094 / :52095
│   │   └── Scheduling.Tests/
│   │
│   ├── ClinicalRecords.Service/           (25 tablas)
│   │   ├── Clinical.Domain/
│   │   ├── Clinical.Core/
│   │   ├── Clinical.Persistence/
│   │   ├── Clinical.Infrastructure/
│   │   ├── Clinical.Api/                  :52097
│   │   ├── Clinical.Tests/
│   │   ├── README.md
│   │   └── seed-clinical.sql
│   │
│   ├── Laboratory.Service/                (4 tablas)
│   │   ├── Lab.Domain/
│   │   ├── Lab.Core/
│   │   ├── Lab.Persistence/
│   │   ├── Lab.Infrastructure/
│   │   ├── Lab.Api/                       :52098
│   │   ├── Lab.Tests/
│   │   ├── README.md
│   │   └── seed-laboratory.sql
│   │
│   ├── Notification.Service/              (1 tabla, sin Infrastructure)
│   │   ├── Notification.Domain/
│   │   ├── Notification.Core/
│   │   ├── Notification.Persistence/
│   │   ├── Notification.Api/              :52099 / :52100
│   │   ├── Notification.Tests/
│   │   ├── README.md
│   │   └── seed-notifications.sql
│   │
│   └── Billing.Service/                   (8 tablas)
│       ├── Billing.Domain/
│       ├── Billing.Core/
│       ├── Billing.Persistence/
│       ├── Billing.Infrastructure/
│       ├── Billing.Api/                   :52092 / :52093
│       └── Billing.Tests/
│
├── citasMedicas.slnx
└── propuesta-microservicios.md
```

### Estructura hexagonal interna de cada servicio

```
Servicio.Domain/
├── Entities/          Entidades del dominio
├── Enums/             Enumeraciones
├── ValueObjects/      Objetos de valor
└── Interfaces/        Puertos (repositorios, servicios externos)

Servicio.Core/
├── Features/          Vertical slice: Command/Query + Handler en el MISMO archivo
│   └── AgrupadoPorDominio/
│       ├── Commands/<Accion>/<Accion>Command.cs
│       └── Queries/<Accion>/<Accion>Query.cs
├── DTOs/              Request/Response + DTOs externos de otros servicios
├── Exceptions/        DomainException + enum Errores (rango propio por micro)
├── Interfaces/        Puertos de repositorio y de servicios externos
├── Services/          Contratos de los clientes HTTP (I...Client)
└── DependencyInjection.cs

Servicio.Persistence/
├── Context/           DbContext
├── Configurations/    Fluent API (ToTable exacto, longitudes, precisión)
├── Repositories/      Implementación sobre Shared.Kernel.Repository<T>
└── Extensions/        DI registration

Servicio.Infrastructure/   (solo cuando hay comunicación entre servicios)
├── Services/          Clientes HTTP typed + stubs + ServiceTokenHandler
└── Extensions/        DI registration

Servicio.Api/
├── Controllers/       Endpoints HTTP (envían MediatR)
├── Program.cs         JWT, CORS, OpenAPI, middleware de errores
├── appsettings.json
└── Properties/launchSettings.json

Servicio.Tests/
├── Fakes/             InMemoryRepository<T>, repositorios falsos y stubs
└── Tests/             Pruebas xUnit de reglas de negocio (sin base de datos)
```

---

## 4. Shared.Kernel

Biblioteca compartida que NO es un microservicio. Contiene código reutilizable por todos los servicios:

- **BaseEntity.cs** — Clase base con Id, CreatedAt, UpdatedAt, IsActive
- **IUnitOfWork.cs** — Interfaz para SaveChanges
- **IAuditable.cs** — Interfaz para auditoría de creación/modificación
- **AuthExtensions.cs** — Extensiones para autenticación JWT
- **BaseCommand.cs** — Clase base para Commands MediatR (escritura)
- **BaseQuery.cs** — Clase base para Queries MediatR (lectura)

---

## 5. Comunicación entre servicios

Comunicación síncrona vía HTTP REST usando `IHttpClientFactory`. Sin messaging por ahora.
Por ahora se ha considerado esta opcion
| Servicio que llama | Servicio destino | Motivo |
|---|---|---|
| Scheduling | Identity | Datos del médico al agendar |
| ClinicalRecords | Identity | Usuario que atiende |
| ClinicalRecords | Scheduling | Datos de la cita y paciente |
| Laboratory | ClinicalRecords | Diagnósticos para el estudio |
| Laboratory | Identity | Médico que solicitó |
| Billing | Scheduling | Datos del paciente y cita |
| Billing | ClinicalRecords | Procedimientos realizados |
| Todos los servicios | Notification | Registro idempotente de avisos (`POST /api/notificaciones`) |

Los clientes HTTP de ClinicalRecords y Laboratory se registran con `IHttpClientFactory`.
Cuando `Services:<Destino>:BaseUrl` está vacío (valor por defecto en `appsettings.json`)
se activa un **stub** y el microservicio arranca y se prueba sin red. Con una URL real
se usa el cliente typed y `ServiceTokenHandler` obtiene un JWT de `Identity.Api`
mediante una cuenta de servicio configurada, con renovación y reintento ante `401`.

Notification es el único caso entrante: los productores hacen un `POST` HTTP con un
`EventoID` y la API devuelve la notificación ya existente si el evento se repite.
No hay bus de mensajes ni clientes salientes en Notification.

### Diagrama de dependencias

```
Identity          ← no depende de nadie
    ↑
Scheduling        ← llama a Identity
    ↑
ClinicalRecords   ← llama a Identity + Scheduling
    ↑
Laboratory        ← llama a ClinicalRecords + Identity
    ↑
Billing           ← llama a Scheduling + ClinicalRecords
    ↑
Notification      ← recibe eventos de todos
```

Flujo unidireccional. Sin dependencias circulares.

---

## 6. Base de datos

| Decisión | Valor |
|---|---|
| Motor | SQL Server (LocalDB) |
| Patrón | Shared database por convención: cada servicio mapea solo sus tablas |
| Nombre | `ClinicaDB` |

Identity, Scheduling, ClinicalRecords, Laboratory y Notification reutilizan las tablas
existentes de `ClinicaDB` mediante Fluent API y **no tienen migraciones**; sus scripts
`seed-*.sql` solo cargan datos de prueba. Billing es la excepción ya implementada: tiene
su propio `BillingDB` con migraciones EF Core.
En todos los casos las claves foráneas entre servicios se representan como IDs (`long`/`int`),
nunca como restricciones FK entre bases distintas.

El patrón microservicios se demuestra a nivel de código y arquitectura (proyectos separados,
hexagonal, MediatR, Controllers independientes, clientes HTTP tipados).

---

## 7. Paquetes NuGet por servicio

| Paquete | Uso |
|---|---|
| Microsoft.EntityFrameworkCore 9.0.0 | ORM |
| Microsoft.EntityFrameworkCore.SqlServer 9.0.0 | Provider SQL Server |
| Microsoft.AspNetCore.Authentication.JwtBearer | Validación JWT |
| System.IdentityModel.Tokens.Jwt | Generación de tokens |
| BCrypt.Net-Next | Hashing de passwords (Identity) |
| MediatR 12.5.0 | Patrón Mediator para CQRS y desacoplamiento |
| System.Linq.Dynamic.Core 1.6.0 | `filter` dinámico y `orderBy` en listados paginados |
| Serilog.AspNetCore 9.0.0 | Logging en las APIs |
| Microsoft.Extensions.Http 9.0.0 | Clientes HTTP tipados entre servicios |
| NugetPackageRest 2.2.0 | Capa REST genérica usada por Billing |

La validación se hace en los handlers (DomainException con código propio) y el mapeo
Entity ↔ DTO con mapper estático por servicio; no se usan FluentValidation ni AutoMapper.

---

## 8. Regla para saber dónde va cada tabla

> **¿Quién CREA esta tabla cuando el paciente entra al sistema?**

| Momento del flujo | Tablas que se crean | Servicio dueño |
|---|---|---|
| Paciente se registra | Pacientes, Contactos, Aseguradoras | Scheduling |
| Se agenda cita | Citas, Turnos | Scheduling |
| Médico atiende | Atenciones, Signos_Vitales, Diagnósticos, Notas | ClinicalRecords |
| Se pide estudio | Solicitudes_Estudio | Laboratory |
| Se factura | Facturas, Pagos | Billing |
| Se genera aviso | Notificaciones | Notification |


## 9. Orden de implementación

| Paso | Servicio | Dependencias | Estado |
|---|---|---|---|
| 1 | Shared.Kernel | Ninguna | Ya |
| 2 | Identity.Service | Shared.Kernel | Ya |
| 3 | Scheduling.Service | Identity | Ya |
| 4 | ClinicalRecords.Service | Identity, Scheduling | Ya |
| 5 | Laboratory.Service | ClinicalRecords, Identity | Ya |
| 6 | Billing.Service | Scheduling, ClinicalRecords | Ya |
| 7 | Notification.Service | Todos (eventos entrantes) | Ya |

---

## 10. Implementación de los microservicios 3, 4 y 6

### 10.1 ClinicalRecords.Service — microservicio 3

**Responsabilidad:** responder `¿QUÉ pasó médicamente en la atención?`. Dueño de las 25 tablas
clínicas (`Expedientes_Clínicos`, `Atenciones`, `Signos_Vitales`, `Notas_Médicas`,
`Diagnósticos`, `Síntomas`, `Prescripciones`, `Tratamientos`, `Indicaciones_Médicas`,
`Referencias_Médicas`, `Procedimientos`, `Archivos_Clínicos`, `Hospitalizaciones`, catálogos, etc.).

**Proyectos:** `Clinical.Domain`, `Clinical.Core`, `Clinical.Persistence`,
`Clinical.Infrastructure`, `Clinical.Api`, `Clinical.Tests`. Puerto HTTP `52097`.

**Reglas de negocio implementadas y probadas:**
- Un paciente tiene como máximo un expediente activo; `CreateAtencion` lo crea automáticamente
  con número `EXP-{año}-{id:D6}` si no existe.
- La atención nace de una cita: se valida existencia, pertenencia al paciente, médico activo
  y que la cita no tenga ya una atención.
- Signos vitales calculan IMC y lo redondean a 2 decimales.
- Un solo diagnóstico principal por atención y sin diagnósticos repetidos.
- Prescripción y tratamiento requieren detalle y medicamento activo.
- Procedimiento requiere catálogo activo y cantidad válida.
- Hospitalización exige cama activa en estado `LIBRE`, prohíbe dos ingresos activos del mismo
  expediente, valida fecha de egreso y libera la cama al egresar.
- Archivos clínicos exigen expediente existente, nombre y ruta no vacíos.
- Listados paginados con `pageNumber`, `pageSize`, `filter`, `orderBy`; filtros combinados.

**Features (MediatR):** crear/listar/obtener expedientes; crear/listar/obtener atenciones;
signos vitales; notas; diagnósticos; síntomas; prescripciones; tratamientos; indicaciones;
referencias; procedimientos; archivos; catálogos; ingresos/egresos hospitalarios.

**Endpoints principales:**
`GET/POST /api/expedientes`, `GET /api/expedientes/paciente/{pacienteId}`,
`GET/POST /api/expedientes/{id}/archivos`, `GET/POST /api/atenciones`,
`GET/POST /api/atenciones/{id}/signos-vitales`, `.../notas`, `.../diagnosticos`,
`.../sintomas`, `.../prescripciones`, `.../tratamientos`, `.../indicaciones`,
`.../referencias`, `.../procedimientos`, `GET /api/catalogos/*`,
`GET /api/hospitalizacion`, `POST /api/hospitalizacion/ingresar`,
`POST /api/hospitalizacion/{id}/egresar`.

**Errores:** `1001-1031` (dominio) y `1090-1092` (JWT: `401`/`403`), con respuesta
`{ errorCode, errorMessage }`.

**Dependencias:** `Scheduling.Api` para citas y `Identity.Api` para médicos; `BaseUrl` vacío
activa stubs para poder ejecutar y probar sin otros servicios. `seed-clinical.sql` carga
catálogos, habitación/cama y el expediente de prueba.

**Verificación:** `dotnet build src/ClinicalRecords.Service/Clinical.Api/Clinical.Api.csproj`
correcto; `dotnet test src/ClinicalRecords.Service/Clinical.Tests/Clinical.Tests.csproj`
22/22 pruebas.

### 10.2 Laboratory.Service — microservicio 4

**Responsabilidad:** responder `¿QUÉ estudios se pidieron y QUÉ mostraron?`. Dueño de 4 tablas:
`Catálogo_Estudios`, `Solicitudes_Estudio`, `Solicitud_Estudio_Detalle`,
`Resultados_Estudio`.

**Proyectos:** `Lab.Domain`, `Lab.Core`, `Lab.Persistence`, `Lab.Infrastructure`,
`Lab.Api`, `Lab.Tests`. Puerto HTTP `52098`.

**Reglas de negocio implementadas y probadas:**
- Alta de estudio con `codigo` único; el catálogo puede darse de baja (`activo = 0`).
- Solicitud con al menos un detalle, atención y médico válidos, y estudios existentes/activos.
- Número de solicitud generado como `SE-{año}-{id}`.
- No se permiten detalles duplicados ni cancelar una solicitud ya cancelada; el motivo es
  obligatorio y no se pueden cancelar solicitudes con resultados.
- Un solo resultado por detalle; al registrar el último resultado la solicitud pasa a
  `COMPLETADA`.
- Validación de propiedad: la atención debe ser del paciente indicado; el detalle debe
  pertenecer a la solicitud.

**Features (MediatR):** crear/listar/obtener estudios; crear/listar/obtener/cancelar
solicitudes; consultar resultados por solicitud o globalmente; registrar resultados.
Listados paginados con `atencionID`, `pacienteID`, `medicoID`, `estado`, `desde`, `hasta`.

**Endpoints principales:**
`GET/POST /api/estudios`, `GET /api/estudios/{id}`,
`GET/POST /api/solicitudes`, `GET /api/solicitudes/{id}`,
`POST /api/solicitudes/{id}/cancelar`, `GET /api/resultados`,
`GET /api/resultados/solicitud/{solicitudEstudioId}`, `POST /api/resultados`.

**Errores:** `1101-1118`, con respuesta `{ errorCode, errorMessage }`.

**Dependencias:** `ClinicalRecords.Api` para validar la atención y `Identity.Api` para el
médico; `BaseUrl` vacío activa stubs. `seed-laboratory.sql` carga 6 estudios, uno inactivo
para probar el error `1109`.

**Verificación:** `dotnet build src/Laboratory.Service/Lab.Api/Lab.Api.csproj` correcto;
`dotnet test src/Laboratory.Service/Lab.Tests/Lab.Tests.csproj` 26/26 pruebas.

### 10.3 Notification.Service — microservicio 6

**Responsabilidad:** responder `¿QUÉ notificaciones se generan?`. Dueño de la tabla
`Notificaciones`: centro de avisos/campana para pacientes y médicos. Los otros microservicios
registran aquí lo que deben comunicar; Notification no envía correo, SMS ni push por sí mismo.

**Proyectos:** `Notification.Domain`, `Notification.Core`, `Notification.Persistence`,
`Notification.Api`, `Notification.Tests`. No hay `Infrastructure` porque no tiene clientes
salientes. Puertos HTTP `52099` y HTTPS `52100`.

**Reglas de negocio implementadas y probadas:**
- `EventoID` obligatorio y normalizado a mayúsculas; es clave de idempotencia entre productores.
- Validación de `UsuarioID`, `Titulo`, `Mensaje`, `Tipo`, `Canal`, `Prioridad` y `OrigenServicio`.
- El destinatario es obligatorio salvo en `INTERNO`; en `EMAIL` debe contener `@`.
- `FechaProgramada`, si se informa, no puede ser anterior al momento actual.
- Alta idempotente: si el `EventoID` ya existe se devuelve la notificación existente, sin insertar.
- La nueva notificación nace `PENDIENTE`, `Activo = true`, `Intentos = 0`.
- Enviar desde `PENDIENTE`/`FALLIDA`: éxito → `ENVIADA` con `FechaEnvio`; fallo → `FALLIDA`
  con `UltimoError` obligatorio. En ambos casos incrementa `Intentos`.
- Marcar como leída desde cualquier estado salvo `CANCELADA`; si ya está `LEIDA` no cambia nada.
- Cancelar desde cualquier estado salvo `LEIDA`/`CANCELADA`, con motivo obligatorio;
  desactiva la fila y registra `FechaCancelacion`.
- Listados paginados con `usuarioID`, `tipo`, `canal`, `estado`, `origenServicio`,
  `soloActivas`, `desde`, `hasta`, `filter`, `orderBy` y `buscar`.

**Features (MediatR):** crear, enviar, marcar leída y cancelar notificaciones; consultar
listado paginado y detalle por ID.

**Endpoints principales:**
`GET /api/notificaciones`, `GET /api/notificaciones/usuario/{usuarioId}`,
`GET /api/notificaciones/{id}`, `POST /api/notificaciones`,
`POST /api/notificaciones/{id}/enviar`, `POST /api/notificaciones/{id}/leer`,
`POST /api/notificaciones/{id}/cancelar`.

**Errores:** `1201-1215` (dominio) y `1290-1292` (JWT: `401`/`403`), con respuesta
`{ errorCode, errorMessage }`.

**Dependencias:** entrantes desde Identity, Scheduling, ClinicalRecords, Laboratory y
Billing mediante `POST /api/notificaciones`. No hay dependencias salientes ni stubs.
`seed-notifications.sql` carga ejemplos en `PENDIENTE`, `ENVIADA` y `LEIDA`.

**Verificación:** `dotnet build src/Notification.Service/Notification.Api/Notification.Api.csproj`
correcto; `dotnet test src/Notification.Service/Notification.Tests/Notification.Tests.csproj`
18/18 pruebas.

### 10.4 Verificación conjunta

Los 17 proyectos nuevos están registrados en `citasMedicas.slnx`. La solución completa
compila con `dotnet build citasMedicas.slnx --nologo` y las pruebas se ejecutan con
`dotnet test citasMedicas.slnx --nologo` (163 pruebas). No se agregaron migraciones EF:
ClinicalRecords, Laboratory y Notification reutilizan `ClinicaDB` como Scheduling.

Los 6 microservicios de la propuesta quedan implementados; la arquitectura, la lógica de
negocio, los códigos de error y las pruebas de cada uno quedan descritos en esta sección.






