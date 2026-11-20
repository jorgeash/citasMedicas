# Notification.Service

Microservicio de notificaciones: **centro único de avisos al usuario**. No envía
correo, SMS ni push por sí mismo; registra y gestiona el ciclo de vida de cada
notificación para que Identity, Scheduling, ClinicalRecords, Laboratory y
Billing dejen constancia de lo que deben comunicar al paciente o al médico.

| Tipo | Generador habitual |
|---|---|
| `CITA` | Scheduling.Service |
| `RECORDATORIO` | Scheduling.Service |
| `RESULTADO` | Laboratory.Service |
| `FACTURA` | Billing.Service |
| `PAGO` | Billing.Service |
| `HOSPITALIZACION` | ClinicalRecords.Service |
| `SISTEMA` | Notification.Service / tareas de sistema |

## Base de datos

Notification **reutiliza la tabla existente `[Notificaciones]` de `ClinicaDB`**,
al igual que Laboratory y ClinicalRecords. No hay carpeta `Migrations`, no se
ejecuta `dotnet ef database update` y el script de semilla no crea ni altera
esquema. El mapeo se hace con Fluent API en `NotificacionConfiguration`.

| Entidad | Tabla | PK |
|---|---|---|
| `Notificacion` | `[Notificaciones]` | `int NotificacionID` |

`EventoID` tiene índice único: es la clave de idempotencia entre servicios.
`UsuarioID` tiene índice para las consultas por destinatario.

## Proyectos

```
src/Notification.Service/
├── Notification.Domain/      1 entidad POCO (sin BaseEntity, sin paquetes)
├── Notification.Core/        DTOs, MediatR, errores, repositorio contrato y DI
├── Notification.Persistence/ NotificationDbContext, configuración y repositorio EF
├── Notification.Api/         1 controller + Program.cs
└── Notification.Tests/       repositorio en memoria + 18 pruebas xUnit
```

No existe proyecto `Infrastructure`: Notification no tiene clientes salientes
ni cuentas de servicio, por lo que tampoco hay `ServiceTokenHandler` ni una
sección `Services` en la configuración.

## Ejecutar

```bash
dotnet run --project src/Notification.Service/Notification.Api
```

Perfiles HTTP `52099` y HTTPS `52100` en `Properties/launchSettings.json`, con
`launchBrowser: false`. Todos los endpoints exigen JWT mediante la política de
fallback. Emisor, audiencia y clave son los mismos que en Laboratory.Api y
Clinical.Api.

## Campo de la tabla `[Notificaciones]`

| Campo | Tipo | Uso |
|---|---|---|
| `NotificacionID` | `int` | PK, `IDENTITY` |
| `EventoID` | `nvarchar(100)` | Id externo del evento productor, único |
| `UsuarioID` | `bigint` | Usuario destinatario |
| `Destinatario` | `nvarchar(256)` | Dirección, correo o identificador de canal |
| `Titulo` | `nvarchar(200)` | Encabezado visible |
| `Mensaje` | `nvarchar(1000)` | Contenido |
| `Tipo` | `nvarchar(30)` | CITA, RECORDATORIO, RESULTADO, FACTURA, PAGO, HOSPITALIZACION, SISTEMA |
| `Canal` | `nvarchar(20)` | INTERNO, EMAIL, SMS, PUSH |
| `Prioridad` | `nvarchar(20)` | BAJA, NORMAL, ALTA, CRITICA |
| `Estado` | `nvarchar(20)` | PENDIENTE, ENVIADA, LEIDA, FALLIDA, CANCELADA |
| `OrigenServicio` | `nvarchar(30)` | IDENTITY, SCHEDULING, CLINICALRECORDS, LABORATORY, BILLING, NOTIFICATION, SISTEMA |
| `Referencia` | `nvarchar(100)` | Identificador opcional del objeto de negocio |
| `FechaCreacion` | `datetime2` | Momento de alta |
| `FechaProgramada` | `datetime2` | Envío diferido opcional |
| `FechaEnvio` | `datetime2` | Momento del envío correcto |
| `FechaLectura` | `datetime2` | Momento en que el usuario la abre |
| `FechaCancelacion` | `datetime2` | Momento de cancelación |
| `MotivoCancelacion` | `nvarchar(300)` | Motivo informado al cancelar |
| `Intentos` | `int` | Número de intentos de envío |
| `UltimoError` | `nvarchar(500)` | Último error de envío |
| `Activo` | `bit` | `false` al cancelar |
| `CreatedAt` | `datetime2` | Auditoría de creación |
| `UpdatedAt` | `datetime2` | Auditoría de modificación |

## Endpoints

| Método | Ruta | Notas |
|---|---|---|
| GET | `/api/notificaciones` | Paginado: `pageNumber`, `pageSize`, `filter`, `orderBy`, `buscar`, `usuarioID`, `tipo`, `canal`, `estado`, `origenServicio`, `soloActivas`, `desde`, `hasta` |
| GET | `/api/notificaciones/usuario/{usuarioId}` | Listado paginado de ese usuario |
| GET | `/api/notificaciones/{id}` | Detalle; `404` con `errorCode = 1202` si no existe |
| POST | `/api/notificaciones` | Alta idempotente por `EventoID`; `201 Created` |
| POST | `/api/notificaciones/{id}/enviar` | Cuerpo opcional `{ "exito": true, "error": null }`; sin cuerpo equivale a `exito: true` |
| POST | `/api/notificaciones/{id}/leer` | Marca como leída; idempotente |
| POST | `/api/notificaciones/{id}/cancelar` | Cuerpo `{ "motivo": "..." }` |

Los listados devuelven el `PagedDto` directo (`totalRecords`, `totalPage`,
`currentPage`, `pageSize`, `data`). Los filtros usan `Filter.FromStringExpression`,
`Filter.FreeText`, `Filter.AndAlso` y `Filter.ParseOrderBy` de `Shared.Kernel`.
La búsqueda libre cubre `EventoID`, `Titulo`, `Mensaje` y `Referencia`.

## Reglas de negocio

- `EventoID` se normaliza a mayúsculas y es obligatorio (`1214`). `UsuarioID`
  debe ser mayor que cero (`1201`).
- `Tipo`, `Canal`, `Prioridad` y `OrigenServicio` se normalizan a mayúsculas y
  se validan contra sus listas (`1203`, `1204`, `1212`, `1213`).
- `Titulo` y `Mensaje` son obligatorios (`1201`) y se recortan a su longitud.
- El destinatario es obligatorio salvo en `INTERNO`; en `EMAIL` debe contener
  `@` (`1205`).
- `FechaProgramada`, si se informa, no puede ser anterior al momento actual
  (`1211`).
- Si ya existe una notificación con el mismo `EventoID`, se devuelve sin
  insertar otra. La nueva nace `PENDIENTE`, `Activo = true`, `Intentos = 0`,
  con `FechaCreacion = CreatedAt = UtcNow`.
- Enviar solo desde `PENDIENTE` o `FALLIDA` (`1207`). Con éxito pasa a
  `ENVIADA`, fija `FechaEnvio` y limpia `UltimoError`. Sin éxito pasa a
  `FALLIDA`, exige `Error` (`1215`) y lo guarda recortado. En ambos casos
  incrementa `Intentos` y actualiza `UpdatedAt`.
- Leer solo desde un estado distinto de `CANCELADA` (`1208`); si ya está
  `LEIDA` devuelve el DTO sin cambios.
- Cancelar solo desde un estado distinto de `LEIDA` o `CANCELADA` (`1209`),
  con motivo obligatorio (`1210`). Pasa a `CANCELADA`, desactiva la fila y
  registra `FechaCancelacion` y `MotivoCancelacion`.
- `DomainException` se devuelve como `400` con `{ errorCode, errorMessage }`,
  tanto en el controller como en el middleware global.

## Códigos de error

| Código | Significado |
|---|---|
| 1201 | Datos inválidos (UsuarioID, título o mensaje) |
| 1202 | Notificación no encontrada |
| 1203 | Tipo inválido |
| 1204 | Canal inválido |
| 1205 | Destinatario inválido |
| 1206 | Estado inválido |
| 1207 | Estado no enviable |
| 1208 | Estado no leíble |
| 1209 | Estado no cancelable |
| 1210 | Motivo de cancelación obligatorio |
| 1211 | Fecha programada inválida |
| 1212 | Prioridad inválida |
| 1213 | Origen inválido |
| 1214 | Evento inválido |
| 1215 | Error de envío obligatorio |
| 1290 | Token expirado |
| 1291 | Token inválido o ausente |
| 1292 | Sin permisos |

## Semillas

```powershell
$sql = Get-Content src\Notification.Service\seed-notifications.sql -Raw
Invoke-Sqlcmd -ServerInstance "(localdb)\mssqllocaldb" -Database ClinicaDB -Query $sql
```

El script es idempotente (`IF NOT EXISTS` + `IDENTITY_INSERT`) y **requiere que
`[dbo].[Notificaciones]` ya exista**: no crea ni altera esquema. Carga tres
ejemplos para los estados `PENDIENTE`, `ENVIADA` y `LEIDA`, con canales
`INTERNO` y `EMAIL`, y termina verificando el `COUNT` insertado.

## Pruebas

```bash
dotnet test src/Notification.Service/Notification.Tests/Notification.Tests.csproj
```

18 pruebas xUnit sin base de datos: `InMemoryRepository<T>` con
getId/setId de `NotificacionID` y `FakeNotificacionRepository`. Cubren alta
válida e idempotente, todas las validaciones de creación, envío correcto y
fallido, error obligatorio, estados no enviables, lectura y su idempotencia,
lectura de cancelada, cancelación y su motivo, y filtros/paginación/búsqueda.
