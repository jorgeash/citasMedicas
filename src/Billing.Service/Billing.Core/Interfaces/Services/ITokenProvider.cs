namespace Billing.Core.Interfaces.Services;

/// <summary>
/// Expone el token OAuth 2.0 de la pasarela de pagos. Se registra como Singleton para
/// que la caché sobreviva entre requests.
/// </summary>
public interface ITokenProvider
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}