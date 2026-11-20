using System.Net;
using System.Net.Http.Json;
using Lab.Core.DTOs.External;
using Lab.Core.Services;

namespace Lab.Infrastructure.Services;

internal class ClinicalRecordsClient : IClinicalRecordsClient
{
    private readonly HttpClient _http;

    public ClinicalRecordsClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AtencionExternaDto?> GetAtencionAsync(
        long atencionId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync($"api/atenciones/{atencionId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AtencionExternaDto>(cancellationToken: cancellationToken);
    }
}
