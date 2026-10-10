namespace Business.Persistence.Configurations;

/// <summary>
/// Fecha fija de los datos de prueba (HasData). Se usa una constante para que la migracion
/// sea determinista y el snapshot del modelo no cambie en cada compilacion.
/// </summary>
public static class SeedDefaults
{
    public static readonly DateTime CreatedAt = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
}