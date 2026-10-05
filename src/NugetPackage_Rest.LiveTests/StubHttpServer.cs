using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NugetPackage_Rest.LiveTests;

/// <summary>
/// Servidor HTTP real (TcpListener) para las pruebas del Anexo de la guía: la librería no se
/// simula con un mock, hace llamadas HTTP verdaderas contra este servidor. Se usa en lugar de
/// HttpListener para no depender de permisos de URL ACL en Windows.
/// </summary>
public sealed class StubHttpServer : IDisposable
{
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();
    private readonly Dictionary<string, StubResponse> _respuestas = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<RecordedRequest> _requests = new();
    private readonly Task _loop;

    public StubHttpServer()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
        _loop = Task.Run(AcceptLoopAsync);
    }

    public int Port { get; }

    public string BaseUri => $"http://127.0.0.1:{Port}";

    public IReadOnlyList<RecordedRequest> Requests => _requests;

    public StubHttpServer Map(string path, string body, HttpStatusCode statusCode = HttpStatusCode.OK, TimeSpan? delay = null)
    {
        _respuestas[path] = new StubResponse(statusCode, body, delay, Abort: false);
        return this;
    }

    /// <summary>Cierra la conexión sin responder: simula una caída de red durante la respuesta.</summary>
    public StubHttpServer MapAbort(string path)
    {
        _respuestas[path] = new StubResponse(HttpStatusCode.OK, string.Empty, null, Abort: true);
        return this;
    }

    private async Task AcceptLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await _listener.AcceptTcpClientAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            _ = Task.Run(() => HandleAsync(client));
        }
    }

    private async Task HandleAsync(TcpClient client)
    {
        try
        {
            using (client)
            await using (var stream = client.GetStream())
            {
                var request = await ReadRequestAsync(stream);
                _requests.Add(request);

                var respuesta = _respuestas.TryGetValue(request.PathOnly, out var r)
                    ? r
                    : new StubResponse(HttpStatusCode.NotFound, "{\"error\":\"ruta no encontrada\"}", null, Abort: false);

                if (respuesta.Abort)
                {
                    client.Close();
                    return;
                }

                if (respuesta.Delay.HasValue)
                    await Task.Delay(respuesta.Delay.Value, _cts.Token);

                var body = Encoding.UTF8.GetBytes(respuesta.Body);
                var header = new StringBuilder();
                header.Append($"HTTP/1.1 {(int)respuesta.StatusCode} {respuesta.StatusCode}\r\n");
                header.Append("Content-Type: application/json; charset=utf-8\r\n");
                header.Append($"Content-Length: {body.Length}\r\n");
                header.Append("Connection: close\r\n\r\n");

                var headerBytes = Encoding.ASCII.GetBytes(header.ToString());
                await stream.WriteAsync(headerBytes, _cts.Token);
                await stream.WriteAsync(body, _cts.Token);
                await stream.FlushAsync(_cts.Token);
            }
        }
        catch (Exception)
        {
            // El cliente puede cerrar el socket antes (por ejemplo, tras un timeout).
        }
    }

    private static async Task<RecordedRequest> ReadRequestAsync(NetworkStream stream)
    {
        var buffer = new byte[16 * 1024];
        var received = new List<byte>();

        while (true)
        {
            var read = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (read == 0) break;
            received.AddRange(buffer.Take(read));

            var text = Encoding.UTF8.GetString(received.ToArray());
            var headerEnd = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (headerEnd < 0) continue;

            var headers = text[..headerEnd].Split("\r\n");
            var requestLine = headers[0].Split(' ');
            var contentLength = 0;
            foreach (var header in headers.Skip(1))
            {
                if (header.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                    contentLength = int.Parse(header["Content-Length:".Length..].Trim());
            }

            var bodyStart = headerEnd + 4;
            var body = text[bodyStart..];
            if (Encoding.UTF8.GetByteCount(body) >= contentLength)
            {
                var headersDictionary = headers.Skip(1)
                    .Select(h => h.Split(": ", 2))
                    .Where(p => p.Length == 2)
                    .ToDictionary(p => p[0], p => p[1], StringComparer.OrdinalIgnoreCase);

                return new RecordedRequest(requestLine[0], requestLine[1], headersDictionary, body);
            }
        }

        return new RecordedRequest("UNKNOWN", "/", new Dictionary<string, string>(), string.Empty);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _listener.Stop();
        try { _loop.Wait(TimeSpan.FromSeconds(2)); } catch { }
        _cts.Dispose();
    }

    private sealed record StubResponse(HttpStatusCode StatusCode, string Body, TimeSpan? Delay, bool Abort);
}

public sealed record RecordedRequest(string Method, string PathAndQuery, Dictionary<string, string> Headers, string Body)
{
    /// <summary>Ruta sin query string, para casar contra las rutas configuradas con Map.</summary>
    public string PathOnly => PathAndQuery.Split('?')[0];

    public string? Header(string name) => Headers.TryGetValue(name, out var value) ? value : null;
}