using System.Net;
using System.Net.Http.Json;
using Clinical.Core.DTOs.External;
using Clinical.Core.Services;

namespace Clinical.Infrastructure.Services;

internal class IdentityClient : IIdentityClient
{
    private readonly HttpClient _http;

    public IdentityClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<MedicoExternoDto?> GetMedicoAsync(int medicoId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync($"api/medicos/{medicoId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MedicoExternoDto>(cancellationToken: cancellationToken);
    }
}
