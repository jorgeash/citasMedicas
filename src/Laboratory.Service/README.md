# Laboratory.Service

Microservicio de laboratorio: **estudios solicitados** y **resultados**.
Responde a la pregunta «¿qué estudios se pidieron y qué mostraron?», y valida
la atención contra ClinicalRecords y el médico contra Identity antes de crear
una solicitud.

## Base de datos

Laboratory **reutiliza las tablas existentes de `ClinicaDB`**, igual que
Scheduling.Service. No hay carpeta `Migrations` y nunca se ejecuta
`dotnet ef database update` contra `ClinicaDB`.

| Entidad | Tabla | PK |
|---|---|---|
| `Estudio` | `Catálogo_Estudios` | `int` |
| `SolicitudEstudio` | `Solicitudes_Estudio` | `int` |
| `SolicitudEstudioDetalle` | `Solicitud_Estudio_Detalle` | `int` |
| `ResultadoEstudio` | `Resultados_Estudio` | `int` |

La cadena `ClinicaDB` está en `appsettings.json`. Todas las relaciones usan
`DeleteBehavior.Restrict` y los `decimal` son `HasPrecision(18, 2)`.

## Proyectos

```
src/Laboratory.Service/
├── Lab.Domain/       4 entidades POCO (sin BaseEntity, sin dependencias)
├── Lab.Core/         DTOs, features MediatR, contratos, errores y DI
├── Lab.Persistence/  LabDbContext, 4 configuraciones y repositorios EF
├── Lab.Infrastructure/ clientes HTTP (ClinicalRecords/Identity), stubs y JWT de servicio
├── Lab.Api/          3 controllers + Program.cs
└── Lab.Tests/        repositorios en memoria + stubs + tests xUnit
```

## Ejecutar

```bash
dotnet run --project src/Laboratory.Service/Lab.Api
```

Puerto HTTP `52098` (`Properties/launchSettings.json`, `launchBrowser: false`).
Todos los endpoints exigen JWT (política de fallback) salvo `[AllowAnonymous]`.
La clave, emisor y audiencia son los mismos que en Billing/Scheduling.

## Semillas

```powershell
$sql = Get-Content src\Laboratory.Service\seed-laboratory.sql -Raw
Invoke-Sqlcmd -ServerInstance "(localdb)\mssqllocaldb" -Database ClinicaDB -Query $sql
```

El script es idempotente (`IF NOT EXISTS` + `IDENTITY_INSERT`) y **requiere que
las tablas ya existan**: no crea ni altera esquema. Carga 6 estudios; el `6`
está inactivo para probar el error `1109`.

## Endpoints

| Método | Ruta | Notas |
|---|---|---|
| GET | `/api/estudios` | Paginado: `pageNumber`, `pageSize`, `filter`, `orderBy`, `buscar`, `activo`, `tipo` |
| GET | `/api/estudios/{id}` | Detalle de catálogo |
| POST | `/api/estudios` | Alta con `codigo` único |
| GET | `/api/solicitudes` | Paginado: `atencionID`, `pacienteID`, `medicoID`, `estado`, `desde`, `hasta`; incluye detalle y resultados |
| GET | `/api/solicitudes/{id}` | Solicitud completa |
| POST | `/api/solicitudes` | Alta; valida atención, médico, detalles y estudios |
| POST | `/api/solicitudes/{id}/cancelar` | Cuerpo opcional `{ "motivo": "..." }` |
| GET | `/api/resultados` | Consulta global paginada: `pacienteID`, `estado`, `desde`, `hasta` |
| GET | `/api/resultados/solicitud/{solicitudEstudioId}` | Listado simple de esa solicitud |
| POST | `/api/resultados` | Registrar resultado |

Los listados paginados devuelven el `PagedDto` directo (`totalRecords`,
`totalPage`, `currentPage`, `pageSize`, `data`). Los filtros usan
`Filter.FromStringExpression`, `Filter.FreeText`, `Filter.AndAlso` y
`Filter.ParseOrderBy` de `Shared.Kernel`.

`DomainException` se devuelve como `400` con `{ errorCode, errorMessage }`
(tanto en el controller como en el middleware global).

## Códigos de error

| Código | Significado |
|---|---|
| 1101 | Datos inválidos (tipo de estudio, prioridad o valor del resultado) |
| 1102 | Código de estudio duplicado |
| 1103 | Estudio no encontrado |
| 1104 | Solicitud sin detalle |
| 1105 | Atención no encontrada |
| 1106 | Atención de otro paciente |
| 1107 | Médico no encontrado |
| 1108 | Médico inactivo |
| 1109 | Estudio inexistente o inactivo |
| 1110 | Detalle duplicado en la solicitud |
| 1111 | Solicitud no encontrada |
| 1112 | Solicitud cancelada |
| 1113 | Resultado duplicado para el detalle |
| 1114 | Estado de resultado inválido |
| 1115 | Detalle ajeno a la solicitud |
| 1116 | Solicitud en estado no cancelable |
| 1117 | Motivo de cancelación obligatorio |
| 1118 | Solicitud con resultados registrados |

## Reglas de negocio

- `NumeroSolicitud` es temporal hasta el insert y después se genera como
  `SE-<año>-<ID con 6 dígitos>` (mismo patrón `F-` de Billing).
- Estado inicial de la solicitud: `PENDIENTE`; con el primer resultado pasa a
  `EN_PROCESO` y, cuando **todos** sus detalles tienen resultado, a
  `COMPLETADA` con `UpdatedAt`.
- Solo se permite **un resultado por detalle**; la solicitud cancelada no
  acepta resultados. Estados de resultado: `NORMAL`, `ANORMAL`, `CRITICO`.
- `ValidadoPorMedicoID` es opcional; si se informa se registra
  `FechaValidacion`.
- Cancelar solo desde `PENDIENTE` o `EN_PROCESO`, con motivo (se guarda en
  `Observaciones` como `CANCELADA: ...`) y sin resultados registrados.
- Tipos de estudio: `LABORATORIO`, `IMAGEN`, `PATOLOGIA`, `OTRO`.
  Prioridades: `NORMAL`, `URGENTE`.

## Pruebas

```bash
dotnet test src/Laboratory.Service/Lab.Tests/Lab.Tests.csproj
```

26 tests xUnit sin base de datos: `InMemoryRepository<T>` copiado de
Billing.Tests, repositorios fake y stubs de `IClinicalRecordsClient` /
`IIdentityClient`. Cubren alta de estudio y código duplicado, solicitudes sin
detalle, atención inexistente/de otro paciente, médico inactivo, detalle
duplicado, estudio inactivo, número generado, resultados y completación,
resultado duplicado, cancelación y filtros/búsqueda paginada.

## Dependencias externas

| Cliente | Contrato | Configuración |
|---|---|---|
| ClinicalRecords | `IClinicalRecordsClient.GetAtencionAsync` | `Services:ClinicalRecords:BaseUrl` |
| Identity | `IIdentityClient.GetMedicoAsync` | `Services:Identity:BaseUrl` |

Con `BaseUrl` vacío (valor por defecto en `appsettings.json`) se activa el
**stub** de cada cliente: devuelve una atención activa cuyo `PacienteID` es
igual al `AtencionID` y un médico activo, para no bloquear el desarrollo.
Con `BaseUrl` real se registra un typed client de `IHttpClientFactory` y
`ServiceTokenHandler` obtiene un JWT de `Services:Identity:BaseUrl` usando la
cuenta de servicio `Services:<Nombre>:ServiceAccount` de cada destino; si
falta la cuenta o la URL de Identity, el arranque falla con un mensaje
explicativo.
