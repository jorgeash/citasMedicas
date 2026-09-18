# PersistenceLibrary

Librería compartida para reutilizar interfaces, repositorios y entidades base en múltiples microservicios.

## Estructura

```
src/PersistenceLibrary/
├── PersistenceLibrary.csproj
├── Entities/
│   └── BaseEntity.cs
├── Interfaces/
│   ├── IRepository.cs
│   └── IUnitOfWork.cs
├── Repositories/
│   ├── Repository.cs
│   └── UnitOfWork.cs
└── Extensions/
    └── PersistenceLibraryExtensions.cs
```

## Componentes

### BaseEntity

Clase base abstracta para entidades con campos de auditoría.

```csharp
namespace PersistenceLibrary.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### IRepository\<T\>

Interfaz genérica para operaciones CRUD.

```csharp
namespace PersistenceLibrary.Interfaces;

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

### IUnitOfWork

Interfaz para transacciones.

```csharp
namespace PersistenceLibrary.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### Repository\<T\>

Implementación genérica con Entity Framework Core.

```csharp
namespace PersistenceLibrary.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    // Implementación completa con CRUD
}
```

## Cómo Usar en Otro Microservicio

### 1. Referenciar PersistenceLibrary

```xml
<!-- TuMicroservicio.csproj -->
<ItemGroup>
    <ProjectReference Include="..\PersistenceLibrary\PersistenceLibrary.csproj" />
</ItemGroup>
```

### 2. Crear Entidad

```csharp
using PersistenceLibrary.Entities;

namespace TuMicroservicio.Domain.Entities;

public class Paciente : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

### 3. Crear DbContext

```csharp
using Microsoft.EntityFrameworkCore;
using TuMicroservicio.Domain.Entities;

namespace TuMicroservicio.Persistence.Context;

public class TuDbContext : DbContext
{
    public TuDbContext(DbContextOptions<TuDbContext> options) : base(options) { }

    public DbSet<Paciente> Pacientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TuDbContext).Assembly);
    }
}
```

### 4. Configurar Entidad (Fluent API)

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TuMicroservicio.Domain.Entities;

namespace TuMicroservicio.Persistence.Configurations;

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Pacientes");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Apellido)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasMaxLength(200)
            .IsRequired();
    }
}
```

### 5. Registrar Servicios

```csharp
using PersistenceLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Registrar PersistenceLibrary ( repositories genéricos )
builder.Services.AddPersistenceLibrary();

// Registrar tu DbContext
builder.Services.AddDbContext<TuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TuConexion")));

var app = builder.Build();
```

### 6. Usar en Servicios/Handlers

```csharp
using PersistenceLibrary.Interfaces;
using TuMicroservicio.Domain.Entities;

namespace TuMicroservicio.Application.Services;

public class PacienteService
{
    private readonly IRepository<Paciente> _repository;

    public PacienteService(IRepository<Paciente> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Paciente>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Paciente?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Paciente> CreateAsync(Paciente paciente)
    {
        return await _repository.AddAsync(paciente);
    }
}
```

## Cadena de Dependencias

```
PersistenceLibrary  (sin dependencias - librería base)
       ↑
   Domain           (entidades de negocio)
       ↑
   Core             (CQRS, MediatR, lógica de aplicación)
       ↑
   Persistence      (DbContext, configuraciones, DI)
       ↑
   Api              (Controllers, Program.cs)
```

## Archivos de Ejemplo

### PersistenceLibrary.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
  </ItemGroup>

</Project>
```

### PersistenceLibraryExtensions.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using PersistenceLibrary.Interfaces;
using PersistenceLibrary.Repositories;

namespace PersistenceLibrary.Extensions;

public static class PersistenceLibraryExtensions
{
    public static IServiceCollection AddPersistenceLibrary(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
```

## Ventajas

| Beneficio | Descripción |
|-----------|-------------|
| **Reutilización** | Una sola librería para todos los microservicios |
| **Consistencia** | Mismas interfaces y patrones en todo el proyecto |
| **Mantenimiento** | Cambios en un solo lugar afectan a todos los servicios |
| **Rapid de desarrollo** | Nuevo microservicio listo en minutos |
| **Testing** | Interfaces fáciles de mockear para pruebas |

## Notas Importantes

1. **BaseEntity usa `int Id`** - Si necesitas `Guid` o `string`, crea tu propia base o modifica BaseEntity
2. **SQL Server** - La librería incluye SQL Server por defecto
3. **Sin dependencias** - PersistenceLibrary no depende de Domain ni Core
4. **Fluent API** - Cada microservicio configura sus entidades independientemente
