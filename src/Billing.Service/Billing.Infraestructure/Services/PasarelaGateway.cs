using Billing.Core.Configs;
using Billing.Core.DTOs.External;
using Billing.Core.Exceptions;
using Billing.Core.Interfaces.Services;
using Microsoft.Extensions.Options;
using NugetPackage_Rest.Exceptions;
using NugetPackage_Rest.Interfaces.IServices;

namespace Billing.Infraestructure.Services;

/// <summary>
/// Cobro contra la pasarela externa: POST con Bearer y reintentos solo ante fallas
/// transitorias (Timeout/Network). Un HttpError —por ejemplo, una tarjeta rechazada—
/// no se reintenta: el resultado será el mismo.
/// </summary>
internal class PasarelaGateway : IPasarelaGateway
{
    private readonly IRest _rest;
    private readonly ITokenProvider _tokenProvider;
    private readonly PasarelaOptions _options;

    public PasarelaGateway(IRest rest, ITokenProvider tokenProvider, IOptions<PasarelaOptions> options)
    {
        _rest = rest;
        _tokenProvider = tokenProvider;
        _options = options.Value;
    }

    public async Task<CobroResponse> CobrarAsync(CobroRequest request, CancellationToken cancellationToken = default)
    {
        int totalIntentos = _options.MaxReintentos + 1;
        try
        {
            for (int intento = 1; intento <= totalIntentos; intento++)
            {
                try
                {
                    return await EnviarCobroAsync(request, cancellationToken);
                }
                catch (ApiException ex) when (EsReintentable(ex) && intento < totalIntentos)
                {
                    // Antes de reenviar, consultar si el cobro sí llegó a registrarse:
                    // reenviar un cobro ya aplicado cobraría dos veces al paciente.
                    var consulta = await ConsultarCobroAsync(request.Referencia, cancellationToken);
                    if (consulta.Estado == "APROBADO")
                        return consulta;
                }
                catch (ApiException ex) when (EsReintentable(ex))
                {
                    // Último intento fallido: se sale del ciclo para traducir el error
                    // técnico a un error de negocio.
                }
            }
        }
        catch (ApiException ex) when (ex.Reason != ApiFailureReason.HttpError)
        {
            // Traduce la falla técnica (red, timeout, JSON inválido) a un error de negocio:
            // Core no debe conocer ApiException. Un HttpError sí se propaga, porque es el
            // rechazo de la pasarela y su body trae el detalle para el paciente.
            throw new DomainException(Errores.PASARELA_NO_DISPONIBLE,
                $"La pasarela no respondió al cobro de la referencia {request.Referencia}: {ex.Message}");
        }

        throw new DomainException(Errores.PASARELA_NO_DISPONIBLE,
            $"No fue posible cobrar la referencia {request.Referencia} tras {_options.MaxReintentos} reintentos.");
    }

    public async Task<CobroResponse> ConsultarCobroAsync(string referencia, CancellationToken cancellationToken = default)
    {
        var token = await _tokenProvider.GetTokenAsync(cancellationToken);

        return await _rest.Get
            .WithBearer(token)
            .WithUri(_options.BaseUrl, $"/v1/cobros/{referencia}")
            .DeserializeWithAsync<CobroResponse>(cancellationToken);
    }

    private async Task<CobroResponse> EnviarCobroAsync(CobroRequest request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync(cancellationToken);

        return await _rest.Post
            .WithBearer(token)
            .WithUri(_options.BaseUrl, "/v1/cobros")
            .WithBody(request)
            .DeserializeWithAsync<CobroResponse>(cancellationToken);
    }

    private static bool EsReintentable(ApiException ex) =>
        ex.Reason is ApiFailureReason.Timeout or ApiFailureReason.Network;
}