namespace Billing.Core.Interfaces.Services;

/// <summary>
/// Consulta al microservicio de Citas para saber si el servicio clínico sigue disponible
/// antes de facturarlo.
/// </summary>
public interface ICitaService
{
    Task<bool> EstaDisponibleAsync(int servicioId, DateTime fecha, CancellationToken cancellationToken = default);
}