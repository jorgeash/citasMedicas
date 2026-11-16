using Identity.Domain.Entities;
using Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Identity.Tests.Tests;

/// <summary>
/// Congela la forma del modelo EF contra las tablas reales de ClinicaDB.
///
/// Regresión conocida: declarar <c>HasOne&lt;T&gt;()</c> SIN la expresión de
/// navegación en <c>UsingEntity&lt;RolPermiso&gt;</c> hace que EF descubra las
/// navegaciones por convención y cree una SEGUNDA relación con FK sombra
/// <c>PermisoID1</c>; la consulta de permisos generaba SQL con esa columna
/// inexistente y el login devolvía 500.
/// </summary>
public class MapeoIdentityTests
{
    private static IdentityDbContext CrearContexto()
    {
        // El modelo se construye con el proveedor SQL Server pero NUNCA se
        // conecta: ninguna de estas pruebas necesita la base de datos.
        var opciones = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=ClinicaDB;Trusted_Connection=True;")
            .Options;

        return new IdentityDbContext(opciones);
    }

    [Fact]
    public void RolPermiso_SoloTieneLasDosColumnasDeLaTabla()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(typeof(RolPermiso));
        Assert.NotNull(entidad);

        // Si volviera a aparecer una FK sombra (PermisoID1/RolID1) este
        // select falla y muestra el nombre exacto del culpable.
        var propiedades = entidad!.GetProperties()
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(new[] { "PermisoID", "RolID" }, propiedades);
    }

    [Fact]
    public void RolPermiso_TieneExactamenteDosForeignKeys()
    {
        using var contexto = CrearContexto();

        var entidad = contexto.Model.FindEntityType(typeof(RolPermiso))!;
        var foreignKeys = entidad.GetForeignKeys().ToList();

        // Una a Roles y otra a Permisos: una tercera revela una relación
        // duplicada por convención.
        Assert.Equal(2, foreignKeys.Count);
        Assert.All(foreignKeys, fk => Assert.False(fk.IsUnique));
    }

    [Fact]
    public void ConsultaDePermisosPorRol_NoReferenciaColumnasInexistentes()
    {
        using var contexto = CrearContexto();

        var sql = contexto.Set<RolPermiso>()
            .Where(rp => rp.RolID == 1)
            .Select(rp => rp.Permiso!.Codigo)
            .ToQueryString();

        Assert.Contains("Rol_Permiso", sql);
        Assert.DoesNotContain("PermisoID1", sql);
        Assert.DoesNotContain("RolID1", sql);
    }

    [Fact]
    public void ConsultaDePermisosDelRolPorInclude_NoReferenciaColumnasInexistentes()
    {
        using var contexto = CrearContexto();

        // Mismo camino que usa el login: Usuario → Rol → Permisos.
        var sql = contexto.Usuarios
            .Where(u => u.NombreUsuario == "admin")
            .Select(u => u.Rol!.Permisos.Select(p => p.Codigo))
            .ToQueryString();

        Assert.DoesNotContain("PermisoID1", sql);
        Assert.DoesNotContain("RolID1", sql);
    }
}
