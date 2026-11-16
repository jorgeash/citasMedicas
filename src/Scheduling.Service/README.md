# Scheduling.Service

Microservicio de agenda: **pacientes**, **citas** y **aseguradoras**.
Es el servicio que Billing consulta para validar `pacienteID` y `citaID`.

## Base de datos

Scheduling **reutiliza las tablas existentes de `ClinicaDB`**:

| Entidad | Tabla | PK |
|---|---|---|
| `Paciente` | `Pacientes` | `bigint` |
| `PacienteAseguradora` | `Paciente_Aseguradora` | `bigint` |
| `Cita` | `Citas` | `bigint` |
| `Aseguradora` | `Aseguradoras` | `int` |
| `Medico` | `Medicos` | `int` (catálogo) |
| `EstadoCita` | `Estados_Cita` | `int` (catálogo) |

Por eso:

- **No hay carpeta `Migrations`** y **nunca** se ejecuta
  `dotnet ef database update` contra `ClinicaDB`.
- Las claves de paciente y cita son `long` (la BD usa `bigint`).
- El vínculo paciente-aseguradora vive en `Paciente_Aseguradora`; el
  `aseguradoraID` del DTO se calcula desde el seguro **activo y vigente**.

## Proyectos

```
src/Scheduling.Service/
├── Scheduling.Domain/        entidades
├── Scheduling.Core/          DTOs, features (MediatR), mapper, interfaces
├── Scheduling.Persistence/   DbContext, configuraciones y repositorios (EF)
├── Scheduling.Api/           controllers + Program.cs
└── Scheduling.Tests/         repositorios en memoria + tests de handlers
```

## Ejecutar

```bash
dotnet run --project src/Scheduling.Service/Scheduling.Api
```

Puertos (ver `Properties/launchSettings.json`): HTTP `52095`, HTTPS `52094`.

La cadena de conexión `ClinicaDB` está en `appsettings.json`.

## Semillas

```powershell
$sql = Get-Content src\Scheduling.Service\seed-scheduling.sql -Raw
Invoke-Sqlcmd -ServerInstance "(localdb)\mssqllocaldb" -Database ClinicaDB -Query $sql
```

El script es idempotente (guardas `IF NOT EXISTS` + `IDENTITY_INSERT`) y carga,
en este orden: `Estados_Cita`, `Especialidades`, `Medicos`, `Medico_Especialidad`,
`Aseguradoras`, `Pacientes`, `Paciente_Aseguradora` y `Citas`.

Datos de prueba resultantes:

| Registro | Detalle |
|---|---|
| Paciente `1` | Juan Pérez → Aseguradora `1` (Seguros Salud) |
| Paciente `2` | María López → sin aseguradora |
| Paciente `3` | Carlos Ruiz → Aseguradora `2` (Particular) |
| Cita `1` | paciente `1`, PROGRAMADA |
| Cita `2` | paciente `2`, ATENDIDA |
| Cita `3` | paciente `3`, CANCELADA |

Sirven para probar la integración: la cita `1` es válida para el paciente `1`,
la cita `2` dispara el error `810` y la `999` el error `809`.

## Endpoints

| Método | Ruta | Notas |
|---|---|---|
| GET | `/api/pacientes` | Paginado (`pageNumber`, `pageSize`, `filter`, `orderBy`) |
| GET | `/api/pacientes/{id}` | **Lo usa Billing**: incluye `nombre` y `aseguradoraID` |
| POST | `/api/pacientes` | Alta; acepta `aseguradoraID` + `numeroPoliza` opcionales |
| GET | `/api/citas` | Paginado + filtros `pacienteID`, `estadoCitaID`, `desde`, `hasta` |
| GET | `/api/citas/{id}` | **Lo usa Billing**: incluye `fecha` |
| POST | `/api/citas` | `medicoID` obligatorio; `estadoCitaID` opcional (por defecto PROGRAMADA) |
| POST | `/api/citas/{id}/cancelar` | Cambia el estado a `CANCELADA` |
| GET | `/api/aseguradoras` | Paginado |
| GET | `/api/aseguradoras/{id}` | Detalle |
| POST | `/api/aseguradoras` | Alta |
| GET | `/api/medicos` | Paginado; catálogo para elegir médico al agendar |
| GET | `/api/estados-cita` | Catálogo completo (sin paginar) |

Los listados paginados devuelven el `PagedDto` **directo** (`totalRecords`,
`totalPage`, `currentPage`, `pageSize`, `data`), igual que Billing.

Contrato mínimo que consume Billing:

```json
GET /api/pacientes/1  ->  { "pacienteID": 1, "nombre": "Juan Pérez", "aseguradoraID": 1 }
GET /api/citas/1      ->  { "citaID": 1, "pacienteID": 1, "fecha": "2026-11-20T09:00:00" }
```

## Errores

`DomainException` se devuelve como `400` con `{ errorCode, errorMessage }`:

| Código | Significado |
|---|---|
| 901 | Datos inválidos |
| 902 | Paciente no encontrado |
| 903 | Paciente duplicado (código o documento) |
| 904 | Aseguradora no encontrada |
| 905 | Médico no encontrado |
| 906 | Estado de cita no encontrado |
| 907 | Cita no encontrada |
| 908 | Cita ya cancelada |

## Pruebas

```bash
dotnet test src/Scheduling.Service/Scheduling.Tests/Scheduling.Tests.csproj
```

17 tests que cubren alta de paciente/cita, cancelación, queries por ID y
filtros, usando repositorios en memoria (sin EF ni base de datos).

## Relación con Billing

`Billing.Api` consume este servicio con `ISchedulingClient`:

- `Services:Scheduling:BaseUrl` = `http://localhost:52095` → cliente HTTP real.
- Sin valor → `StubSchedulingClient` (acepta cualquier paciente/cita).

Al crear una factura con `citaID`, Billing valida que la cita exista y que
pertenezca al mismo paciente (códigos `809` y `810`).
