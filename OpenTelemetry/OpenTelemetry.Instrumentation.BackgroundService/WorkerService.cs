using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Exception = System.Exception;

namespace OpenTelemetry.Instrumentation.BackgroundService;

public abstract class WorkerService : Microsoft.Extensions.Hosting.BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly SchedulerOptions? _schedulerOptions;

	protected WorkerService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected WorkerService(IServiceProvider serviceProvider, SchedulerOptions schedulerOptions) : this(serviceProvider)
	{
		_schedulerOptions = schedulerOptions;
	}

	protected virtual Task Start(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public sealed override async Task StartAsync(CancellationToken cancellationToken)
	{
		await Start(cancellationToken);
		await base.StartAsync(cancellationToken);
	}

	protected abstract Task Execute(IServiceProvider scope);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (stoppingToken.IsCancellationRequested == false)
		{
			using var activity = BackgroundServiceActivitySource.ActivitySource.StartActivity(GetType().Name);
			using var scope = _serviceProvider.CreateScope();

			try
			{
				// here need setup span attributes
				activity?.AddTag("job.type", "worker");

				await Execute(scope.ServiceProvider);

				activity?.SetStatus(ActivityStatusCode.Ok);
			}
			catch (Exception ex)
			{
				activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
				activity?.AddException(ex);
			}

			await DelayOnDemand(stoppingToken);
		}
	}

	protected virtual Task Stop(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public sealed override async Task StopAsync(CancellationToken cancellationToken)
	{
		await Stop(cancellationToken);
		await base.StopAsync(cancellationToken);
	}

	private async Task DelayOnDemand(CancellationToken stoppingToken)
	{
		if (_schedulerOptions is not null)
		{
			await Task.Delay(new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: _schedulerOptions.IntervalSec).Milliseconds, stoppingToken);
		}
	}

	public sealed record SchedulerOptions(int IntervalSec);
}
