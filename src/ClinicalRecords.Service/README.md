# ClinicalRecords.Service

Microservicio 3 de Citas Médicas: historia clínica, atención médica, prescripciones, procedimientos, archivos clínicos y hospitalización.

## Propósito

Gestionar el registro clínico completo del paciente reutilizando la base de datos existente `ClinicaDB` (la misma que usa `Scheduling.Service`). No crea su propia base de datos, no incluye migraciones EF y no modifica el esquema: solo mapea las 25 tablas clínicas existentes.

Expone una API REST protegida con JWT, con filtros/orden/paginación genéricos (`filter`, `buscar`, `orderBy`, `pageNumber`, `pageSize`) en los listados y reglas de negocio centralizadas en handlers MediatR.

## Proyectos

| Proyecto | Responsabilidad |
|---|---|
| `Clinical.Domain` | 25 entidades POCO, sin `BaseEntity`, propiedades en español. |
| `Clinical.Core` | DTOs, excepciones/códigos de error, interfaces de repositorio, clientes abstraction, features CQRS y `ClinicalMapper`. |
| `Clinical.Persistence` | `ClinicalDbContext`, 25 configuraciones Fluent API y 25 repositorios sobre `ClinicaDB`. |
| `Clinical.Infrastructure` | Clientes HTTP a Scheduling/Identity, stubs y `ServiceTokenHandler`. |
| `Clinical.Api` | Host ASP.NET Core, controllers, JWT, CORS, Swagger/OpenAPI y middleware de errores. |
| `Clinical.Tests` | 22 pruebas xUnit con repositorios en memoria y stubs de servicios externos. |

Estructura física: `src/ClinicalRecords.Service/`.

## Tablas mapeadas

`ClinicalDbContext` aplica `ApplyConfigurationsFromAssembly` y mapea exactamente estas 25 tablas de `ClinicaDB`:

- `Antecedentes_Médicos`
- `Antecedentes_Familiares`
- `Alergias`
- `Hábitos`
- `Archivos_Clínicos`
- `Atenciones`
- `Atención_Diagnóstico`
- `Atención_Procedimiento`
- `Atención_Síntoma`
- `Camas`
- `Diagnósticos`
- `Expedientes_Clínicos`
- `Habitaciones`
- `Hospitalizaciones`
- `Indicaciones_Médicas`
- `Medicamentos`
- `Notas_Médicas`
- `Prescripciones`
- `Prescripción_Detalle`
- `Procedimientos`
- `Referencias_Médicas`
- `Signos_Vitales`
- `Síntomas`
- `Tratamientos`
- `Tratamiento_Detalle`

Decisiones de mapeo: claves primarias `ValueGeneratedOnAdd`, longitudes y precisión declaradas en Fluent API, decimales con `HasPrecision(18,2)`, relaciones con `DeleteBehavior.Restrict`, y un repositorio por tabla derivado de `Repository<T>` de `Shared.Kernel`.

## Configuración y ejecución

- Cadena de conexión: `ConnectionStrings:ClinicaDB` (por defecto `(localdb)\mssqllocaldb;Database=ClinicaDB`).
- API: `http://localhost:52097` (`launchBrowser: false`).
- Identity.Api: `http://localhost:52096` (solo se usa para tokens cuando los clientes reales están activos).
- JWT: misma `SigningKey`, `Issuer` y `Audience` que Billing.Service, para que el frontend reuse el token.

```powershell
dotnet build src/ClinicalRecords.Service/Clinical.Api/Clinical.Api.csproj
dotnet test src/ClinicalRecords.Service/Clinical.Tests/Clinical.Tests.csproj
```

Antes de probar la API con datos, ejecutar `seed-clinical.sql` sobre `ClinicaDB`.

## Endpoints

Todas las rutas requieren JWT (FallbackPolicy global). Prefijo común: `/api`.

### `/api/expedientes`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/expedientes` | Listado paginado/filtrable. |
| GET | `/api/expedientes/{id:int}` | Detalle con antecedentes, alergias y hábitos. |
| GET | `/api/expedientes/paciente/{pacienteId:long}` | Expediente activo del paciente. |
| POST | `/api/expedientes` | Crear expediente; genera `EXP-{año}-{id:D6}`. |
| GET | `/api/expedientes/{id:int}/archivos` | Archivos clínicos del expediente. |
| POST | `/api/expedientes/{id:int}/archivos` | Adjuntar archivo clínico. |

### `/api/atenciones`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/atenciones` | Listado paginado/filtrable. |
| GET | `/api/atenciones/{id:long}` | Detalle completo de la atención. |
| POST | `/api/atenciones` | Crear atención desde una cita. |
| POST/GET | `/{atencionId}/signos-vitales` | Registrar/consultar signos vitales. |
| POST/GET | `/{atencionId}/notas` | Registrar/consultar notas médicas. |
| POST/GET | `/{atencionId}/diagnosticos` | Asignar/consultar diagnósticos. |
| POST/GET | `/{atencionId}/sintomas` | Registrar/consultar síntomas. |
| POST/GET | `/{atencionId}/prescripciones` | Crear/consultar prescripciones. |
| POST/GET | `/{atencionId}/tratamientos` | Crear/consultar tratamientos. |
| POST/GET | `/{atencionId}/indicaciones` | Crear/consultar indicaciones médicas. |
| POST/GET | `/{atencionId}/referencias` | Crear/consultar referencias médicas. |
| POST/GET | `/{atencionId}/procedimientos` | Registrar/consultar procedimientos. |

Rutas completas: `/api/atenciones/{atencionId:long}/...`.

### `/api/catalogos`

| Método | Ruta |
|---|---|
| GET | `/api/catalogos/diagnosticos` |
| GET | `/api/catalogos/sintomas` |
| GET | `/api/catalogos/medicamentos` |
| GET | `/api/catalogos/procedimientos` |
| GET | `/api/catalogos/habitaciones` |
| GET | `/api/catalogos/camas` |

### `/api/hospitalizacion`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/hospitalizacion` | Listado paginado/filtrable (`soloActivas`, `estado`). |
| POST | `/api/hospitalizacion/ingresar` | Ingresar paciente; ocupa la cama. |
| POST | `/api/hospitalizacion/{id:long}/egresar` | Egresar paciente; libera la cama. Acepta cuerpo opcional con `fechaEgreso`/`observaciones`. |

## Códigos de error

`Clinical.Core.Exceptions.Errores` usa el rango `1001-1099` para no colisionar con Billing (`8xx`) ni Scheduling (`9xx`). Respuesta HTTP: `400` con `{ errorCode, errorMessage }`.

| Código | Nombre |
|---|---|
| 1001 | `DATOS_INVALIDOS` |
| 1002 | `EXPEDIENTE_NO_ENCONTRADO` |
| 1003 | `EXPEDIENTE_DUPLICADO` |
| 1004 | `CITA_NO_ENCONTRADA` |
| 1005 | `CITA_PACIENTE_INCONSISTENTE` |
| 1006 | `ATENCION_CITA_DUPLICADA` |
| 1007 | `ATENCION_NO_ENCONTRADA` |
| 1008 | `MEDICO_NO_VALIDO` |
| 1009 | `SIGNOS_VITALES_INVALIDOS` |
| 1010 | `NOTA_NO_ENCONTRADA` |
| 1011 | `DIAGNOSTICO_NO_ENCONTRADO` |
| 1012 | `DIAGNOSTICO_PRINCIPAL_DUPLICADO` |
| 1013 | `DIAGNOSTICO_DUPLICADO` |
| 1014 | `SINTOMA_NO_ENCONTRADO` |
| 1015 | `MEDICAMENTO_NO_VALIDO` |
| 1016 | `PRESCRIPCION_SIN_DETALLE` |
| 1017 | `PRESCRIPCION_NO_ENCONTRADA` |
| 1018 | `TRATAMIENTO_SIN_DETALLE` |
| 1019 | `TRATAMIENTO_NO_ENCONTRADO` |
| 1020 | `INDICACION_NO_ENCONTRADA` |
| 1021 | `REFERENCIA_NO_ENCONTRADA` |
| 1022 | `PROCEDIMIENTO_NO_VALIDO` |
| 1023 | `PROCEDIMIENTO_CANTIDAD_INVALIDA` |
| 1024 | `ARCHIVO_NO_ENCONTRADO` |
| 1025 | `HABITACION_NO_ENCONTRADA` |
| 1026 | `CAMA_NO_ENCONTRADA` |
| 1027 | `CAMA_NO_DISPONIBLE` |
| 1028 | `HOSPITALIZACION_ACTIVA_PREVIA` |
| 1029 | `HOSPITALIZACION_NO_ENCONTRADA` |
| 1030 | `HOSPITALIZACION_NO_ACTIVA` |
| 1031 | `FECHA_EGRESO_INVALIDA` |
| 1090 | `TOKEN_EXPIRADO` (`401`) |
| 1091 | `TOKEN_INVALIDO` (`401`) |
| 1092 | `SIN_PERMISOS` (`403`) |

Decisión: los errores JWT se adaptaron al rango Clinical (`1090-1092`) en lugar de reutilizar los códigos de Billing.

## Reglas de negocio

- Un paciente solo puede tener un expediente activo (`1003`); si no existe, `CreateAtencion` lo crea automáticamente con número `EXP-{año}-{id:D6}`.
- Una atención se origina en una cita: se valida que exista (`1004`), que pertenezca al paciente indicado (`1005`), que el médico exista/esté activo (`1008`) y que la cita no tenga ya atención (`1006`).
- Signos vitales: si hay peso y talla, se calcula `IMC = peso / (talla/100)^2`, redondeado a 2 decimales.
- Solo se permite un diagnóstico principal por atención (`1012`) y no se repite el mismo diagnóstico (`1013`).
- Prescripción y tratamiento requieren al menos un detalle (`1016`/`1018`) y medicamentos activos (`1015`).
- Hospitalización: la cama debe existir, estar activa y en estado `LIBRE` (`1026`/`1027`); no puede haber dos ingresos activos para el mismo expediente (`1028`); el egreso no puede ser anterior al ingreso (`1031`) y devuelve la cama a `LIBRE`.
- Archivos clínicos: exigen expediente existente y nombre/ruta no vacíos (`1002`/`1001`).

## Dependencias con stub por defecto

Clinical consume otros microservicios, pero **no depende de que estén levantados para arrancar ni para probar**:

| Configuración | Comportamiento |
|---|---|
| `Services:Scheduling:BaseUrl` vacío | `StubSchedulingClient`: devuelve la cita solicitada, `PacienteID = 1`, `EstadoCodigo = "PROGRAMADA"`. |
| `Services:Identity:BaseUrl` vacío | `StubIdentityClient`: devuelve un médico activo con el ID solicitado. |

Al configurar una `BaseUrl` no vacía se usa el cliente HTTP real (`SchedulingClient`/`IdentityClient`) y `ServiceTokenHandler` obtiene un JWT desde `Identity:BaseUrl` con la cuenta de servicio `Services:<Nombre>:ServiceAccount`. `ServiceTokenHandler` cachea el token, lo renueva 60 segundos antes del vencimiento y reintenta una vez ante `401`.

Decisión: los stubs por defecto permiten desarrollar y ejecutar la batería de tests sin red; las cuentas de servicio están en `appsettings.json` como configuración de ejemplo y no son credenciales reales.

## Seed de datos

`seed-clinical.sql`:

- Es idempotente (`IF NOT EXISTS` + `IDENTITY_INSERT`, mismo estilo que `seed-scheduling.sql`).
- No contiene DDL: asume que `ClinicaDB` ya tiene las 25 tablas clínicas creadas.
- Carga catálogos mínimos (diagnósticos, síntomas, medicamentos, procedimientos), habitación 1, cama 1 en `LIBRE` y expediente 1 del paciente 1.

## Pruebas

22 tests xUnit en `Clinical.Tests`, sin base de datos:

- `InMemoryRepository<T>` con `Func<T,int>` (IDs `long` convertidos con `(int)x.AtencionID`).
- Un fake por cada uno de los 25 repositorios.
- Stubs de `ISchedulingClient` e `IIdentityClient`.
- Cobertura: creación de expediente/atención, errores de cita y médico, IMC, diagnósticos, prescripciones, archivos, hospitalización/egreso y filtros combinados.
