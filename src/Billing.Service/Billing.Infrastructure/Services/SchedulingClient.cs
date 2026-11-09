using System.Net;
using System.Net.Http.Json;
using Billing.Core.DTOs.External;
using Billing.Core.Services;

namespace Billing.Infrastructure.Services;

/// <summary>Cliente HTTP síncrono contra Scheduling (paciente y cita).</summary>
internal class SchedulingClient : ISchedulingClient
{
    private readonly HttpClient _http;

    public SchedulingClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PacienteExternoDto?> GetPacienteAsync(int pacienteId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync($"api/pacientes/{pacienteId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PacienteExternoDto>(cancellationToken: cancellationToken);
    }

    public async Task<CitaExternaDto?> GetCitaAsync(int citaId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync($"api/citas/{citaId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CitaExternaDto>(cancellationToken: cancellationToken);
    }
}
