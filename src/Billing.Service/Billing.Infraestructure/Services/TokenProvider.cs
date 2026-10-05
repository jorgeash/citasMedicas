using Billing.Core.Configs;
using Billing.Core.DTOs.External;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Interfaces.IServices;

namespace Billing.Infraestructure.Services;

/// <summary>
/// Obtiene un token OAuth 2.0 (client credentials) de la pasarela y lo guarda en memoria
/// hasta un minuto antes de que expire. Se registra como Singleton para que la caché
/// sobreviva entre requests.
/// </summary>
/// <remarks>
/// Al ser Singleton no puede inyectar <see cref="IRest"/> directamente: el typed client es
/// transitorio, así que lo capturaría para siempre y perdería la rotación de handlers y el
/// refresco de DNS que IHttpClientFactory garantiza. Por eso recibe un
/// <see cref="IServiceScopeFactory"/> y resuelve un IRest nuevo en cada refresco de token.
/// </remarks>
internal class TokenProvider : ITokenProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PasarelaOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private string? _token;
    private DateTime _expiraUtc;

    public TokenProvider(IServiceScopeFactory scopeFactory, IOptions<PasarelaOptions> options)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_token != null && DateTime.UtcNow < _expiraUtc) return _token;

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_token != null && DateTime.UtcNow < _expiraUtc) return _token;

            using var scope = _scopeFactory.CreateScope();
            var rest = scope.ServiceProvider.GetRequiredService<IRest>();

            var response = await rest.Post
                .WithoutAuth()
                .WithUri(_options.BaseUrl, "/oauth/token")
                .WithFormUrlEncoded(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _options.ClientId,
                    ["client_secret"] = _options.ClientSecret
                })
                .DeserializeWithAsync<TokenResponse>(cancellationToken);

            if (string.IsNullOrWhiteSpace(response?.AccessToken))
                throw new DomainException(Errores.AUTENTICACION_FALLIDA, "La pasarela no devolvió un token de acceso.");

            _token = response.AccessToken;
            _expiraUtc = DateTime.UtcNow.AddSeconds(response.ExpiresIn - 60);
            return _token;
        }
        finally
        {
            _lock.Release();
        }
    }
}
