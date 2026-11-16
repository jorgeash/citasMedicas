namespace Identity.Core.Interfaces;

/// <summary>
/// Hash de contraseñas con PBKDF2 (Rfc2898DeriveBytes, SHA-256, 100 000 iteraciones).
/// Formato almacenado: <c>PBKDF2$&lt;iteraciones&gt;$&lt;saltBase64&gt;$&lt;hashBase64&gt;</c>.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hashAlmacenado);
}
