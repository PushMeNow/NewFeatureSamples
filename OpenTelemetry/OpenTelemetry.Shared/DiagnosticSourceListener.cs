using System.Diagnostics;

namespace OpenTelemetry.Shared;

internal sealed class DiagnosticSourceListener : IObserver<KeyValuePair<string, object?>>
{
	private readonly ListenerHandler _handler;

	private readonly Action<string, string, Exception>? _logUnknownException;

	public DiagnosticSourceListener(ListenerHandler handler, Action<string, string, Exception>? logUnknownException)
	{
		_handler = handler;
		_logUnknownException = logUnknownException;
	}

	public void OnCompleted()
	{
	}

	public void OnError(Exception error)
	{
	}

	public void OnNext(KeyValuePair<string, object?> value)
	{
		if (!_handler.SupportsNullActivity && Activity.Current == null)
		{
			return;
		}

		try
		{
			_handler.OnEventWritten(value.Key, value.Value);
		}
		catch (Exception ex)
		{
			_logUnknownException?.Invoke(_handler.SourceName, value.Key, ex);
		}
	}
}
