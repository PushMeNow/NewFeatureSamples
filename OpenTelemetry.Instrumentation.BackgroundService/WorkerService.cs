using System.Diagnostics;
using OpenTelemetry.Trace;
using Exception = System.Exception;

namespace OpenTelemetry.Instrumentation.BackgroundService;

public abstract class WorkerService : Microsoft.Extensions.Hosting.BackgroundService
{
	protected Activity? CurrentActivity;

	protected abstract Task Execute(CancellationToken stoppingToken);

	protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		CurrentActivity = BackgroundServiceActivitySource.ActivitySource.StartActivity(GetType().Name, ActivityKind.Server);

		try
		{
			// here need setup span attributes
			CurrentActivity.AddTag("job.type", "worker");

			await Execute(stoppingToken);

			CurrentActivity.SetStatus(ActivityStatusCode.Ok);
		}
		catch (Exception ex)
		{
			CurrentActivity.SetStatus(ActivityStatusCode.Error, ex.Message);
			CurrentActivity.RecordException(ex);
		}
		finally
		{
			CurrentActivity.Dispose();
		}
	}
}
