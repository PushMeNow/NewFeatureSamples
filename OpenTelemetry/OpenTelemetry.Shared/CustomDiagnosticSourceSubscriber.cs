using System.Diagnostics;

namespace OpenTelemetry.Shared;

public sealed class CustomDiagnosticSourceSubscriber(
	Func<string, ListenerHandler> handlerFactory,
	Func<DiagnosticListener, bool> diagnosticSourceFilter,
	Func<string, object?, object?, bool>? isEnabledFilter,
	Action<string, string, Exception> logUnknownException)
	: IDisposable, IObserver<DiagnosticListener>
{
	private readonly List<IDisposable> _listenerSubscriptions = new();
	private long _disposed;
	private IDisposable? _allSourcesSubscription;

	public CustomDiagnosticSourceSubscriber(ListenerHandler handler,
		Func<string, object?, object?, bool>? isEnabledFilter,
		Action<string, string, Exception> logUnknownException)
		: this(_ => handler, value => true, isEnabledFilter, logUnknownException)
	{
	}

	public void Subscribe()
	{
		_allSourcesSubscription ??= DiagnosticListener.AllListeners.Subscribe(this);
	}

	public void OnNext(DiagnosticListener value)
	{
		if ((Interlocked.Read(ref _disposed) == 0) &&
		    diagnosticSourceFilter(value))
		{
			var handler = handlerFactory(value.Name);
			var listener = new DiagnosticSourceListener(handler, logUnknownException);
			var subscription = isEnabledFilter == null ? value.Subscribe(listener) : value.Subscribe(listener, isEnabledFilter);

			lock (_listenerSubscriptions)
			{
				_listenerSubscriptions.Add(subscription);
			}
		}
	}

	public void OnCompleted()
	{
	}

	public void OnError(Exception error)
	{
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 1)
		{
			return;
		}

		lock (_listenerSubscriptions)
		{
			if (disposing)
			{
				foreach (var listenerSubscription in _listenerSubscriptions)
				{
					listenerSubscription?.Dispose();
				}
			}

			_listenerSubscriptions.Clear();
		}

		if (disposing)
		{
			_allSourcesSubscription?.Dispose();
		}

		_allSourcesSubscription = null;
	}
}
