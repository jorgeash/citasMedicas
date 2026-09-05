# Guía de Arquitectura y CQRS - Sistema de Gestión Clínica

---

## 1. Arquitectura General

El proyecto sigue **Clean Architecture** con separación en capas:

```
citasMedicas/
├── src/
│   ├── Domain/           ← Capa más interna (sin dependencias)
│   ├── Core/             ← Lógica de aplicación (CQRS, DTOs, MediatR)
│   ├── Persistence/      ← Acceso a datos (EF Core, Repositorios)
│   ├── Infrastructure/   ← Servicios externos (HTTP, Email, etc.)
│   └── Api/              ← Presentación (Controllers, Program.cs)
```

### Cadena de dependencias

```
Api → Domain, Core, Persistence, Infrastructure
Persistence → Domain, Core
Core → Domain
Infrastructure → Domain, Core
Domain → (nada)
```

**Regla:** Domain NUNCA depende de otros proyectos. Es la capa más pura.

---

## 2. Patrón Repository Genérico

Un solo par de archivos maneja **toda la persistencia** del proyecto:

### IRepository\<T\> (Interfaz)

**Ubicación:** `src/Persistence/Interfaces/IRepository.cs`

```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### Repository\<T\> (Implementación)

**Ubicación:** `src/Persistence/Repositories/Repository.cs`

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;
    // CRUD completo con soporte para Include
}
```

### Registro en DI

**Ubicación:** `src/Persistence/Extensions/PersistenceExtensions.cs`

```csharp
services.AddScoped<IRepository<Servicio>, Repository<Servicio>>();
services.AddScoped<IRepository<Tarifa>, Repository<Tarifa>>();
```

### Uso en Handlers

```csharp
public class CrearServicioHandler : IRequestHandler<CrearServicioCommand, int>
{
    private readonly IRepository<Servicio> _repository;

    public CrearServicioHandler(IRepository<Servicio> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CrearServicioCommand request, CancellationToken ct)
    {
        var servicio = new Servicio { Codigo = request.Codigo, Nombre = request.Nombre };
        await _repository.AddAsync(servicio);
        return servicio.ServicioID;
    }
}
```

### Para agregar una nueva entidad

Solo agrega 1 línea en `PersistenceExtensions.cs`:

```csharp
services.AddScoped<IRepository<Paciente>, Repository<Paciente>>();
```

**NO necesitas crear interfaces ni repositorios por cada entidad.**

---

## 3. CQRS con MediatR

**CQRS** = Command Query Responsibility Segregation (Separar lecturas de escrituras)

### Commands (Escritura)

| Elemento | Descripción |
|----------|-------------|
| **Command** | Objeto que representa una intención de negocio (Crear, Actualizar, Eliminar) |
| **Handler** | Lógica que ejecuta el command |
| **Retorno** | Generalmente el ID del recurso creado |

**Estructura de carpeta:**

```
Features/
└── Billing/
    └── Servicios/
        └── Commands/
            └── CrearServicio/
                └── CrearServicioCommand.cs  (Command + Handler en mismo archivo)
```

**Ejemplo:**

```csharp
public class CrearServicioCommand : IRequest<int>
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class CrearServicioCommandHandler : IRequestHandler<CrearServicioCommand, int>
{
    public async Task<int> Handle(CrearServicioCommand request, CancellationToken ct)
    {
        // Lógica de negocio
    }
}
```

### Queries (Lectura)

| Elemento | Descripción |
|----------|-------------|
| **Query** | Objeto que representa una consulta (Obtener, Buscar, Filtrar) |
| **Handler** | Lógica que ejecuta la query |
| **Retorno** | DTO con los datos |

**Estructura de carpeta:**

```
Features/
└── Billing/
    └── Servicios/
        └── Queries/
            ├── ObtenerServicios/
            │   └── ObtenerServiciosQuery.cs
            └── ObtenerServicioPorId/
                └── ObtenerServicioPorIdQuery.cs
```

**Ejemplo:**

```csharp
public class ObtenerServicioPorIdQuery : IRequest<ServicioDto?>
{
    public int ServicioID { get; set; }
}

public class ObtenerServicioPorIdQueryHandler : IRequestHandler<ObtenerServicioPorIdQuery, ServicioDto?>
{
    private readonly IRepository<Servicio> _repository;

    public async Task<ServicioDto?> Handle(ObtenerServicioPorIdQuery request, CancellationToken ct)
    {
        var servicio = await _repository.GetByIdAsync(request.ServicioID);
        return servicio is null ? null : new ServicioDto { ... };
    }
}
```

### DTOs (Data Transfer Objects)

**Ubicación:** `src/Core/DTOs/`

```csharp
public class ServicioDto
{
    public int ServicioID { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
```

---

## 4. Organización de Microservicios

### Regla de oro: Organizar por CASO DE USO, no por tabla

**NO hacer esto:**

```
Billing.Service (8 tablas)
├── ServiciosController.cs      ← 1 controller por tabla ❌
├── TarifasController.cs        ← 1 controller por tabla ❌
├── FacturasController.cs       ← 1 controller por tabla ❌
├── FacturaDetalleController.cs ← 1 controller por tabla ❌
├── FormasPagoController.cs     ← 1 controller por tabla ❌
├── PagosController.cs          ← 1 controller por tabla ❌
└── CreditosController.cs       ← 1 controller por tabla ❌
```

**Sí hacer esto:**

```
Billing.Service (8 tablas, 3 controllers)
├── ServiciosController.cs      ← Agrupa Servicios + Tarifas
├── FacturacionController.cs    ← Agrupa Facturas + Detalle + Pagos
└── CreditoController.cs        ← Creditos Médicos
```

### Controllers por microservicio

| Microservicio | Tablas | Controllers | Features CQRS |
|---------------|--------|-------------|---------------|
| **Identity** | 13 | 3-4 | 12-15 |
| **Scheduling** | 9 | 2-3 | 8-10 |
| **ClinicalRecords** | 25 | 5-6 | 18-22 |
| **Laboratory** | 4 | 1-2 | 4-6 |
| **Billing** | 8 | 3 | 10-12 |
| **Notification** | 1 | 1 | 2-3 |
| **TOTAL** | **60** | **~15-19** | **~54-68** |

### Ejemplo: Billing.Service

| Controller | Tablas que maneja | Features CQRS |
|------------|-------------------|---------------|
| `ServiciosController` | Servicios, Tarifas | CrearServicio, ObtenerServicios, ObtenerServicioPorId, CrearTarifa, ObtenerTarifasPorServicio |
| `FacturacionController` | Facturas, Factura_Detalle, Pagos, Formas_Pago | CrearFactura, ObtenerFacturaPorId, RegistrarPago |
| `CreditoController` | Creditos_Médicos | SolicitarCredito |

### ¿Por qué no 1 controller por tabla?

1. **Las tablas están relacionadas** - `Factura_Detalle` no existe sin `Factura`. Un `CrearFacturaCommand` crea ambas en una transacción.

2. **Los controllers agrupan por dominio** - `FacturacionController` maneja todo lo relacionado con facturación.

3. **Los Commands/Queries son por CASO DE USO** - No por tabla. `RegistrarPagoCommand` toca `Pagos` Y actualiza `Facturas`.

4. **Menos archivos, más cohesión** - En vez de 40 archivos repetitivos, tienes 10-12 archivos que representan operaciones reales del negocio.

---

## 5. Estructura de Proyectos

### Opción A: Monolito con Clean Architecture (Estado actual)

```
src/
├── Domain/          ← 1 solo proyecto para todos los dominios
├── Core/            ← 1 solo proyecto para toda la lógica
├── Persistence/     ← 1 solo proyecto para toda la persistencia
├── Infrastructure/  ← Servicios externos
└── Api/             ← 1 solo proyecto API
```

**Ventajas:**
- Simple de desarrollar y desplegar
- Compartir código fácilmente
- Una sola base de datos

**Desventajas:**
- No se puede escalar por servicio
- Un error puede afectar todo el sistema
- Despliegue monolítico

### Opción B: Microservicios separados (Propuesto)

```
src/
├── Shared.Kernel/
│   ├── Entities/BaseEntity.cs
│   ├── Interfaces/IRepository.cs
│   └── Extensions/AuthExtensions.cs
│
├── Identity.Service/
│   ├── Identity.Domain/
│   ├── Identity.Application/
│   ├── Identity.Persistence/
│   └── Identity.Api/
│
├── Scheduling.Service/
│   ├── Scheduling.Domain/
│   ├── Scheduling.Application/
│   ├── Scheduling.Persistence/
│   └── Scheduling.Api/
│
├── ClinicalRecords.Service/
│   ├── Clinical.Domain/
│   ├── Clinical.Application/
│   ├── Clinical.Persistence/
│   └── Clinical.Api/
│
├── Laboratory.Service/
│   ├── Lab.Domain/
│   ├── Lab.Application/
│   ├── Lab.Persistence/
│   └── Lab.Api/
│
└── Billing.Service/
    ├── Billing.Domain/
    ├── Billing.Application/
    ├── Billing.Persistence/
    └── Billing.Api/
```

**Ventajas:**
- Escalar por servicio
- Despliegue independiente
- Falla aislada

**Desventajas:**
- Complejidad de infraestructura
- Comunicación entre servicios
- Datos distribuidos

### Shared.Kernel (Código compartido)

```csharp
// BaseEntity.cs
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

// IRepository.cs (el que ya tenemos)
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

---

## 6. Tabla Resumen de Implementación

### Dependencias entre microservicios

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

**Flujo unidireccional. Sin dependencias circulares.**

### Orden de implementación

| Paso | Servicio | Dependencias |
|------|----------|--------------|
| 1 | Shared.Kernel | Ninguna |
| 2 | Identity.Service | Shared.Kernel |
| 3 | Scheduling.Service | Identity |
| 4 | ClinicalRecords.Service | Identity, Scheduling |
| 5 | Laboratory.Service | ClinicalRecords, Identity |
| 6 | Billing.Service | Scheduling, ClinicalRecords |
| 7 | Notification.Service | Todos |

### Base de datos

| Decisión | Valor |
|----------|-------|
| Motor | SQL Server (LocalDB) |
| Patrón | Shared database (una sola BD para todos los servicios) |
| Nombre | `ClinicaDB` |

Cada microservicio usa sus tablas por convención, no por restricción técnica. El patrón microservicios se demuestra a nivel de código y arquitectura.

---

## 7. Paquetes NuGet por servicio

| Paquete | Uso |
|---------|-----|
| Microsoft.EntityFrameworkCore 9.0.0 | ORM |
| Microsoft.EntityFrameworkCore.SqlServer 9.0.0 | Provider SQL Server |
| Microsoft.AspNetCore.Authentication.JwtBearer | Validación JWT |
| System.IdentityModel.Tokens.Jwt | Generación de tokens |
| BCrypt.Net-Next | Hashing de passwords |
| FluentValidation | Validación de DTOs |
| MediatR | Patrón Mediator para CQRS |

---

## 8. Estructura de Features CQRS (Vertical Slice)

Cada feature es una carpeta independiente con todo lo que necesita:

```
Features/
└── Billing/
    └── Facturacion/
        └── CrearFactura/
            ├── CrearFacturaCommand.cs      ← Command + Handler
            ├── CrearFacturaDto.cs          ← DTO de entrada (si aplica)
            └── CrearFacturaValidator.cs    ← Validación (FluentValidation)
```

**Ventajas:**
- Cada feature es autocontenida
- Fácil de encontrar y mantener
- No hay dependencias entre features
- Escalable horizontalmente

---

*Documento generado para el proyecto Citas Médicas - Arquitectura Hexagonal con CQRS y Microservicios*
