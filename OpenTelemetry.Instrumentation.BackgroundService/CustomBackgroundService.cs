using System.Diagnostics;

namespace OpenTelemetry.Instrumentation.BackgroundService;

public abstract class CustomBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
{
	private Activity? _activity;

	public override Task StartAsync(CancellationToken cancellationToken)
	{
		_activity = BackgroundServiceActivitySource.ActivitySource.StartActivity();
		return base.StartAsync(cancellationToken);
	}

	public override Task StopAsync(CancellationToken cancellationToken)
	{
		_activity?.SetStatus(ActivityStatusCode.Ok);
		_activity?.Stop();
		return base.StopAsync(cancellationToken);
	}

	public override void Dispose()
	{
		_activity?.Dispose();
		base.Dispose();
	}
}
