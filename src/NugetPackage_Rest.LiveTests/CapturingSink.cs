using System.Collections.Concurrent;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace NugetPackage_Rest.LiveTests;

/// <summary>Sink de Serilog que guarda los eventos emitidos para poder verificarlos en los tests.</summary>
public sealed class CapturingSink : ILogEventSink
{
    private readonly ConcurrentQueue<LogEvent> _events = new();

    public IReadOnlyList<LogEvent> Events => _events.ToArray();

    public void Emit(LogEvent logEvent) => _events.Enqueue(logEvent);

    public bool Contains(string fragment) =>
        Events.Any(e => e.RenderMessage().Contains(fragment, StringComparison.OrdinalIgnoreCase));

    public void Reset() => _events.Clear();
}
