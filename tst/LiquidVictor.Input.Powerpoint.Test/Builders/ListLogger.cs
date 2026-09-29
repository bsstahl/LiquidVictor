using Microsoft.Extensions.Logging;

namespace LiquidVictor.Input.Powerpoint.Test.Builders;

/// <summary>
/// An in-memory logger that captures log entries so tests can verify logging behavior
/// </summary>
public sealed class ListLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, EventId EventId, string Message, Exception? Exception)> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);
        this.Entries.Add((logLevel, eventId, formatter(state, exception), exception));
    }
}
