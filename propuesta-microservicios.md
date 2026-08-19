# Propuesta: Microservicios — Sistema de Gestión Clínica

**Backend:** C# / .NET 9 — Arquitectura Hexagonal — Microservicios
**Frontend:** Vue 3

---

## 1. Distribución de 60 tablas en 5 microservicios

| # | Microservicio | Tablas | Pregunta que responde |
|---|---|---|---|
| 1 | **Identity.Service** | 13 | ¿QUIÉN es esta persona y QUÉ puede hacer? |
| 2 | **Scheduling.Service** | 9 | ¿CUÁNDO y DÓNDE ocurre la atención? |
| 3 | **ClinicalRecords.Service** | 25 | ¿QUÉ pasó médicamente en la atención? |
| 4 | **Laboratory.Service** | 4 | ¿QUÉ estudios se pidieron y QUÉ mostraron? |
| 5 | **Billing.Service** | 8 | ¿CUÁNTO se cobra y se PAGÓ? |
| 6 | **Notification.Service** | 1 | ¿Qué notificaciones se generan? |

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
│   ├── Shared.Kernel/
│   │   ├── Shared.Kernel.csproj
│   │   ├── Entities/
│   │   │   └── BaseEntity.cs
│   │   ├── Interfaces/
│   │   │   └── IUnitOfWork.cs
│   │   └── Extensions/
│   │       └── AuthExtensions.cs
│   │
│   ├── Identity.Service/
│   │   ├── Identity.Domain/
│   │   ├── Identity.Application/
│   │   ├── Identity.Persistence/
│   │   └── Identity.Api/
│   │
│   ├── Scheduling.Service/
│   │   ├── Scheduling.Domain/
│   │   ├── Scheduling.Application/
│   │   ├── Scheduling.Persistence/
│   │   └── Scheduling.Api/
│   │
│   ├── ClinicalRecords.Service/
│   │   ├── Clinical.Domain/
│   │   ├── Clinical.Application/
│   │   ├── Clinical.Persistence/
│   │   └── Clinical.Api/
│   │
│   ├── Laboratory.Service/
│   │   ├── Lab.Domain/
│   │   ├── Lab.Application/
│   │   ├── Lab.Persistence/
│   │   └── Lab.Api/
│   │
│   └── Billing.Service/
│       ├── Billing.Domain/
│       ├── Billing.Application/
│       ├── Billing.Persistence/
│       └── Billing.Api/
│
├── citasMedicas.slnx
└── docker-compose.yml
```

### Estructura hexagonal interna de cada servicio

```
Servicio.Domain/
├── Entities/          Entidades del dominio
├── Enums/             Enumeraciones
├── ValueObjects/      Objetos de valor
└── Interfaces/        Puertos (repositorios, servicios externos)

Servicio.Application/
├── Features/          Organización por dominio (MediatR)
│   ├── Entidad1/
│   │   ├── Commands/
│   │   │   ├── CreateEntidad1/
│   │   │   │   ├── CreateEntidad1Command.cs
│   │   │   │   ├── CreateEntidad1CommandHandler.cs
│   │   │   │   └── CreateEntidad1CommandValidator.cs
│   │   │   ├── UpdateEntidad1/
│   │   │   └── DeleteEntidad1/
│   │   └── Queries/
│   │       ├── GetEntidad1s/
│   │       │   ├── GetEntidad1sQuery.cs
│   │       │   └── GetEntidad1sQueryHandler.cs
│   │       └── GetEntidad1ById/
│   ├── Entidad2/
│   └── ...
├── DTOs/
│   ├── Requests/
│   └── Responses/
└── Interfaces/        Contratos de aplicación

Servicio.Persistence/
├── Context/           DbContext
├── Configurations/    Fluent API
├── Repositories/      Implementación de puertos
├── Migrations/        Migraciones EF Core
└── Extensions/        DI registration

Servicio.Api/
├── Controllers/       Endpoints HTTP (usan MediatR)
├── Middleware/         Auditoría, errores
├── Program.cs
└── appsettings.json
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
| Notification | Todos | Recibe eventos de todos los servicios |

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
| Patrón | Shared database (una sola BD para todos los servicios) | // A Consultar
| Nombre | `ClinicaDB` |

Cada microservicio usa sus tablas por convención (schema), no por restricción técnica. El patrón microservicios se demuestra a nivel de código y arquitectura (proyectos separados, hexagonal, MediatR, Controllers independientes).

---

## 7. Paquetes NuGet por servicio

| Paquete | Uso |
|---|---|
| Microsoft.EntityFrameworkCore 9.0.0 | ORM |
| Microsoft.EntityFrameworkCore.SqlServer 9.0.0 | Provider SQL Server |
| Microsoft.AspNetCore.Authentication.JwtBearer | Validación JWT |
| System.IdentityModel.Tokens.Jwt | Generación de tokens |
| BCrypt.Net-Next | Hashing de passwords |
| FluentValidation | Validación de DTOs |
| AutoMapper | Mapeo Entity ↔ DTO |
| MediatR | Patrón Mediator para CQRS y desacoplamiento |
| Microsoft.Extensions.Http.Polly | Retry + Circuit Breaker para llamadas entre servicios |

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

| Paso | Servicio | Dependencias |
|---|---|---|
| 1 | Shared.Kernel | Ninguna |
| 2 | Identity.Service | Shared.Kernel |
| 3 | Scheduling.Service | Identity |
| 4 | ClinicalRecords.Service | Identity, Scheduling |
| 5 | Laboratory.Service | ClinicalRecords, Identity |
| 6 | Billing.Service | Scheduling, ClinicalRecords |






