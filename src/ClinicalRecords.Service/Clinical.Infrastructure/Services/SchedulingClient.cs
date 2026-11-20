using System.Net;
using System.Net.Http.Json;
using Clinical.Core.DTOs.External;
using Clinical.Core.Services;

namespace Clinical.Infrastructure.Services;

internal class SchedulingClient : ISchedulingClient
{
    private readonly HttpClient _http;

    public SchedulingClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<CitaExternaDto?> GetCitaAsync(long citaId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync($"api/citas/{citaId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CitaExternaDto>(cancellationToken: cancellationToken);
    }
}
