using Identity.Core.Services;

namespace Identity.Tests.Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void Hash_DevuelveElFormatoPbkdf2ConCuatroPartes()
    {
        var hash = _hasher.Hash("Admin123!");

        var partes = hash.Split('$');
        Assert.Equal(4, partes.Length);
        Assert.Equal("PBKDF2", partes[0]);
        Assert.Equal(100000, int.Parse(partes[1]));
        Assert.NotEmpty(partes[2]);
        Assert.NotEmpty(partes[3]);
    }

    [Fact]
    public void Hash_NoAlmacenaLaPasswordEnClaro()
    {
        var hash = _hasher.Hash("Admin123!");

        Assert.DoesNotContain("Admin123!", hash, StringComparison.Ordinal);
    }

    [Fact]
    public void Hash_DosVecesUsaSalDistinta()
    {
        var primero = _hasher.Hash("Admin123!");
        var segundo = _hasher.Hash("Admin123!");

        // Misma contraseña, sal distinta → hash distinto (nunca se comparte sal).
        Assert.NotEqual(primero, segundo);
    }

    [Fact]
    public void Verify_ConLaMismaPassword_DevuelveTrue()
    {
        var hash = _hasher.Hash("Admin123!");

        Assert.True(_hasher.Verify("Admin123!", hash));
    }

    [Fact]
    public void Verify_ConOtraPassword_DevuelveFalse()
    {
        var hash = _hasher.Hash("Admin123!");

        Assert.False(_hasher.Verify("admin123!", hash));
        Assert.False(_hasher.Verify("Admin123", hash));
        Assert.False(_hasher.Verify("", hash));
    }

    [Fact]
    public void Verify_ConElHashDummyDelSeed_DevuelveFalse()
    {
        // Los usuarios sembrados por Scheduling traen este placeholder: no deben
        // autenticar a nadie.
        Assert.False(_hasher.Verify("Admin123!", "SEED-DUMMY-NO-USAR"));
    }

    [Fact]
    public void Verify_ConHashCorrompido_NoLanzaYDevuelveFalse()
    {
        Assert.False(_hasher.Verify("Admin123!", "esto-no-es-un-hash"));
        Assert.False(_hasher.Verify("Admin123!", "PBKDF2$abc$!!!$!!!"));
        Assert.False(_hasher.Verify("Admin123!", "PBKDF2$100000$YWJj"));
        Assert.False(_hasher.Verify("Admin123!", ""));
    }

    [Fact]
    public void Verify_ReconoceUnaPasswordDeOtroHashDelSeed()
    {
        // Regresión del formato real usado en seed-identity.sql.
        var hash = _hasher.Hash("Medico123!");

        Assert.True(_hasher.Verify("Medico123!", hash));
        Assert.False(_hasher.Verify("Admin123!", hash));
    }
}
