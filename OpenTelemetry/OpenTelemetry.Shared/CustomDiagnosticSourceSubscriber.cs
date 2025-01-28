using System.Diagnostics;

namespace OpenTelemetry.Shared;

public sealed class CustomDiagnosticSourceSubscriber : IDisposable, IObserver<DiagnosticListener>
{
	private readonly List<IDisposable> _listenerSubscriptions;
	private readonly Func<string, ListenerHandler> _handlerFactory;
	private readonly Func<DiagnosticListener, bool> _diagnosticSourceFilter;
	private readonly Func<string, object?, object?, bool>? _isEnabledFilter;
	private readonly Action<string, string, Exception> _logUnknownException;
	private long _disposed;
	private IDisposable? _allSourcesSubscription;

	public CustomDiagnosticSourceSubscriber(ListenerHandler handler,
		Func<string, object?, object?, bool>? isEnabledFilter,
		Action<string, string, Exception> logUnknownException)
		: this(_ => handler, value => true, isEnabledFilter, logUnknownException)
	{
	}

	public CustomDiagnosticSourceSubscriber(Func<string, ListenerHandler> handlerFactory,
		Func<DiagnosticListener, bool> diagnosticSourceFilter,
		Func<string, object?, object?, bool>? isEnabledFilter,
		Action<string, string, Exception> logUnknownException)
	{
		_listenerSubscriptions = new List<IDisposable>();
		_handlerFactory = handlerFactory;
		_diagnosticSourceFilter = diagnosticSourceFilter;
		_isEnabledFilter = isEnabledFilter;
		_logUnknownException = logUnknownException;
	}

	public void Subscribe()
	{
		_allSourcesSubscription ??= DiagnosticListener.AllListeners.Subscribe(this);
	}

	public void OnNext(DiagnosticListener value)
	{
		if ((Interlocked.Read(ref _disposed) == 0) &&
		    _diagnosticSourceFilter(value))
		{
			var handler = _handlerFactory(value.Name);
			var listener = new DiagnosticSourceListener(handler, _logUnknownException);
			var subscription = _isEnabledFilter == null ? value.Subscribe(listener) : value.Subscribe(listener, _isEnabledFilter);

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
