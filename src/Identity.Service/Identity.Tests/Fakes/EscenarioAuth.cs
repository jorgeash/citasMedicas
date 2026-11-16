using Identity.Core.Options;
using Identity.Core.Services;
using Identity.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Identity.Tests.Fakes;

/// <summary>Fábrica de subject bajo prueba y sus dependencias en memoria.</summary>
public sealed class EscenarioAuth
{
    public const string ClaveJwt =
        "C3I8uRO5bTBEQ/n02bqxT5YtQMddqUzCi2JGQgEkasyO0dJBj/5T6xYtvQAnMkhh";

    public FakeUsuarioRepository Usuarios { get; } = new();
    public FakePermisoRepository Permisos { get; } = new();
    public FakeSesionRepository Sesiones { get; } = new();
    public PasswordHasher Hasher { get; } = new();
    public TokenService Tokens { get; }

    /// <summary>RolID 1 = Administrador, RolID 2 = Médico (los mismos del seed).</summary>
    public List<Rol> Roles { get; } = new()
    {
        new Rol { RolID = 1, Nombre = "Administrador", Descripcion = "Acceso total al sistema", Activo = true },
        new Rol { RolID = 2, Nombre = "Médico", Descripcion = "Usuarios vinculados a medicos", Activo = true }
    };

    public EscenarioAuth(Action<JwtOptions>? configurar = null)
    {
        var opciones = new JwtOptions
        {
            Issuer = "CitasMedicas.Identity",
            Audience = "CitasMedicas.Frontend",
            SigningKey = ClaveJwt,
            AccessTokenMinutes = 60,
            RefreshTokenDays = 7
        };

        configurar?.Invoke(opciones);
        Tokens = new TokenService(Options.Create(opciones));

        // El admin está siempre disponible: la mayoría de los tests parte de él.
        SembrarAdmin();
    }

    /// <summary>Usuario admin (RolID 1) con permisos y contraseña <c>Admin123!</c>.</summary>
    public Usuario SembrarAdmin()
    {
        var existente = Usuarios.Items.FirstOrDefault(u => u.UsuarioID == 1);
        if (existente is not null)
            return existente;

        Permisos.Seed(
            new Permiso { PermisoID = 1, Codigo = "facturas.leer", Nombre = "Ver facturas" },
            new Permiso { PermisoID = 2, Codigo = "usuarios.gestionar", Nombre = "Gestionar usuarios" });

        Permisos.AsignarRol(rolID: 1, "facturas.leer", "usuarios.gestionar");

        var admin = new Usuario
        {
            UsuarioID = 1,
            RolID = 1,
            SucursalID = 1,
            NombreUsuario = "admin",
            NombreCompleto = "Administrador del Sistema",
            Email = "admin@clinica.local",
            PasswordHash = Hasher.Hash("Admin123!"),
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            // La navegación viene cargada por Include(Rol) en el repo real; en
            // memoria hay que ponerla a mano para que el DTO salga igual.
            Rol = RolAdministrador()
        };

        Usuarios.Seed(admin);
        return admin;
    }

    /// <summary>Médico (RolID 2) con permiso de lectura de citas, contraseña <c>Medico123!</c>.</summary>
    public Usuario SembrarMedico()
    {
        var existente = Usuarios.Items.FirstOrDefault(u => u.UsuarioID == 2);
        if (existente is not null)
            return existente;

        Permisos.Seed(new Permiso { PermisoID = 3, Codigo = "citas.leer", Nombre = "Ver citas" });
        Permisos.AsignarRol(rolID: 2, "citas.leer");

        var medico = new Usuario
        {
            UsuarioID = 2,
            RolID = 2,
            NombreUsuario = "lcastro",
            NombreCompleto = "Luis Fernando Castro",
            PasswordHash = Hasher.Hash("Medico123!"),
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            Rol = Roles.Single(r => r.RolID == 2)
        };

        Usuarios.Seed(medico);
        return medico;
    }

    private Rol RolAdministrador() => Roles.Single(r => r.RolID == 1);
}
