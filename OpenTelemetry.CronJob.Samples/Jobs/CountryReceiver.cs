using Counties.Client;

namespace OpenTelemetry.CronJob.Sample.Jobs;

internal sealed class CountryReceiver : Instrumentation.BackgroundService.WorkerService
{
	private readonly ICountiesClient _countiesClient;
	private readonly ILogger<CountryReceiver> _logger;
	private readonly IHostApplicationLifetime _applicationLifetime;

	public CountryReceiver(ICountiesClient countiesClient, ILogger<CountryReceiver> logger, IHostApplicationLifetime applicationLifetime)
	{
		_countiesClient = countiesClient;
		_logger = logger;
		_applicationLifetime = applicationLifetime;
	}

	protected override async Task Execute(CancellationToken stoppingToken)
	{
		var country = await _countiesClient.GetCountry(stoppingToken);

		_logger.LogInformation("Country received. Value: {Value}", country);

		_applicationLifetime.StopApplication();
	}

	private record Country(string Ip, string CountryCode);
}
