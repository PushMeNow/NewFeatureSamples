using System.Diagnostics;

namespace OpenTelemetry.Shared;

internal sealed class DiagnosticSourceListener(ListenerHandler handler, Action<string, string, Exception>? logUnknownException)
	: IObserver<KeyValuePair<string, object?>>
{
	public void OnCompleted()
	{
	}

	public void OnError(Exception error)
	{
	}

	public void OnNext(KeyValuePair<string, object?> value)
	{
		if (!handler.SupportsNullActivity && Activity.Current == null)
		{
			return;
		}

		try
		{
			handler.OnEventWritten(value.Key, value.Value);
		}
		catch (Exception ex)
		{
			logUnknownException?.Invoke(handler.SourceName, value.Key, ex);
		}
	}
}
