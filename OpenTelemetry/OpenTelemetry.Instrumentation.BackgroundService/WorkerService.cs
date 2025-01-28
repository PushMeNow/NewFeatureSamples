using System.Diagnostics;
using OpenTelemetry.Trace;
using Exception = System.Exception;

namespace OpenTelemetry.Instrumentation.BackgroundService;

public abstract class WorkerService : Microsoft.Extensions.Hosting.BackgroundService
{
	private readonly SchedulerOptions? _schedulerOptions;
	protected Activity? CurrentActivity;

	protected WorkerService()
	{
	}

	protected WorkerService(SchedulerOptions schedulerOptions)
	{
		_schedulerOptions = schedulerOptions;
	}

	public virtual Task Start(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public sealed override async Task StartAsync(CancellationToken cancellationToken)
	{
		await Start(cancellationToken);
		await base.StartAsync(cancellationToken);
	}

	protected abstract Task Execute(CancellationToken stoppingToken);

	protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		do
		{
			CurrentActivity = BackgroundServiceActivitySource.ActivitySource.StartActivity(GetType().Name);

			try
			{
				// here need setup span attributes
				CurrentActivity?.AddTag("job.type", "worker");

				await Execute(stoppingToken);

				CurrentActivity?.SetStatus(ActivityStatusCode.Ok);
			}
			catch (Exception ex)
			{
				CurrentActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
				CurrentActivity?.RecordException(ex);
			}
			finally
			{
				CurrentActivity?.Dispose();
				CurrentActivity = null;
			}

			if (_schedulerOptions is not null)
			{
				await Task.Delay(new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: _schedulerOptions.IntervalSec).Milliseconds, stoppingToken);
			}
		} while (_schedulerOptions != null && stoppingToken.IsCancellationRequested == false);
	}

	public virtual Task Stop(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public sealed override async Task StopAsync(CancellationToken cancellationToken)
	{
		await Stop(cancellationToken);
		await base.StopAsync(cancellationToken);
	}

	public sealed record SchedulerOptions(int IntervalSec);
}
