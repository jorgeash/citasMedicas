namespace Notifications.Tests.Fakes;

/// <summary>
/// Sustituye la red: devuelve una respuesta fabricada o lanza la excepción indicada.
/// Copia del fake del proyecto de pruebas de la librería, con el namespace de este servicio.
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage>? _responseFactory;
    private readonly Exception? _exceptionToThrow;

    public List<HttpRequestMessage> Requests { get; } = new();

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public FakeHttpMessageHandler(Exception exceptionToThrow)
    {
        _exceptionToThrow = exceptionToThrow;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);

        if (_exceptionToThrow != null)
            throw _exceptionToThrow;

        return Task.FromResult(_responseFactory!(request));
    }
}
