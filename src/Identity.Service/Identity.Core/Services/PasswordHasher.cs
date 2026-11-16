using System.Security.Cryptography;
using Identity.Core.Interfaces;

namespace Identity.Core.Services;

/// <summary>
/// PBKDF2 con SHA-256. Usa solo APIs incluidas en .NET (sin dependencias extra).
/// Formato: <c>PBKDF2$&lt;iteraciones&gt;$&lt;saltBase64&gt;$&lt;hashBase64&gt;</c>.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const string Prefijo = "PBKDF2";
    private const int Iteraciones = 100_000;
    private const int TamanioSalt = 16;    // 128 bits
    private const int TamanioHash = 32;    // 256 bits (SHA-256)

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(TamanioSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanioHash);

        return string.Join('$', Prefijo, Iteraciones, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public bool Verify(string password, string hashAlmacenado)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(hashAlmacenado))
            return false;

        var partes = hashAlmacenado.Split('$');
        if (partes.Length != 4 || !partes[0].Equals(Prefijo, StringComparison.Ordinal))
            return false;

        if (!int.TryParse(partes[1], out var iteraciones) || iteraciones < 1)
            return false;

        byte[] salt, hashEsperado;
        try
        {
            salt = Convert.FromBase64String(partes[2]);
            hashEsperado = Convert.FromBase64String(partes[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (salt.Length == 0 || hashEsperado.Length == 0)
            return false;

        var hashObtenido = Rfc2898DeriveBytes.Pbkdf2(password, salt, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

        // Comparación en tiempo constante para no filtrar la posición del fallo.
        return CryptographicOperations.FixedTimeEquals(hashObtenido, hashEsperado);
    }
}
