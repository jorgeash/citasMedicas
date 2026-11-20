using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Lab.Infrastructure.Services;

internal sealed class ServiceTokenHandler : DelegatingHandler
{
    private static readonly TimeSpan MargenRenovacion = TimeSpan.FromSeconds(60);

    private readonly ServiceAccountSettings _cuenta;
    private readonly HttpClient _identity;
    private readonly string _destino;
    private readonly SemaphoreSlim _candado = new(1, 1);

    private string? _token;
    private DateTime _venceUtc = DateTime.MinValue;

    public ServiceTokenHandler(ServiceAccountSettings cuenta, HttpClient identity, string destino)
    {
        _cuenta = cuenta;
        _identity = identity;
        _destino = destino;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await AdjuntarTokenAsync(request, forceRefresh: false, cancellationToken);

        var respuesta = await base.SendAsync(request, cancellationToken);

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
        {
            respuesta.Dispose();

            await AdjuntarTokenAsync(request, forceRefresh: true, cancellationToken);
            respuesta = await base.SendAsync(request, cancellationToken);
        }

        return respuesta;
    }

    private async Task AdjuntarTokenAsync(
        HttpRequestMessage request,
        bool forceRefresh,
        CancellationToken cancellationToken)
    {
        var token = await ObtenerTokenAsync(forceRefresh, cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string> ObtenerTokenAsync(bool forceRefresh, CancellationToken cancellationToken)
    {
        if (!forceRefresh && _token is not null && DateTime.UtcNow < _venceUtc - MargenRenovacion)
            return _token;

        await _candado.WaitAsync(cancellationToken);
        try
        {
            if (!forceRefresh && _token is not null && DateTime.UtcNow < _venceUtc - MargenRenovacion)
                return _token;

            return await LoginAsync(cancellationToken);
        }
        finally
        {
            _candado.Release();
        }
    }

    private async Task<string> LoginAsync(CancellationToken cancellationToken)
    {
        using var respuesta = await _identity.PostAsJsonAsync(
            "api/auth/login",
            new { nombreUsuario = _cuenta.Username, password = _cuenta.Password },
            cancellationToken);

        if (!respuesta.IsSuccessStatusCode)
        {
            var cuerpo = await respuesta.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Laboratory no pudo autenticarse contra Identity.Api para llamar a {_destino} " +
                $"({(int)respuesta.StatusCode}). Revise Services:{_destino}:ServiceAccount y que " +
                $"Identity.Api esté en ejecución. Detalle: {cuerpo}");
        }

        var auth = await respuesta.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Identity.Api devolvió una respuesta vacía en el login.");

        _token = auth.AccessToken;
        _venceUtc = DateTime.UtcNow.AddSeconds(auth.ExpiresIn);
        return _token;
    }

    private sealed record AuthResponse(string AccessToken, int ExpiresIn);
}

internal sealed class ServiceAccountSettings
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
