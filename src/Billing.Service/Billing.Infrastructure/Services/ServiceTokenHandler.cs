using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Billing.Infrastructure.Services;

/// <summary>
/// Adjunta un JWT de la cuenta de servicio a cada llamada saliente a Scheduling
/// y lo renueva cuando vence.
///
/// Billing y Scheduling se comunican de servidor a servidor (sin navegador ni
/// usuario delante), así que Billing se autentica en Identity.Api con la cuenta
/// <c>svc-billing</c> y guarda el token mientras siga vigente. Si Scheduling
/// responde 401 (el token venció justo antes del envío) se reintenta una vez
/// con un token recién pedido.
/// </summary>
internal sealed class ServiceTokenHandler : DelegatingHandler
{
    private static readonly TimeSpan MargenRenovacion = TimeSpan.FromSeconds(60);

    private readonly ServiceAccountSettings _cuenta;
    private readonly HttpClient _identity;
    private readonly SemaphoreSlim _candado = new(1, 1);

    private string? _token;
    private DateTime _venceUtc = DateTime.MinValue;

    public ServiceTokenHandler(ServiceAccountSettings cuenta, HttpClient identity)
    {
        _cuenta = cuenta;
        _identity = identity;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await AdjuntarTokenAsync(request, forceRefresh: false, cancellationToken);

        var respuesta = await base.SendAsync(request, cancellationToken);

        // El token pudo vencer entre que lo leímos y que llegó: un solo reintento.
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
            // Otro hilo pudo renovarlo mientras esperábamos el candado.
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
                $"Billing no pudo autenticarse contra Identity.Api para llamar a Scheduling " +
                $"({(int)respuesta.StatusCode}). Revise Services:Scheduling:ServiceAccount y que " +
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

/// <summary>Credenciales de la cuenta de servicio con que Billing habla con Scheduling.</summary>
internal sealed class ServiceAccountSettings
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
